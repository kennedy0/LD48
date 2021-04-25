using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioLooper : MonoBehaviour
{
    private AudioSource audioSource;
    public float LoopStart;
    public float LoopEnd;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (audioSource.isPlaying && audioSource.time > LoopEnd)
        {
            audioSource.time = LoopStart;
        }
    }
}
