using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;


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
    public bool CanShoot;
    public GameObject LeftWeaponObject;
    public GameObject RightWeaponObject;
    public Weapon PlayerWeapon;
    public Transform LeftWeaponSpawnPoint;
    public Transform RightWeaponSpawnPoint;

    [Header("Oxygen")]
    public float Oxygen;
    public float MaxOxygen;
    public float OxygenDrainRate;
    public float OxygenRegenRate;
    
    [Header("Heat")]
    public float Heat;
    public float MaxHeat;
    public float HeatBuildRate;
    public float HeatDecayRate;

    [Header("Drill")]
    public CircleCollider2D DrillCollider;
    
    [Header("Other")]
    public VehicleMode VehicleMode;
    public bool CanMove = true;
    public bool IsDead = false;
    public float PostDeathTime = 2f;
    public MessageBoxScript MessageBox;
    public List<string> WaterDeathMessages;
    public List<string> LandDeathMessages;
    public bool isTutorial;

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
    private float weaponCooldown;
    private bool fireLeft = true;
    private TerrainManager terrainManager;

    private void Start()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        SetInitialAnimationState();
        terrainManager = GameObject.Find("MANAGER").GetComponent<TerrainManager>();
        
        // Setup
        IsDead = false;
        Oxygen = MaxOxygen;
        Heat = 0f;
        CanShoot = false;
        LeftWeaponObject.SetActive(false);
        RightWeaponObject.SetActive(false);
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
        HandleDeath();

        GetInput();
        HandleEngine();
        HandleAnimation();
        HandleBubbleParticles();
        HandleOxygen();
        HandleHeat();
        HandleDrillObject();

        if (TerrainMismatch() && !isTransitioning)
        {
            StartCoroutine(Transition());
        }
    }

    private bool TerrainMismatch()
    {
        if (terrainManager.CurrentTerrain == Terrain.Land && VehicleMode == VehicleMode.Water)
        {
            return true;
        }
        if (terrainManager.CurrentTerrain == Terrain.Water && VehicleMode == VehicleMode.Land)
        {
            return true;
        }

        return false;
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
        weaponCooldown -= Time.deltaTime;
        if (weaponCooldown < 0f)
        {
            weaponCooldown = 0f;
        }
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

    public float GetDrillSpeed()
    {
        return drill;
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

        if (!CanShoot)
        {
            return;
        }

        Transform spawnPoint;
        if (fireLeft)
        {
            spawnPoint = LeftWeaponSpawnPoint;
        }
        else
        {
            spawnPoint = RightWeaponSpawnPoint;
        }

        if (weaponCooldown <= 0f)
        {
            GameObject projectile = Instantiate(PlayerWeapon.Projectile, spawnPoint.position, spawnPoint.rotation);
            projectile.GetComponent<Rigidbody2D>().AddForce(projectile.transform.up * PlayerWeapon.Speed, ForceMode2D.Impulse);
            weaponCooldown = PlayerWeapon.Cooldown;
            fireLeft = !fireLeft;
        }
    }

    private void HandleHeat()
    {
        if (terrainManager.CurrentTerrain == Terrain.Land)
        {
            Heat += Time.deltaTime * HeatBuildRate;
        }
        else
        {
            Heat -= Time.deltaTime * HeatDecayRate;
        }

        Heat = Mathf.Clamp(Heat, 0f, MaxHeat);
    }

    private void HandleOxygen()
    {
        if (terrainManager.CurrentTerrain == Terrain.Water)
        {
            Oxygen -= Time.deltaTime * OxygenDrainRate;
        }
        else
        {
            Oxygen += Time.deltaTime * OxygenRegenRate;
        }

        Oxygen = Mathf.Clamp(Oxygen, 0f, MaxOxygen);
    }

    private void HandleDeath()
    {
        if (IsDead)
        {
            return;
        }

        if (Oxygen <= 0f)
        {
            string message = WaterDeathMessages[Random.Range(0, WaterDeathMessages.Count)];
            StartCoroutine(Die(message));
        }
        else if (Heat >= MaxHeat)
        {
            string message = LandDeathMessages[Random.Range(0, LandDeathMessages.Count)];
            StartCoroutine(Die(message));
        }
    }

    public IEnumerator Die(string message)
    {
        // Set variables
        IsDead = true;
        CanMove = false;
        CanShoot = false;
        HandleHighScore();
        cam.GetComponent<CameraScroll>().IsScrollingDown = false;

        if (isTutorial)
        {
            yield break;
        }

        // Pause
        yield return new WaitForSeconds(PostDeathTime);
        
        // Show message
        List<string> m = new List<string>();
        m.Add(message);
        StartCoroutine(MessageBox.ShowMessages(m));
        
        // Pause
        yield return new WaitForSeconds(PostDeathTime);
        
        // Main Menu
        SceneManager.LoadScene("MainMenu");
    }

    public void HandleDrillObject()
    {
        if (drillOn && drill > 0f)
        {
            DrillCollider.enabled = true;
        }
        else
        {
            DrillCollider.enabled = false;
        }
    }

    public void HandleHighScore()
    {
        int highScore = 0;
        if (PlayerPrefs.HasKey("highscore"))
        {
            highScore = PlayerPrefs.GetInt("highscore");
        }

        int currentScore = Mathf.FloorToInt(Mathf.Abs(cam.transform.position.y) * 5);
        if (currentScore > highScore)
        {
            PlayerPrefs.SetInt("highscore", currentScore);
        }
    }
}
