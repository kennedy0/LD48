using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheels : MonoBehaviour
{
    private GameObject player;
    private Rigidbody2D rb;
    private Rigidbody2D playerRb;
    private PlayerController playerController;
    private Transform playerTransform;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        playerTransform = player.GetComponent<Transform>();
        playerRb = player.GetComponent<Rigidbody2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        transform.position = playerTransform.position;
    }

    private void FixedUpdate()
    {
        if (playerController.EngineIsOn())
        {
            HandleRotation();
        }
    }

    private void HandleRotation()
    {
        // Rotate
        float rotateForce = playerController.LandRotateSpeed * playerController.RotateSpeedModifier;
        float rotation = CalculateRotation();
        float rotateSpeed = playerController.GetEngineSpeed() * rotateForce * rotation;
        rb.MoveRotation(rb.rotation + rotateSpeed * Time.fixedDeltaTime);
    }

    private float CalculateRotation()
    {
        // Calculate rotation to reach target angle
        float targetAngle = playerRb.rotation;
        float rotation = targetAngle - rb.rotation;
        rotation %= 360f;

        // Wrap values greater than 180
        if (rotation < -180f)
        {
            rotation += 360f;
        }
        else if (rotation > 180f)
        {
            rotation -= 360f;
        }

        Mathf.Clamp(rotation, -1f, 1f);
        return rotation;
    }
}
