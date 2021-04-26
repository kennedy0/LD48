using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GenerateTiles : MonoBehaviour
{
    [Header("Tiles")]
    public Tile BlueTile;
    public Tile RedTile;
    public Tile BlackTile;
    public Tile VeryBlackTile;
    
    [Header("Hitboxes")]
    public GameObject TileHitbox_Red;
    public GameObject TileHitbox_Blue;
    public GameObject TileHitbox_Black;
    public GameObject TileHitbox_Invincible;

    [Header("Generation")]
    public int MaxWallThickness = 5;
    public float BlackTilePercentage = .33f;

    [Header("Layout")]
    public float TileSize = 16f;
    public float XOffset = 3.5f;
    public int ChunkWidth = 20;
    public int ChunkHeight = 10;
    
    private Tilemap tilemap;
    private GameObject player;
    public float lastChunkY;

    private float pixelScale;
    private float tileSize;
    private float halfTileSize;
    private float chunkWidthWorldUnits;
    private float chunkHeightWorldUnits;
    private Transform cam;

    private void Start()
    {
        cam = Camera.main.GetComponent<Transform>();
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
        // Build new chunks slightly ahead of the player
        float playerY = player.transform.position.y;
        if (playerY < lastChunkY)
        {
            GenerateChunks(playerY - chunkHeightWorldUnits);
        }
    }

    private void GenerateChunks(float y)
    {
        GenerateChunk(-XOffset, y);
        GenerateChunk(XOffset, y);
    }

    private void GenerateChunk(float x, float y)
    {
        lastChunkY = y;
        int score = Mathf.FloorToInt(Mathf.Abs(cam.transform.position.y) * 5);
        int depth = Mathf.FloorToInt(score / 100);
        int wallThickness = MaxWallThickness + depth;
        
        for (int j = 0; j < ChunkHeight; j++)
        {
            for (int i = 0; i < ChunkWidth; i++)
            {
                // Set color
                Tile colorTile;
                GameObject colorHitbox;
                if (x < 0f)
                {
                    colorTile = BlueTile;
                    colorHitbox = TileHitbox_Blue;
                }
                else
                {
                    colorTile = RedTile;
                    colorHitbox = TileHitbox_Red;
                }

                // Calculate noise
                float leftSeed = 218594f;
                float rightSeed = 11502f;
                float noiseLeft = Mathf.Clamp01(Mathf.PerlinNoise(x + leftSeed, j));
                float noiseRight = Mathf.Clamp01(Mathf.PerlinNoise(x + rightSeed, j));
                int wallThicknessLeft = Mathf.FloorToInt(wallThickness * noiseLeft);
                int wallThicknessRight = Mathf.FloorToInt(wallThickness * noiseRight);
                
                float leftSeedBlack = 8675;
                float rightSeedBlack = 309999;
                float noiseLeftBlack = Mathf.Clamp01(Mathf.PerlinNoise(x + leftSeedBlack, j));
                float noiseRightBlack = Mathf.Clamp01(Mathf.PerlinNoise(x + rightSeedBlack, j));
                int wallThicknessLeftBlack = Mathf.FloorToInt(wallThickness * noiseLeftBlack * BlackTilePercentage);
                int wallThicknessRightBlack = Mathf.FloorToInt(wallThickness * noiseRightBlack * BlackTilePercentage);
                
                // Get world position of tile
                float yPos = y - halfTileSize - (tileSize * j);
                float xPos = x - (chunkWidthWorldUnits / 2f) + halfTileSize + (tileSize * i);
                Vector3 tilePosWorld = new Vector3(xPos, yPos, 0f);

                if (i == 0 && x < 0f)
                {
                    // Left Indestructable Boundary
                    SetTile(tilePosWorld, VeryBlackTile, TileHitbox_Invincible);
                }
                else if (i == ChunkWidth - 1 && x > 0f)
                {
                    // Right Indestructable Boundary
                    SetTile(tilePosWorld, VeryBlackTile, TileHitbox_Invincible);
                }
                else if (i <= wallThicknessLeftBlack)
                {
                    // Left black tiles
                    SetTile(tilePosWorld, BlackTile, TileHitbox_Black);
                }
                else if (i <= wallThicknessLeft)
                {
                    // Left colored tiles
                    SetTile(tilePosWorld, colorTile, colorHitbox);
                }
                else if (i >= ChunkWidth - wallThicknessRightBlack)
                {
                    // Right black tiles
                    SetTile(tilePosWorld, BlackTile, TileHitbox_Black);
                }
                else if (i >= ChunkWidth - wallThicknessRight)
                {
                    // Right colored tiles
                    SetTile(tilePosWorld, colorTile, colorHitbox);
                }
            }
        }
    }

    private void SetTile(Vector3 tilePosWorld, Tile tile, GameObject hitbox)
    {
        Vector3Int tilePos = tilemap.WorldToCell(tilePosWorld);
        tilemap.SetTile(tilePos, tile);
        Instantiate(hitbox, tilePosWorld, quaternion.identity);
    }
}
