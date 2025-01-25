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

    private Vector3 initialSize;

    public float size;

    void Start()
    {
        initialSize = instructor.localScale;
        size = initialSize.x;
    }

    void Update()
    {
        if(isBlowing)
        {
            instructor.localScale += Vector3.one * inputCurve.Evaluate(blowTimer/duration) * Time.deltaTime;
            blowTimer += Time.deltaTime;
        }

    }

    public void Reset()
    {
        isBlowing = false;
        instructor.localScale = initialSize;
        blowTimer = 0;
    }
}
