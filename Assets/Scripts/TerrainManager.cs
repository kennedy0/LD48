using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Terrain
{
    Water,
    Land
}

public class TerrainManager : MonoBehaviour
{
    public Terrain CurrentTerrain;
    public float XOffset;
    
    private GameObject player;
    private PlayerController playerController;
    private Transform playerTransform;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        playerTransform = player.GetComponent<Transform>();
    }

    private void Update()
    {
        SetCurrentTerrain();
    }

    private void SetCurrentTerrain()
    {
        if (playerTransform.position.x > XOffset)
        {
            CurrentTerrain = Terrain.Land;
        }
        else if (playerTransform.position.x < -XOffset)
        {
            CurrentTerrain = Terrain.Water;
        }
    }
}
