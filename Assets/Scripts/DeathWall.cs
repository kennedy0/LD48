using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DeathWall : MonoBehaviour
{
    public List<string> DeathMessages;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            string message = DeathMessages[Random.Range(0, DeathMessages.Count)];
            StartCoroutine(other.GetComponent<PlayerController>().Die(message));
        }
    }
}
