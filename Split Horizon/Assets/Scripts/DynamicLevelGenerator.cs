using UnityEngine;

public class DynamicLevelGenerator : MonoBehaviour
{
    public Transform player;
    public GameObject ground;

    [Header("Prefab Settings")]
    public GameObject cubePrefab;
    public GameObject cubeType2Prefab;
    public GameObject cubeType3Prefab;
    public GameObject cubeType4Prefab;
    public GameObject cubeType5Prefab;

    [Header("Rotation Offsets")]
    public Vector3 cubeType2Rotation;
    public Vector3 cubeType3Rotation;
    public Vector3 cubeType4Rotation;
    public Vector3 cubeType5Rotation;

    [Header("Custom Y Offsets")]
    public float cubeType4YOffset = 1f;
    public float cubeType5YOffset = 0.5f;

    [Header("CubeType1 Settings")]
    public int cubeCount = 20;
    public float spawnYOffset = 0.1f;
    public float spawnHeightRange = 7f;

    [Header("Other Cube Type Counts")]
    public int cubeType2Count = 10;
    public int cubeType3Count = 10;
    public int cubeType4Count = 10;
    public int cubeType5Count = 10;

    [Header("Rare Obstacle Settings")]
    public GameObject[] rareObstaclePrefabs;
    [Tooltip("Chance for a rare obstacle to spawn in each section (0 to 1)")]
    public float rareObstacleChance = 0.1f;

    [Header("Cube Material Settings")]
    public Material cubeMaterial;

    [Header("Generation Settings")]
    public float generationDistance = 500f; // Distance ahead of the player to generate sections.
    public float sectionLength = 50f;       // Length of each section along the Z-axis.
    public float generationXDistance = 500f;  // Maximum distance on the x-axis from the player.

    private float nextGenerationZ;
    private float groundXMin;
    private float groundXMax;
    private float groundZMin;
    private float groundZMax;
    private float groundTopY;

    void Start()
    {
        // Calculate ground boundaries.
        Vector3 groundPos = ground.transform.position;
        Vector3 groundScale = ground.transform.localScale;
        groundXMin = groundPos.x - groundScale.x / 2f;
        groundXMax = groundPos.x + groundScale.x / 2f;
        groundZMin = groundPos.z - groundScale.z / 2f;
        groundZMax = groundPos.z + groundScale.z / 2f;
        groundTopY = groundPos.y + groundScale.y / 2f;

        // Set starting point based on player's current position, clamped to ground boundaries.
        if (player != null)
            nextGenerationZ = Mathf.Clamp(player.position.z, groundZMin, groundZMax);

        // Pre-generate sections up to a limit (not exceeding groundZMax).
        float generationLimit = Mathf.Min(groundZMax, player.position.z + generationDistance);
        while (player != null && nextGenerationZ < generationLimit)
        {
            GenerateSection(nextGenerationZ, nextGenerationZ + sectionLength);
            nextGenerationZ += sectionLength;
        }
    }

    void Update()
    {
        // Check if player exists.
        if (player == null)
            return;

        // Only generate if we haven't reached the ground's max Z.
        if (player.position.z + generationDistance > nextGenerationZ && nextGenerationZ < groundZMax)
        {
            GenerateSection(nextGenerationZ, nextGenerationZ + sectionLength);
            nextGenerationZ += sectionLength;
        }
    }

