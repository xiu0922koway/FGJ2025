using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    // Start is called before the first frame update
    public List<AudioClip> audioClips;
    public List<AudioClip> bgms;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayClip(int index, float volumn)
    {
        this.GetComponent<AudioSource>().PlayOneShot(audioClips[index], volumn);
    }
}
