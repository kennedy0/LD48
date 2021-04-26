using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScroll : MonoBehaviour
{
    public float Speed;
    public float ScreenXOffset = 4f;
    public float TransitionTime = 2f;
    public float PauseAfterTransition = 1f;
    public float SpeedTransitionTime = 5f;

    public bool IsScrollingDown = false;
    private GameObject player;
    private PlayerController playerController;
    private Transform playerTransform;
    private TerrainManager terrainManager;
    private bool isTransitioning;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        playerTransform = player.GetComponent<Transform>();
        terrainManager = GameObject.Find("MANAGER").GetComponent<TerrainManager>();

        // Start camera at zero speed
        float s = Speed;
        Speed = 0f;
        StartCoroutine(SetCameraSpeed(s));
    }

    void Update()
    {
        if (IsScrollingDown && !isTransitioning)
        {
            ScrollDown();
        }

        if (TerrainMismatch() && !isTransitioning)
        {
            StartCoroutine(Transition());
        }
    }
    
    private bool TerrainMismatch()
    {
        if (terrainManager.CurrentTerrain == Terrain.Land && transform.position.x < 0f)
        {
            return true;
        }
        if (terrainManager.CurrentTerrain == Terrain.Water && transform.position.x > 0f)
        {
            return true;
        }

        return false;
    }

    private void ScrollDown()
    {
        transform.position += Speed * Time.deltaTime * Vector3.down;
    }

    private IEnumerator Transition()
    {
        isTransitioning = true;
        Time.timeScale = 0f;

        Vector3 currentPosition = transform.position;
        Vector3 targetPosition = transform.position;
        if (transform.position.x < 0f)
        {
            targetPosition.x = ScreenXOffset;
        }
        else
        {
            targetPosition.x = -ScreenXOffset;
        }

        float t = TransitionTime;
        while (t > 0f)
        {
            float p = (TransitionTime - t) / TransitionTime;
            float x = Mathf.SmoothStep(currentPosition.x, targetPosition.x, p);
            float y = Mathf.SmoothStep(currentPosition.y, targetPosition.y, p);
            float z = Mathf.SmoothStep(currentPosition.z, targetPosition.z, p);
            transform.position = new Vector3(x, y, z);
            
            t -= Time.unscaledDeltaTime;
            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
        }

        Time.timeScale = 1f;
        
        yield return new WaitForSeconds(PauseAfterTransition);
        isTransitioning = false;
    }

    private IEnumerator SetCameraSpeed(float speed)
    {
        float t = SpeedTransitionTime;
        float startSpeed = Speed;
        while (t > 0f)
        {
            Speed = Mathf.SmoothStep(speed, startSpeed, t / SpeedTransitionTime);
            t -= Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
}
