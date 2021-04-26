using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockDestroyAudio : MonoBehaviour
{
    public AudioClip[] DestructionSound;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayRockSound()
    {
        audioSource.PlayOneShot(DestructionSound[Random.Range(0, DestructionSound.Length - 1)], audioSource.volume);
    }
}
