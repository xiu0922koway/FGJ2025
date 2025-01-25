using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    public bool isGenerated;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isGenerated)
        {
            this.transform.position += Vector3.up * 0.6f * Time.deltaTime;
        }
    }
}
