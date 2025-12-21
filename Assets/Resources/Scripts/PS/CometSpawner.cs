using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CometSpawner : MonoBehaviour
{
    [Header("Settings")]
    public GameObject cometPrefab;
    public int maxComets = 3;
    
    [Header("Timing")]
    public float minSpawnDelay = 1.0f;
    public float maxSpawnDelay = 4.0f;

    [Header("Spawn Area")]
    [Tooltip("How far down the side edges can they spawn? (0 = Top only, 0.5 = Down to middle)")]
    public float sideSpawnHeight = 0.5f; 
    [Tooltip("Offset outside the screen so they don't pop in visibly")]
    public float spawnBuffer = 1.0f;

    // Static list to keep track of active comets globally
    public static List<GameObject> activeComets = new List<GameObject>();

    private float nextSpawnTime;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        ScheduleNextSpawn();
    }

    void Update()
    {
        // Cleanup list to remove nulls
        activeComets.RemoveAll(item => item == null);

        if (Time.time >= nextSpawnTime && activeComets.Count < maxComets)
        {
            SpawnComet();
            ScheduleNextSpawn();
        }
    }

    void ScheduleNextSpawn()
    {
        nextSpawnTime = Time.time + Random.Range(minSpawnDelay, maxSpawnDelay);
    }

    void SpawnComet()
    {
        Vector3 spawnPos = Vector3.zero;
        Vector3 targetPos = Vector3.zero;

        // 1. Get Screen Bounds in World Space
        float vertExtent = mainCamera.orthographicSize;
        float horzExtent = vertExtent * mainCamera.aspect;
        
        // Calculate edges
        float topEdge = mainCamera.transform.position.y + vertExtent;
        float bottomEdge = mainCamera.transform.position.y - vertExtent;
        float leftEdge = mainCamera.transform.position.x - horzExtent;
        float rightEdge = mainCamera.transform.position.x + horzExtent;

        // 2. Decide which edge to spawn on (Weighted random)
        // We consider the Top Edge and the Upper halves of Left/Right edges
        float topLength = (rightEdge - leftEdge);
        float sideLength = (vertExtent * 2 * sideSpawnHeight);
        
        // Total "perimeter" length we are spawning on
        float totalLength = topLength + (sideLength * 2); 
        float randomPick = Random.Range(0, totalLength);

        // 3. Determine Position based on random pick
        if (randomPick < topLength) 
        {
            // Spawn on TOP Edge
            spawnPos = new Vector3(
                Random.Range(leftEdge, rightEdge), 
                topEdge + spawnBuffer, 
                0
            );
        }
        else if (randomPick < topLength + sideLength)
        {
            // Spawn on LEFT Edge (Upper half)
            float randomY = Random.Range(topEdge, topEdge - (vertExtent * 2 * sideSpawnHeight));
            spawnPos = new Vector3(leftEdge - spawnBuffer, randomY, 0);
        }
        else
        {
            // Spawn on RIGHT Edge (Upper half)
            float randomY = Random.Range(topEdge, topEdge - (vertExtent * 2 * sideSpawnHeight));
            spawnPos = new Vector3(rightEdge + spawnBuffer, randomY, 0);
        }

        // 4. Calculate a "Target" so it flows diagonally into the screen
        // We pick a random point on the BOTTOM edge to aim at.
        // This guarantees a nice diagonal flow across the screen.
        float targetX = Random.Range(leftEdge, rightEdge);
        targetPos = new Vector3(targetX, bottomEdge - 2.0f, 0); // Aim below screen

        // Instantiate
        GameObject newComet = Instantiate(cometPrefab, spawnPos, Quaternion.identity);
        
        // Initialize the movement script with our calculated path
        CometMovement mover = newComet.GetComponent<CometMovement>();
        if (mover != null)
        {
            mover.Initialize(spawnPos, targetPos);
        }

        activeComets.Add(newComet);
    }
}