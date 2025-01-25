using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.InputSystem;

public class BubbleController1 : MonoBehaviour
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
    public float inputValue;
    private float sizeDelta;
    public float sizeMultiplier = 1;

    public float bubbleEndSizeDelta = 0.5f;

    public bool bubbleEnd;
    public bool startBlow;
    public bool isBlowing;
    public bool isBreathing;

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
        if((rightTriggerValue + leftTriggerValue)/2 > 0)
        {
            startBlow = true;
        }

        if(startBlow)
        {
            CheckBlow();
            if(isBlowing)
            {
                BlowBubble();
            }
            else
            {
                BreatheIn();
            }
        }
    }

    void CheckBlow()
    {
        
        if((leftTriggerValue) > inputValue)
        {
            isBlowing = true;
        }
        else
        {
            isBlowing = false;
        }

        inputValue = leftTriggerValue;
    }

    void BlowBubble()
    {
        Debug.Log("Blow");
        
        blowDelta += blowSpeed * leftTriggerValue;
        blowDelta = Mathf.Clamp(blowDelta, 0, maxSpeed);

        sizeDelta = sizeMultiplier / Mathf.Pow(bubble.transform.localScale.x,0);
        sizeDelta = Mathf.Clamp(sizeDelta, 0.1f, 1);
        
        bubble.transform.localScale += Vector3.one * blowDelta * sizeDelta * Time.deltaTime;
    }

    void BreatheIn()
    {
        Debug.Log("BREATHIN");
        
        blowDelta -= slowSpeed * Time.deltaTime;
        blowDelta = Mathf.Clamp(blowDelta, -0.025f * maxSpeed, maxSpeed);

        sizeDelta = sizeMultiplier / Mathf.Pow(bubble.transform.localScale.x,0);
        sizeDelta = Mathf.Clamp(sizeDelta, 0.1f, 1);

        bubble.transform.localScale += Vector3.one * blowDelta * sizeDelta * Time.deltaTime;
    }

    void BubblePop()
    {
        Debug.Log("Pop");
        startBlow = false;
        Destroy(bubble);
        bubble = null;

        willGenerate = true;
    }

    void BubbleGenerate()
    {
        Debug.Log("Generate");
        startBlow = false;
        bubble.GetComponent<Bubble>().isGenerated = true;
        bubble = null;

        willGenerate = true;
    }
}
