using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;


public enum VehicleMode
{
    Water,
    Land
}

public class PlayerController : MonoBehaviour
{
    [Header("Base Speeds")]
    public float WaterMoveSpeed = 1f;
    public float WaterRotateSpeed = 1f;
    
    public float LandMoveSpeed = 1f;
    public float LandRotateSpeed = 1f;
    
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
    public Animator[] WheelAnimator;
    public float MinPropSpeed = 0.1f;
    public float MinDrillSpeed = 0.1f;
    public float MinWheelSpeed = 0.1f;

    [Header("Weapons")]
    public Weapon LeftWeapon;
    public Weapon RightWeapon;
    
    [Header("Other")]
    public Transform WheelsGroup;
    public VehicleMode VehicleMode;
    public bool CanMove = true;

    private Rigidbody2D rb;

    private Camera cam;
    private bool engineOn;
    private bool drillOn;
    private float engine = 0f;
    private float drill = 0f;
    private Vector3 mousePos;

    private Vector2 velocity;
    private float rotateSpeed;

    private bool isTransitioning;
    private float leftWeaponCooldown;
    private float rightWeaponCooldown;
    private List<Weapon> weapons;

    private void Start()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        SetInitialAnimationState();
        
        weapons = new List<Weapon>();
        weapons.Add(LeftWeapon);
        weapons.Add(RightWeapon);
    }

    private void SetInitialAnimationState()
    {
        if (VehicleMode == VehicleMode.Land)
        {
            WheelAnimator[0].SetTrigger("OpenWheels");
            WheelAnimator[1].SetTrigger("OpenWheels");
        }
        else
        {
            PropAnimator.SetTrigger("OpenProp");
        }
    }

    private void Update()
    {
        GetInput();
        HandleEngine();
        HandleAnimation();
        HandleBubbleParticles();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(Transition());
        }
    }

    private bool InputEnabled()
    {
        if (!CanMove)
        {
            return false;
        }

        if (isTransitioning)
        {
            return false;
        }

        return true;
    }

    private void GetInput()
    {
        drillOn = false;
        engineOn = false;

        if (!InputEnabled())
        {
            return;
        }

        // Update Mouse Position
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        
        // Get mouse buttons
        if (Input.GetMouseButton(0))
        {
            engineOn = true;
            drillOn = true;
        }
        if (Input.GetMouseButton(1))
        {
            drillOn = true;
        }
        
        // Weapons
        HandleWeapons();
    }

    private void HandleEngine()
    {
        if (engineOn)
        {
            engine += Time.deltaTime * ThrustModifier;
            if (VehicleMode == VehicleMode.Land)
            {
                engine = 1f;
            }
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
        if (isTransitioning)
        {
            return;
        }

        if (VehicleMode == VehicleMode.Water)
        {
            MoveWater();
        }
        else
        {
            MoveLand();
        }
    }

    private void MoveWater()
    {
        // Move
        float thrust = engine * WaterMoveSpeed * MoveSpeedModifier;
        Vector2 direction = transform.TransformDirection(Vector2.up);
        velocity = direction * thrust;
        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);

        // Rotate
        float rotateForce = drill * WaterRotateSpeed * RotateSpeedModifier;
        float rotation = CalculateRotation();
        rotateSpeed = rotateForce * rotation;
        rb.MoveRotation(rb.rotation + rotateSpeed * Time.fixedDeltaTime);
    }

    private void MoveLand()
    {
        // Rotate
        float rotateForce = LandRotateSpeed * RotateSpeedModifier;
        float rotation = CalculateRotation();
        rotateSpeed = rotateForce * rotation;
        rb.MoveRotation(rb.rotation + rotateSpeed * Time.fixedDeltaTime);
        
        // Move
        float thrust = engine * LandMoveSpeed * MoveSpeedModifier;
        if (!engineOn)
        {
            thrust = 0f;
        }
        Vector2 direction = transform.TransformDirection(Vector2.up);
        velocity = direction * thrust;
        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
    }

    private float CalculateTargetAngle()
    {
        Vector3 direction = mousePos - transform.position;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        targetAngle -= 90f;  // idk why, hacky fix
        return targetAngle;
    }

    private float CalculateRotation()
    {
        // Calculate target angle
        float targetAngle = CalculateTargetAngle();

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

        float wheelSpeed = velocity.magnitude;
        WheelAnimator[0].SetFloat("WheelAnimationSpeed", Mathf.Max(wheelSpeed, MinWheelSpeed));
        WheelAnimator[1].SetFloat("WheelAnimationSpeed", Mathf.Max(wheelSpeed, MinWheelSpeed));
    }

    private void HandleBubbleParticles()
    {
        var emission = BubbleParticles.emission;
        
        if (VehicleMode == VehicleMode.Land || isTransitioning)
        {
            emission.enabled = false;
            return;
        }

        emission.enabled = true;
        emission.rateOverTime = Mathf.Lerp(0f, EmissionRate, engine);
    }

    public IEnumerator Transition()
    {
        isTransitioning = true;
        velocity = Vector2.zero;
        
        if (VehicleMode == VehicleMode.Land)
        {
            WheelAnimator[0].SetTrigger("CloseWheels");
            WheelAnimator[1].SetTrigger("CloseWheels");
            PropAnimator.SetTrigger("OpenProp");
            while (!PropAnimator.GetCurrentAnimatorStateInfo(0).IsName("Prop_Spin") &&
                   !WheelAnimator[0].GetCurrentAnimatorStateInfo(0).IsName("Wheel_Off"))
            {
                yield return new WaitForSeconds(Time.deltaTime);
            }

            VehicleMode = VehicleMode.Water;
        }
        else
        {
            WheelAnimator[0].SetTrigger("OpenWheels");
            WheelAnimator[1].SetTrigger("OpenWheels");
            PropAnimator.SetTrigger("CloseProp");
            while (!PropAnimator.GetCurrentAnimatorStateInfo(0).IsName("Prop_Off") &&
                   !WheelAnimator[0].GetCurrentAnimatorStateInfo(0).IsName("Wheel_On"))
            {
                yield return new WaitForSeconds(Time.deltaTime);
            }
            
            VehicleMode = VehicleMode.Land;
        }

        isTransitioning = false;
    }

    public float GetEngineSpeed()
    {
        return engine;
    }

    public bool EngineIsOn()
    {
        return engineOn;
    }

    private void HandleWeapons()
    {
        if (!drillOn && !engineOn)
        {
            return;
        }

        foreach (Weapon weapon in weapons)
        {
            
        }
    }
}
