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
        // Clamp section boundaries to ensure the section lies within the ground.
        float clampedStart = Mathf.Clamp(sectionStartZ, groundZMin, groundZMax);
        float clampedEnd = Mathf.Clamp(sectionEndZ, groundZMin, groundZMax);

        // Generate a random Z coordinate uniformly within the clamped section.
        float GetRandomZ()
        {
            return Random.Range(clampedStart, clampedEnd);
        }

        // Helper to generate a random X coordinate based on player's current position.
        float GetRandomX()
        {
            float effectiveXMin = Mathf.Max(groundXMin, player.position.x - generationXDistance);
            float effectiveXMax = Mathf.Min(groundXMax, player.position.x + generationXDistance);
            return Random.Range(effectiveXMin, effectiveXMax);
        }

        // Get half-height of CubeType1 using its collider.
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
            Instantiate(cubePrefab, spawnPos, Quaternion.identity);
        }

        // Spawn CubeType2.
        Quaternion cubeType2Quat = Quaternion.Euler(cubeType2Rotation);
        for (int i = 0; i < cubeType2Count; i++)
        {
            float randomX = GetRandomX();
            float randomZ = GetRandomZ();
            Vector3 spawnPos = new Vector3(randomX, groundTopY + cubeHalfHeight + spawnYOffset, randomZ);
            Instantiate(cubeType2Prefab, spawnPos, cubeType2Quat);
        }

        // Spawn CubeType3.
        Quaternion cubeType3Quat = Quaternion.Euler(cubeType3Rotation);
        for (int i = 0; i < cubeType3Count; i++)
        {
            float randomX = GetRandomX();
            float randomZ = GetRandomZ();
            Vector3 spawnPos = new Vector3(randomX, groundTopY + cubeHalfHeight + spawnYOffset, randomZ);
            Instantiate(cubeType3Prefab, spawnPos, cubeType3Quat);
        }

        // Spawn CubeType4.
        Quaternion cubeType4Quat = Quaternion.Euler(cubeType4Rotation) * cubeType4Prefab.transform.rotation;
        for (int i = 0; i < cubeType4Count; i++)
        {
            float randomX = GetRandomX();
            float randomZ = GetRandomZ();
            Vector3 spawnPos = new Vector3(randomX, groundTopY + cubeHalfHeight + spawnYOffset + cubeType4YOffset, randomZ);
            Instantiate(cubeType4Prefab, spawnPos, cubeType4Quat);
        }

        // Spawn CubeType5.
        Quaternion cubeType5Quat = Quaternion.Euler(cubeType5Rotation) * cubeType5Prefab.transform.rotation;
        for (int i = 0; i < cubeType5Count; i++)
        {
            float randomX = GetRandomX();
            float randomZ = GetRandomZ();
            Vector3 spawnPos = new Vector3(randomX, groundTopY + cubeHalfHeight + spawnYOffset + cubeType5YOffset, randomZ);
            Instantiate(cubeType5Prefab, spawnPos, cubeType5Quat);
        }
    }
}
