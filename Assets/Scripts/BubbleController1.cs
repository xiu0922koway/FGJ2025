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
    private Transform generatePoint;
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
    public float leftPreviousValue;
    public float rightPreviousValue;
    private float sizeDelta;
    public float sizeMultiplier = 1;
    public bool startBlow;
    public bool isBlowing;

    private bool willGenerate;
    private float generateTimer;
    private float generateTime = 3;
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
        generatePoint = this.transform.GetChild(0);
        if(bubble == null) 
        {
            bubble = Instantiate(bubblePrefab,generatePoint.position,Quaternion.identity, generatePoint);
            GameManager.Instance.bubbles.Add(bubble);
        }
    }

    private float safeBlowTime = 2;
    private float safeBlowTimer;
    private bool canEnd;
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
                bubble = Instantiate(bubblePrefab,generatePoint.position,Quaternion.identity, generatePoint);
                GameManager.Instance.bubbles.Add(bubble);

                willGenerate = false;
                ResetField();
            }
        }
        else
        {
            if((rightTriggerValue + leftTriggerValue)/2 > 0)
            {
                startBlow = true;
                isBlowing = true;
            }

            if(startBlow)
            {
                if(safeBlowTimer < safeBlowTime)
                {
                    safeBlowTimer += Time.deltaTime;
                }
                else
                {
                    canEnd = true;
                }
                
                if(CheckBlow()) return;
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
    }

    bool CheckBlow()
    {
        
        if(Mathf.Max(leftTriggerValue, rightTriggerValue) == 1)
        {
            if(canEnd)
            {
                BubblePop();
                return true;
            }
            

        }
        if(!isBlowing && leftTriggerValue == 0)
        {
            BubbleGenerate();
            return true;
        }
        if(!isBlowing && rightTriggerValue == 0)
        {
            BubbleGenerate();
            return true;
        }


        if(leftTriggerValue > leftPreviousValue && rightTriggerValue > rightPreviousValue)
        {
            isBlowing = true;
        }
        else
        {
            isBlowing = false;
        }


        leftPreviousValue = leftTriggerValue;
        rightPreviousValue = rightTriggerValue;

        return false;
    }

    void BlowBubble()
    {
        Debug.Log("Blow");
        
        blowDelta += blowSpeed * (leftTriggerValue + rightTriggerValue)/2;
        blowDelta = Mathf.Clamp(blowDelta, 0, maxSpeed);

        sizeDelta = sizeMultiplier / Mathf.Pow(bubble.transform.localScale.x,0);
        sizeDelta = Mathf.Clamp(sizeDelta, 0.1f, 1);
        
        bubble.transform.localScale += Vector3.one * blowDelta * sizeDelta * Time.deltaTime;
        if(bubble.transform.position.x > 0)bubble.transform.position += Vector3.left * blowDelta * sizeDelta * Time.deltaTime * 2;
    }

    void BreatheIn()
    {
        Debug.Log("BREATHIN");
        
        blowDelta -= slowSpeed * Time.deltaTime;
        blowDelta = Mathf.Clamp(blowDelta, -0.005f * maxSpeed, maxSpeed);

        sizeDelta = sizeMultiplier / Mathf.Pow(bubble.transform.localScale.x,1.5f);
        sizeDelta = Mathf.Clamp(sizeDelta, 0.1f, 1);

        bubble.transform.localScale += Vector3.one * blowDelta * sizeDelta * Time.deltaTime;
    }

    void BubblePop()
    {
        Debug.Log("Pop");
        startBlow = false;
        
        bubble.GetComponent<Animator>().SetTrigger("Brust");
        //Destroy(bubble);
        bubble = null;

        willGenerate = true;

    }

    void BubbleGenerate()
    {
        Debug.Log("Generate");
        startBlow = false;
        bubble.GetComponent<Bubble>().isGenerated = true;

        if(!GameManager.Instance.isStart)
        {
            GameManager.Instance.isStart = true;
        }
        else
        {
            CalculateScore(bubble.transform.localScale.x);
        }

        bubble = null;

        willGenerate = true;

    }

    void ResetField()
    {
        canEnd = false;
        safeBlowTimer = 0;
        blowDelta = 0;
    }

    void CalculateScore(float number)
    {
        GameManager.Instance.AddScore(number);
    }
}
