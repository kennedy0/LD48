using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillSparks : MonoBehaviour
{
    private ParticleSystem particles;
    private PlayerController playerController;

    private float t;
    
    void Start()
    {
        particles = GetComponent<ParticleSystem>();
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        t -= Time.deltaTime;
        t = Mathf.Clamp01(t);

        if (t > 0 && playerController.GetDrillSpeed() >= 1f)
        {
            if (!particles.isPlaying)
            {
                particles.Play();
            }
        }
        else
        {
            particles.Stop();
        }

    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Block"))
        {
            t = .1f;
        }
    }
}
