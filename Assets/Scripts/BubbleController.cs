using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BubbleController : MonoBehaviour
{
    public Transform bubble;
    public PlayerInputAction inputActions; 
    // Start is called before the first frame update
    void Awake()
    {
        inputActions = new PlayerInputAction();
    }
    
    void OnEnable()
    {
        
    }
    // Update is called once per frame
    void Update()
    {

    }
}
