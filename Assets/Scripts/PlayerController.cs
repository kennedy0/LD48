using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Base Speeds")]
    public float BaseMoveSpeed = 50f;
    public float BaseRotateSpeed = 50f;
    
    [Header("Modifiers")]
    public float MoveSpeedModifier = 1f;
    public float ThrustModifier = 1f;
    public float RotateSpeedModifier = 1f;

    [Header("Physics")]
    public float Drag;
    public float AngularDrag;

    [Header("Particles")]
    public ParticleSystem BubbleParticles;
    public float EmissionRate;

    [Header("Animation")]
    public Animator PropAnimator;
    public Animator DrillAnimator;
    public float MinPropSpeed = 0.1f;
    public float MinDrillSpeed = 0.1f;

    private Rigidbody2D rb;

    private Camera cam;
    private bool engineOn;
    private bool drillOn;
    private float engine = 0f;
    private float drill = 0f;
    private Vector3 mousePos;

    private Vector2 velocity;
    private float rotateSpeed;

    private void Start()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        GetInput();
        GetMousePos();
        HandleEngine();
        HandleAnimation();
        HandleParticles();
    }

    private void GetInput()
    {
        drillOn = false;
        engineOn = false;
        if (Input.GetMouseButton(0))
        {
            engineOn = true;
            drillOn = true;
        }
        if (Input.GetMouseButton(1))
        {
            drillOn = true;
        }
    }

    private void GetMousePos()
    {
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
    }

    private void HandleEngine()
    {
        if (engineOn)
        {
            engine += Time.deltaTime * ThrustModifier;
        }
        else
        {
            engine -= Time.deltaTime * Drag;
        }
        
        if (engineOn || drillOn)
        {
            drill += Time.deltaTime * ThrustModifier;
        }
        else
        {
            drill -= Time.deltaTime * AngularDrag;
        }

        engine = Mathf.Clamp01(engine);
        drill = Mathf.Clamp01(drill);
    }

    private void FixedUpdate()
    {
        // Move
        float thrust = engine * BaseMoveSpeed * MoveSpeedModifier;
        Vector2 direction = transform.TransformDirection(Vector2.up);
        velocity = direction * thrust;
        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);

        // Rotate
        float rotateForce = drill * BaseRotateSpeed * RotateSpeedModifier;
        float rotation = CalculateRotation();
        rotateSpeed = rotateForce * rotation;
        rb.MoveRotation(rb.rotation + rotateSpeed * Time.fixedDeltaTime);
    }

    private float CalculateRotation()
    {
        // Calculate target angle
        Vector3 direction = mousePos - transform.position;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        targetAngle -= 90f;  // idk why, hacky fix

        // Calculate rotation to reach target angle
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

    private void HandleAnimation()
    {
        PropAnimator.SetFloat("PropAnimationSpeed", Mathf.Max(engine, MinPropSpeed));
        DrillAnimator.SetFloat("DrillAnimationSpeed", Mathf.Max(drill, MinDrillSpeed));
    }

    private void HandleParticles()
    {
        var emission = BubbleParticles.emission;
        emission.rateOverTime = Mathf.Lerp(0f, EmissionRate, engine);
    }
}
