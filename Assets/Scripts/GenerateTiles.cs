using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GenerateTiles : MonoBehaviour
{
    public Tile BlueTile;
    
    public GameObject TileHitbox;
    public float TileSize = 16f;
    public float XOffset = 3.5f;
    public float ChunkWidth = 28;
    public int ChunkHeight = 10;
    
    private Tilemap tilemap;
    private GameObject player;

    private void Start()
    {
        tilemap = transform.Find("Tilemap").GetComponent<Tilemap>();
        player = GameObject.FindWithTag("Player");
        
        GenerateChunk(-XOffset, 0f);
    }

    private void Update()
    {
        
    }

    private void GenerateChunk(float x, float y)
    {
        for (int j = 0; j < ChunkHeight; j++)
        {
            for (int i  = 0; i < ChunkWidth; i++)
            {
                // Vector3Int tilePos = tilemap.WorldToCell();
                // tilemap.SetTile(tilePos, BlueTile);
            }
        }
    }
}
