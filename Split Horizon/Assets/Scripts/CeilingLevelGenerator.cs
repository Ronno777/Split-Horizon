using UnityEngine;

public class CeilingLevelGenerator : MonoBehaviour
{
    public Transform player;
    public GameObject ceiling;

    [Header("Prefab Settings")]
    public GameObject cubePrefab;         // CubeType1: Floating cube (with random downward offset)
    public GameObject cubeType2Prefab;      // CubeType2 prefab
    public GameObject cubeType3Prefab;      // CubeType3 prefab
    public GameObject cubeType4Prefab;      // CubeType4 prefab (needs rotation fix & extra offset)
    public GameObject cubeType5Prefab;      // CubeType5 prefab (needs rotation fix & extra offset)

    [Header("Rotation Offsets (Euler Angles) for CubeTypes 2-5")]
    public Vector3 cubeType2Rotation;
    public Vector3 cubeType3Rotation;
    public Vector3 cubeType4Rotation;
    public Vector3 cubeType5Rotation;

    [Header("Custom Y Offsets for Ceiling CubeTypes")]
    public float cubeType4YOffset = 1f;     // Extra downward offset for CubeType4
    public float cubeType5YOffset = 0.5f;     // Extra downward offset for CubeType5

    [Header("Ceiling Settings")]
    public int cubeCount = 20;            // Number of CubeType1 cubes (floating)
    public float spawnYOffset = 0.1f;       // Base offset from the ceiling to avoid clipping
    public float spawnHeightRange = 7f;     // Maximum additional downward offset for CubeType1

    [Header("Other Cube Type Counts (Flush with the Ceiling)")]
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
    public float generationDistance = 500f; // Distance ahead of the player to generate sections
    public float sectionLength = 50f;       // Length of each section along the Z-axis
    public float generationXDistance = 500f;  // Maximum distance on the x-axis from the player

    private float nextGenerationZ;
    private float xMin;
    private float xMax;
    private float zMin;
    private float zMax;
    private float ceilingBottomY;
    private float cubeHalfHeight;

    void Start()
    {
        // Calculate ceiling boundaries.
        Vector3 ceilPos = ceiling.transform.position;
        Vector3 ceilScale = ceiling.transform.localScale;
        xMin = ceilPos.x - ceilScale.x / 2f;
        xMax = ceilPos.x + ceilScale.x / 2f;
        zMin = ceilPos.z - ceilScale.z / 2f;
        zMax = ceilPos.z + ceilScale.z / 2f;

        // Calculate the bottom of the ceiling.
        ceilingBottomY = ceilPos.y - ceilScale.y / 2f;

        // Get CubeType1's half-height using its collider.
        float cubeHeight = 1f;
        Collider cubeCol = cubePrefab.GetComponent<Collider>();
        if (cubeCol != null)
            cubeHeight = cubeCol.bounds.size.y;
        cubeHalfHeight = cubeHeight / 2f;

        // Set starting generation point based on the player's z position, clamped to ceiling boundaries.
        if (player != null)
            nextGenerationZ = Mathf.Clamp(player.position.z, zMin, zMax);

        // Pre-generate sections up to a limit (not exceeding ceiling's zMax).
        float generationLimit = Mathf.Min(zMax, player.position.z + generationDistance);
        while (player != null && nextGenerationZ < generationLimit)
        {
            GenerateSection(nextGenerationZ, nextGenerationZ + sectionLength);
            nextGenerationZ += sectionLength;
        }
    }

    void Update()
    {
        if (player == null)
            return;

        // Dynamically generate a new section if the player is close enough and within ceiling boundaries.
        if (player.position.z + generationDistance > nextGenerationZ && nextGenerationZ < zMax)
        {
            GenerateSection(nextGenerationZ, nextGenerationZ + sectionLength);
            nextGenerationZ += sectionLength;
        }
    }