    void GenerateSection(float sectionStartZ, float sectionEndZ)
    {
        // Clamp section boundaries.
        float clampedStart = Mathf.Clamp(sectionStartZ, groundZMin, groundZMax);
        float clampedEnd = Mathf.Clamp(sectionEndZ, groundZMin, groundZMax);

        // Generate a random Z coordinate within the section.
        float GetRandomZ() => Random.Range(clampedStart, clampedEnd);

        // Generate a random X coordinate based on the player's position.
        float GetRandomX()
        {
            float effectiveXMin = Mathf.Max(groundXMin, player.position.x - generationXDistance);
            float effectiveXMax = Mathf.Min(groundXMax, player.position.x + generationXDistance);
            return Random.Range(effectiveXMin, effectiveXMax);
        }

        // Get half-height of CubeType1.
        float cubeHeight = 1f;
        Collider cubeCollider = cubePrefab.GetComponent<Collider>();
        if (cubeCollider != null)
            cubeHeight = cubeCollider.bounds.size.y;
        float cubeHalfHeight = cubeHeight / 2f;

        // Spawn CubeType1: Floating cubes.
        for (int i = 0; i < cubeCount; i++)
        {
            float randomX = GetRandomX();
            float randomZ = GetRandomZ();
            float randomYOffset = Random.Range(0f, spawnHeightRange);
            Vector3 spawnPos = new Vector3(randomX, groundTopY + cubeHalfHeight + spawnYOffset + randomYOffset, randomZ);
            GameObject instance = Instantiate(cubePrefab, spawnPos, Quaternion.identity);
            ApplyCubeMaterial(instance);
        }

        // Spawn CubeType2.
        Quaternion cubeType2Quat = Quaternion.Euler(cubeType2Rotation);
        for (int i = 0; i < cubeType2Count; i++)
        {
            float randomX = GetRandomX();
            float randomZ = GetRandomZ();
            Vector3 spawnPos = new Vector3(randomX, groundTopY + cubeHalfHeight + spawnYOffset, randomZ);
            GameObject instance = Instantiate(cubeType2Prefab, spawnPos, cubeType2Quat);
            ApplyCubeMaterial(instance);
        }

        // Spawn CubeType3.
        Quaternion cubeType3Quat = Quaternion.Euler(cubeType3Rotation);
        for (int i = 0; i < cubeType3Count; i++)
        {
            float randomX = GetRandomX();
            float randomZ = GetRandomZ();
            Vector3 spawnPos = new Vector3(randomX, groundTopY + cubeHalfHeight + spawnYOffset, randomZ);
            GameObject instance = Instantiate(cubeType3Prefab, spawnPos, cubeType3Quat);
            ApplyCubeMaterial(instance);
        }

        // Spawn CubeType4.
        Quaternion cubeType4Quat = Quaternion.Euler(cubeType4Rotation) * cubeType4Prefab.transform.rotation;
        for (int i = 0; i < cubeType4Count; i++)
        {
            float randomX = GetRandomX();
            float randomZ = GetRandomZ();
            Vector3 spawnPos = new Vector3(randomX, groundTopY + cubeHalfHeight + spawnYOffset + cubeType4YOffset, randomZ);
            GameObject instance = Instantiate(cubeType4Prefab, spawnPos, cubeType4Quat);
            ApplyCubeMaterial(instance);
        }

        // Spawn CubeType5.
        Quaternion cubeType5Quat = Quaternion.Euler(cubeType5Rotation) * cubeType5Prefab.transform.rotation;
        for (int i = 0; i < cubeType5Count; i++)
        {
            float randomX = GetRandomX();
            float randomZ = GetRandomZ();
            Vector3 spawnPos = new Vector3(randomX, groundTopY + cubeHalfHeight + spawnYOffset + cubeType5YOffset, randomZ);
            GameObject instance = Instantiate(cubeType5Prefab, spawnPos, cubeType5Quat);
            ApplyCubeMaterial(instance);
        }

        // Occasionally spawn a rare obstacle.
        if (rareObstaclePrefabs != null && rareObstaclePrefabs.Length > 0 && Random.value < rareObstacleChance)
        {
            // Choose a random rare obstacle prefab.
            GameObject rarePrefab = rareObstaclePrefabs[Random.Range(0, rareObstaclePrefabs.Length)];
            float randomX = GetRandomX();
            float randomZ = GetRandomZ();
            float rareHalfHeight = 0f;
            Collider rareCollider = rarePrefab.GetComponent<Collider>();
            if (rareCollider != null)
                rareHalfHeight = rareCollider.bounds.size.y / 2f;
            // Add an extra upward offset (e.g., 0.1f) so the rare obstacle doesn't spawn in the ground.
            // float extraYOffset = 0.0f; add it back to bottom equation if needed.
            Vector3 spawnPos = new Vector3(randomX, groundTopY + rareHalfHeight, randomZ);
            GameObject instance = Instantiate(rarePrefab, spawnPos, rarePrefab.transform.rotation);
            ApplyCubeMaterial(instance);
        }
    }

    // Helper method to apply the specified material to an object's renderer(s).
    void ApplyCubeMaterial(GameObject obj)
    {
        if (cubeMaterial == null)
            return;

        Renderer rend = obj.GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material = cubeMaterial;
        }
        else
        {
            // If the object has children with renderers, apply to all.
            Renderer[] childRenderers = obj.GetComponentsInChildren<Renderer>();
            foreach (Renderer childRend in childRenderers)
            {
                childRend.material = cubeMaterial;
            }
        }
    }
}
