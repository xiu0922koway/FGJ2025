using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BubbleInstruction : Singleton<BubbleInstruction>
{
    public List<float> sizeIndicator;

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

    }

    public void Reset()
    {
        isBlowing = false;
        instructor.localScale = initialSize;
        blowTimer = 0;
    }
}