    void GenerateSection(float sectionStartZ, float sectionEndZ)
    {
        // Clamp section boundaries to the ceiling's z range.
        float clampedStart = Mathf.Clamp(sectionStartZ, zMin, zMax);
        float clampedEnd = Mathf.Clamp(sectionEndZ, zMin, zMax);

        // Local functions to generate random positions.
        float GetRandomZ() => Random.Range(clampedStart, clampedEnd);

        float GetRandomX()
        {
            float effectiveXMin = Mathf.Max(xMin, player.position.x - generationXDistance);
            float effectiveXMax = Mathf.Min(xMax, player.position.x + generationXDistance);
            return Random.Range(effectiveXMin, effectiveXMax);
        }

        // Calculate flush spawn Y: cube's top touches the ceiling's bottom.
        float flushSpawnY = ceilingBottomY + cubeHalfHeight - spawnYOffset;

        // --- Spawn CubeType1 (Floating Cubes) ---
        for (int i = 0; i < cubeCount; i++)
        {
            float randomX = GetRandomX();
            float randomZ = GetRandomZ();
            float randomYOffset = Random.Range(0f, spawnHeightRange);
            Vector3 spawnPos = new Vector3(randomX, ceilingBottomY + cubeHalfHeight - spawnYOffset - randomYOffset, randomZ);
            GameObject instance = Instantiate(cubePrefab, spawnPos, Quaternion.identity);
            ApplyCubeMaterial(instance);
        }

        // Prepare rotation quaternions for flush-spawned cube types.
        Quaternion cubeType2Quat = Quaternion.Euler(cubeType2Rotation);
        Quaternion cubeType3Quat = Quaternion.Euler(cubeType3Rotation);
        Quaternion cubeType4Quat = Quaternion.Euler(cubeType4Rotation) * cubeType4Prefab.transform.rotation;
        Quaternion cubeType5Quat = Quaternion.Euler(cubeType5Rotation) * cubeType5Prefab.transform.rotation;

        // --- Spawn CubeType2 (Flush) ---
        for (int i = 0; i < cubeType2Count; i++)
        {
            float randomX = GetRandomX();
            float randomZ = GetRandomZ();
            Vector3 spawnPos = new Vector3(randomX, flushSpawnY, randomZ);
            GameObject instance = Instantiate(cubeType2Prefab, spawnPos, cubeType2Quat);
            ApplyCubeMaterial(instance);
        }

        // --- Spawn CubeType3 (Flush) ---
        for (int i = 0; i < cubeType3Count; i++)
        {
            float randomX = GetRandomX();
            float randomZ = GetRandomZ();
            Vector3 spawnPos = new Vector3(randomX, flushSpawnY, randomZ);
            GameObject instance = Instantiate(cubeType3Prefab, spawnPos, cubeType3Quat);
            ApplyCubeMaterial(instance);
        }

        // --- Spawn CubeType4 (Flush with extra downward offset) ---
        for (int i = 0; i < cubeType4Count; i++)
        {
            float randomX = GetRandomX();
            float randomZ = GetRandomZ();
            Vector3 spawnPos = new Vector3(randomX, flushSpawnY - cubeType4YOffset, randomZ);
            GameObject instance = Instantiate(cubeType4Prefab, spawnPos, cubeType4Quat);
            ApplyCubeMaterial(instance);
        }

        // --- Spawn CubeType5 (Flush with extra downward offset) ---
        for (int i = 0; i < cubeType5Count; i++)
        {
            float randomX = GetRandomX();
            float randomZ = GetRandomZ();
            Vector3 spawnPos = new Vector3(randomX, flushSpawnY - cubeType5YOffset, randomZ);
            GameObject instance = Instantiate(cubeType5Prefab, spawnPos, cubeType5Quat);
            ApplyCubeMaterial(instance);
        }

        // --- Occasionally spawn a rare obstacle on the ceiling ---
        if (rareObstaclePrefabs != null && rareObstaclePrefabs.Length > 0 && Random.value < rareObstacleChance)
        {
            GameObject rarePrefab = rareObstaclePrefabs[Random.Range(0, rareObstaclePrefabs.Length)];
            float randomX = GetRandomX();
            float randomZ = GetRandomZ();
            float rareHalfHeight = 0f;
            Collider rareCollider = rarePrefab.GetComponent<Collider>();
            if (rareCollider != null)
                rareHalfHeight = rareCollider.bounds.size.y / 2f;
            // Spawn the rare obstacle flush with the ceiling.
            Vector3 spawnPos = new Vector3(randomX, ceilingBottomY + rareHalfHeight, randomZ);
            GameObject instance = Instantiate(rarePrefab, spawnPos, Quaternion.identity);
            ApplyCubeMaterial(instance);
        }
    }

    // Helper method to apply the assigned cube material.
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
            Renderer[] childRenderers = obj.GetComponentsInChildren<Renderer>();
            foreach (Renderer child in childRenderers)
            {
                child.material = cubeMaterial;
            }
        }
    }
}
