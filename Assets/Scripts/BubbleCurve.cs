using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BubbleInstruction : Singleton<BubbleInstruction>
{
    public AnimationCurve inputCurve;
    public float duration = 3;

    public Transform instructor;
    public bool isBlowing;
    public float blowTimer;

    void Start()
    {

    }

    void Update()
    {
        if(isBlowing)
        {
            instructor.localScale += (Vector3.right + Vector3.up) * inputCurve.Evaluate(blowTimer/duration) * Time.deltaTime;
            blowTimer += Time.deltaTime;
        }
    }
}
