using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class BubbleController : MonoBehaviour
{
    public Transform bubble;
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

    void Update()
    {
        
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

        sizeDelta = sizeMultiplier / Mathf.Pow(bubble.lossyScale.x,0);
        sizeDelta = Mathf.Clamp(sizeDelta, 0.1f, 1);
        
        bubble.localScale += Vector3.one * blowDelta * sizeDelta * Time.deltaTime;
    }
}
