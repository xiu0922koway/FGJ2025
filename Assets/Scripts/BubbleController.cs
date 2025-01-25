using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.InputSystem;

public class BubbleController : MonoBehaviour
{
    private GameObject bubble;
    public GameObject bubblePrefab;
    private Transform instructor;
    public PlayerInputAction inputActions;
    public float leftTriggerValue;
    public float rightTriggerValue;


    [Header("吹")]
    public float blowSpeed = 0.5f;
    public float slowSpeed = 20;
    public float maxSpeed = 5;
    public float blowDelta;
    private float inputValue;
    private float sizeDelta;
    public float sizeMultiplier = 1;

    public float bubbleEndSizeDelta = 0.5f;

    public bool bubbleEnd;
    public bool isBlowing;

    private bool willGenerate;
    private float generateTimer;
    private float generateTime = 2;
    // Start is called before the first frame update
    void Awake()
    {
        inputActions = new PlayerInputAction();
    }
    
    void OnEnable()
    {
        inputActions.PlayerActionMap.Enable();
        
        inputActions.PlayerActionMap.LeftTrigger.performed += OnLeftTrigger;
        inputActions.PlayerActionMap.LeftTrigger.canceled += OnLeftTrigger;

        inputActions.PlayerActionMap.RightTrigger.performed += OnRightTrigger;
        inputActions.PlayerActionMap.RightTrigger.canceled += OnRightTrigger;

    }
    
    void OnDisable()
    {
        inputActions.PlayerActionMap.Disable();
    }

    void OnLeftTrigger(InputAction.CallbackContext context)
    {
        leftTriggerValue = context.ReadValue<float>();
    }
    void OnRightTrigger(InputAction.CallbackContext context)
    {
        rightTriggerValue = context.ReadValue<float>();
    }

    void Start()
    {
        instructor = BubbleInstruction.Instance.instructor;
        if(bubble == null) 
        {
            bubble = Instantiate(bubblePrefab,Vector3.zero,Quaternion.identity);
        }
    }

    void Update()
    {
        if(willGenerate)
        {
            if(generateTimer < generateTime)
            {
                generateTimer += Time.deltaTime;
            }
            else
            {
                generateTimer = 0;
                bubble = Instantiate(bubblePrefab,Vector3.zero,Quaternion.identity);
                willGenerate = false;
            }
        }
        else
        {
            CheckBubbleEnd();
            if(isBlowing)
            {
                BubbleSizeControl();
            }
        }
    }

    void CheckBubbleEnd()
    {
        if(!isBlowing)
        {
            if(leftTriggerValue > 0 || rightTriggerValue > 0)
            {
                isBlowing = true;
            }
        }
        else
        {   
            if(bubble.transform.localScale.magnitude > instructor.localScale.magnitude + bubbleEndSizeDelta)
            {
                BubblePop();
            }
            else if(bubble.transform.localScale.magnitude < instructor.localScale.magnitude - 2* bubbleEndSizeDelta)
            {
                BubbleGenerate();
            }
        }
    }

    void BubblePop()
    {
        Debug.Log("Pop");
        isBlowing = false;
        Destroy(bubble);
        bubble = null;

        BubbleInstruction.Instance.Reset();
        willGenerate = true;
    }
    void BubbleGenerate()
    {
        Debug.Log("Generate");
        isBlowing = false;
        bubble.GetComponent<Bubble>().isGenerated = true;
        bubble = null;

        BubbleInstruction.Instance.Reset();
        willGenerate = true;
    }

    void BubbleSizeControl()
    {
        inputValue = (leftTriggerValue + rightTriggerValue)/2;
        
        if(inputValue == 0)
        {
            blowDelta -= slowSpeed * Time.deltaTime;
        }
        else
        {
            blowDelta += blowSpeed * inputValue;
            BubbleInstruction.Instance.isBlowing = true;
        }
        blowDelta = Mathf.Clamp(blowDelta, 0, maxSpeed);

        sizeDelta = sizeMultiplier / Mathf.Pow(bubble.transform.lossyScale.x,0);
        sizeDelta = Mathf.Clamp(sizeDelta, 0.1f, 1);
        
        bubble.transform.localScale += Vector3.one * blowDelta * sizeDelta * Time.deltaTime;
    }
}
