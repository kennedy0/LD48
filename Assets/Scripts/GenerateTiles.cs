using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GenerateTiles : MonoBehaviour
{
    public Tile BlueTile;
    public Tile RedTile;
    public Tile BlackTile;
    
    public GameObject TileHitbox;
    public float TileSize = 16f;
    public float XOffset = 3.5f;
    public int ChunkWidth = 28;
    public int ChunkHeight = 10;
    
    private Tilemap tilemap;
    private GameObject player;
    public float lastChunkY;

    private float pixelScale;
    private float tileSize;
    private float halfTileSize;
    private float chunkWidthWorldUnits;
    private float chunkHeightWorldUnits;

    private void Start()
    {
        tilemap = transform.Find("Tilemap").GetComponent<Tilemap>();
        player = GameObject.FindWithTag("Player");
        
        pixelScale = 1 / 64f;
        tileSize = TileSize * pixelScale;;
        halfTileSize = tileSize / 2f;
        chunkWidthWorldUnits = ChunkWidth * tileSize;
        chunkHeightWorldUnits = ChunkHeight * tileSize;
        
        GenerateStartingChunks(5);
    }
    
    private void GenerateStartingChunks(int chunks)
    {
        for (int i = 0; i < chunks; i++)
        {
            float y = i * chunkHeightWorldUnits;
            GenerateChunks(y);
        }
    }

    private void Update()
    {
        float playerY = player.transform.position.y;
        if (playerY < lastChunkY - chunkHeightWorldUnits)
        {
            GenerateChunks(playerY);
        }
    }

    private void GenerateChunks(float y)
    {
        GenerateChunk(-XOffset, y, BlueTile);
        GenerateChunk(XOffset, y, RedTile);
    }

    private void GenerateChunk(float x, float y, Tile tile)
    {
        lastChunkY = y;
        for (int j = 0; j < ChunkHeight; j++)
        {
            for (int i = 0; i < ChunkWidth; i++)
            {
                if (i == 0 || i == ChunkWidth - 1)
                {
                    float yPos = y - halfTileSize - (tileSize * j);
                    float xPos = x - (chunkWidthWorldUnits / 2f) + halfTileSize + (tileSize * i);
                    Vector3 tilePosWorld = new Vector3(xPos, yPos, 0f);
                    Vector3Int tilePos = tilemap.WorldToCell(tilePosWorld);
                    tilemap.SetTile(tilePos, tile);
                    Instantiate(TileHitbox, tilePosWorld, quaternion.identity);
                }
            }
        }
    }
}
