using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject cubePrefab;         // Your Cube prefab.
    public GameObject ground;             // Your Ground object.
    public int cubeCount = 20;            // Number of cubes to spawn.
    public float spawnYOffset = 0.1f;       // Base offset above the platform.
    public float spawnHeightRange = 7f;     // Additional height range above the platform.
    public Transform playerTransform;     // Assign the player's transform.
    public float spawnZMinOffset = 10f;     // Minimum distance in z from the player's spawn.

    void Start()
    {
        // Calculate ground boundaries.
        Vector3 groundPos = ground.transform.position;
        Vector3 groundScale = ground.transform.localScale;
        float groundXMin = groundPos.x - groundScale.x / 2f;
        float groundXMax = groundPos.x + groundScale.x / 2f;
        float groundZMin = groundPos.z - groundScale.z / 2f;
        float groundZMax = groundPos.z + groundScale.z / 2f;

        // Determine the top y-coordinate of the ground.
        float groundTopY = groundPos.y + groundScale.y / 2f;

        // Get the cube's actual height using its collider bounds.
        float cubeHeight = 1f;
        Collider cubeCollider = cubePrefab.GetComponent<Collider>();
        if (cubeCollider != null)
        {
            cubeHeight = cubeCollider.bounds.size.y;
        }
        float cubeHalfHeight = cubeHeight / 2f;

        // Set the minimum z for spawning based on the player's position.
        float minSpawnZ = playerTransform.position.z + spawnZMinOffset;
        if (minSpawnZ < groundZMin)
            minSpawnZ = groundZMin;

        // Spawn cubes at random positions above the platform.
        for (int i = 0; i < cubeCount; i++)
        {
            float randomX = Random.Range(groundXMin, groundXMax);
            float randomZ = Random.Range(minSpawnZ, groundZMax);
            // Random vertical offset within the specified range.
            float randomYOffset = Random.Range(0f, spawnHeightRange);
            Vector3 spawnPos = new Vector3(randomX, groundTopY + cubeHalfHeight + spawnYOffset + randomYOffset, randomZ);
            Instantiate(cubePrefab, spawnPos, Quaternion.identity);
        }
    }
}
