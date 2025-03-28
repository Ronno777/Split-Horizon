using UnityEngine;

public class MirroredLevelGenerator : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public GameObject ground;   // Reference to the ground GameObject.
    public GameObject ceiling;  // Reference to the ceiling GameObject.

    [Header("Prefab Settings")]
    public GameObject cubePrefab;         // CubeType1 prefab.
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
    public float spawnYOffset = 0.01f;
    public float spawnHeightRange = 7f;

    [Header("Other Cube Type Counts")]
    public int cubeType2Count = 10;
    public int cubeType3Count = 10;
    public int cubeType4Count = 10;
    public int cubeType5Count = 10;

    [Header("Rare Obstacle Settings")]
    public GameObject[] rareObstaclePrefabs;
    [Tooltip("Chance for a rare obstacle pair to spawn in each section (0 to 1)")]
    public float rareObstacleChance = 0.1f;

    [Header("Cube Material Settings")]
    public Material groundCubeMaterial;  // Material for ground obstacles.
    public Material ceilingCubeMaterial; // Material for ceiling obstacles.

    [Header("Generation Settings")]
    public float generationDistance = 500f; // Distance ahead of the player to generate sections.
    public float sectionLength = 50f;       // Length of each section along the Z-axis.
    public float generationXDistance = 500f;  // Maximum distance on the x-axis from the player.

    // Ground boundaries.
    private float groundXMin, groundXMax, groundZMin, groundZMax, groundTopY;
    // Ceiling boundaries.
    private float ceilingXMin, ceilingXMax, ceilingZMin, ceilingZMax, ceilingBottomY;
    // Common next generation Z value.
    private float nextGenerationZ;
    // Common cube half-height (from cubePrefab collider).
    private float cubeHalfHeight;
    // Downward adjustment for ceiling cubes.
    private const float ceilingAdjustment = 6.8f;

    void Start()
    {
        if (player == null || ground == null || ceiling == null)
        {
            Debug.LogError("MirroredLevelGenerator: Missing references for player, ground, or ceiling.");
            enabled = false;
            return;
        }

        // Calculate ground boundaries.
        Vector3 groundPos = ground.transform.position;
        Vector3 groundScale = ground.transform.localScale;
        groundXMin = groundPos.x - groundScale.x / 2f;
        groundXMax = groundPos.x + groundScale.x / 2f;
        groundZMin = groundPos.z - groundScale.z / 2f;
        groundZMax = groundPos.z + groundScale.z / 2f;
        groundTopY = groundPos.y + groundScale.y / 2f;

        // Calculate ceiling boundaries.
        Vector3 ceilingPos = ceiling.transform.position;
        Vector3 ceilingScale = ceiling.transform.localScale;
        ceilingXMin = ceilingPos.x - ceilingScale.x / 2f;
        ceilingXMax = ceilingPos.x + ceilingScale.x / 2f;
        ceilingZMin = ceilingPos.z - ceilingScale.z / 2f;
        ceilingZMax = ceilingPos.z + ceilingScale.z / 2f;
        ceilingBottomY = ceilingPos.y - ceilingScale.y / 2f;

        // Determine cube half-height from cubePrefab.
        cubeHalfHeight = 1f;
        Collider cubeCol = cubePrefab.GetComponent<Collider>();
        if (cubeCol != null)
            cubeHalfHeight = cubeCol.bounds.size.y / 2f;

        // Set starting generation Z based on player's current position.
        nextGenerationZ = Mathf.Clamp(player.position.z, groundZMin, groundZMax);

        // Pre-generate sections.
        float generationLimit = Mathf.Min(groundZMax, player.position.z + generationDistance);
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

        if (player.position.z + generationDistance > nextGenerationZ && nextGenerationZ < groundZMax)
        {
            GenerateSection(nextGenerationZ, nextGenerationZ + sectionLength);
            nextGenerationZ += sectionLength;
        }
    }

    // Generates a section and spawns obstacles in mirrored pairs (ground and ceiling).
    void GenerateSection(float sectionStartZ, float sectionEndZ)
    {
        // Clamp section boundaries.
        float clampedStart = Mathf.Clamp(sectionStartZ, groundZMin, groundZMax);
        float clampedEnd = Mathf.Clamp(sectionEndZ, groundZMin, groundZMax);

        // Local functions for random positions.
        float GetRandomZ() => Random.Range(clampedStart, clampedEnd);
        float GetRandomX()
        {
            float effectiveXMin = Mathf.Max(groundXMin, ceilingXMin, player.position.x - generationXDistance);
            float effectiveXMax = Mathf.Min(groundXMax, ceilingXMax, player.position.x + generationXDistance);
            return Random.Range(effectiveXMin, effectiveXMax);
        }

        // --- Floating Cubes (CubeType1) ---
        for (int i = 0; i < cubeCount; i++)
        {
            float randomX = GetRandomX();
            float randomZ = GetRandomZ();
            float randomYOffset = Random.Range(0f, spawnHeightRange);
            // Ground spawn: no change.
            Vector3 groundSpawnPos = new Vector3(
                randomX,
                groundTopY + cubeHalfHeight + spawnYOffset + randomYOffset,
                randomZ);
            // Ceiling spawn: use old logic then apply a 7m downward adjustment.
            Vector3 ceilingSpawnPos = new Vector3(
                randomX,
                (ceilingBottomY + cubeHalfHeight - spawnYOffset - randomYOffset) - ceilingAdjustment,
                randomZ);

            GameObject groundInstance = Instantiate(cubePrefab, groundSpawnPos, cubePrefab.transform.rotation);
            GameObject ceilingInstance = Instantiate(cubePrefab, ceilingSpawnPos, cubePrefab.transform.rotation);
            ApplyCubeMaterial(groundInstance, groundCubeMaterial);
            ApplyCubeMaterial(ceilingInstance, ceilingCubeMaterial);
        }

        // Prepare rotation quaternions for flush obstacles.
        Quaternion cubeType2Quat = Quaternion.Euler(cubeType2Rotation);
        Quaternion cubeType3Quat = Quaternion.Euler(cubeType3Rotation);
        Quaternion cubeType4Quat = Quaternion.Euler(cubeType4Rotation) * cubeType4Prefab.transform.rotation;
        Quaternion cubeType5Quat = Quaternion.Euler(cubeType5Rotation) * cubeType5Prefab.transform.rotation;

        // Flush spawn Y positions:
        float groundFlushY = groundTopY + cubeHalfHeight + spawnYOffset;
        float ceilingFlushY = (ceilingBottomY + cubeHalfHeight - spawnYOffset) - ceilingAdjustment;

        // --- CubeType2 (Flush) ---
        for (int i = 0; i < cubeType2Count; i++)
        {
            float randomX = GetRandomX();
            float randomZ = GetRandomZ();
            Vector3 groundSpawnPos = new Vector3(randomX, groundFlushY, randomZ);
            Vector3 ceilingSpawnPos = new Vector3(randomX, ceilingFlushY, randomZ);
            GameObject gi = Instantiate(cubeType2Prefab, groundSpawnPos, cubeType2Quat);
            GameObject ci = Instantiate(cubeType2Prefab, ceilingSpawnPos, cubeType2Quat);
            ApplyCubeMaterial(gi, groundCubeMaterial);
            ApplyCubeMaterial(ci, ceilingCubeMaterial);
        }

        // --- CubeType3 (Flush) ---
        for (int i = 0; i < cubeType3Count; i++)
        {
            float randomX = GetRandomX();
            float randomZ = GetRandomZ();
            Vector3 groundSpawnPos = new Vector3(randomX, groundFlushY, randomZ);
            Vector3 ceilingSpawnPos = new Vector3(randomX, ceilingFlushY, randomZ);
            GameObject gi = Instantiate(cubeType3Prefab, groundSpawnPos, cubeType3Quat);
            GameObject ci = Instantiate(cubeType3Prefab, ceilingSpawnPos, cubeType3Quat);
            ApplyCubeMaterial(gi, groundCubeMaterial);
            ApplyCubeMaterial(ci, ceilingCubeMaterial);
        }

        // --- CubeType4 (Flush with extra offset) ---
        for (int i = 0; i < cubeType4Count; i++)
        {
            float randomX = GetRandomX();
            float randomZ = GetRandomZ();
            Vector3 groundSpawnPos = new Vector3(randomX, groundFlushY + cubeType4YOffset, randomZ);
            Vector3 ceilingSpawnPos = new Vector3(randomX, ceilingFlushY - cubeType4YOffset, randomZ);
            GameObject gi = Instantiate(cubeType4Prefab, groundSpawnPos, cubeType4Quat);
            GameObject ci = Instantiate(cubeType4Prefab, ceilingSpawnPos, cubeType4Quat);
            ApplyCubeMaterial(gi, groundCubeMaterial);
            ApplyCubeMaterial(ci, ceilingCubeMaterial);
        }

        // --- CubeType5 (Flush with extra offset) ---
        for (int i = 0; i < cubeType5Count; i++)
        {
            float randomX = GetRandomX();
            float randomZ = GetRandomZ();
            Vector3 groundSpawnPos = new Vector3(randomX, groundFlushY + cubeType5YOffset, randomZ);
            Vector3 ceilingSpawnPos = new Vector3(randomX, ceilingFlushY - cubeType5YOffset, randomZ);
            GameObject gi = Instantiate(cubeType5Prefab, groundSpawnPos, cubeType5Quat);
            GameObject ci = Instantiate(cubeType5Prefab, ceilingSpawnPos, cubeType5Quat);
            ApplyCubeMaterial(gi, groundCubeMaterial);
            ApplyCubeMaterial(ci, ceilingCubeMaterial);
        }

        // --- Rare Obstacles (spawn as a pair) ---
        if (rareObstaclePrefabs != null && rareObstaclePrefabs.Length > 0 && Random.value < rareObstacleChance)
        {
            GameObject rarePrefab = rareObstaclePrefabs[Random.Range(0, rareObstaclePrefabs.Length)];
            float randomX = GetRandomX();
            float randomZ = GetRandomZ();
            float rareHalfHeight = 0f;
            Collider rareCollider = rarePrefab.GetComponent<Collider>();
            if (rareCollider != null)
                rareHalfHeight = rareCollider.bounds.size.y / 2f;
            float extraYOffset = 0.1f;
            Vector3 groundRarePos = new Vector3(randomX, groundTopY + rareHalfHeight + extraYOffset, randomZ);
            Vector3 ceilingRarePos = new Vector3(randomX, (ceilingBottomY + rareHalfHeight) - ceilingAdjustment, randomZ);
            GameObject gi = Instantiate(rarePrefab, groundRarePos, rarePrefab.transform.rotation);
            GameObject ci = Instantiate(rarePrefab, ceilingRarePos, rarePrefab.transform.rotation);
            ApplyCubeMaterial(gi, groundCubeMaterial);
            ApplyCubeMaterial(ci, ceilingCubeMaterial);
        }
    }

    // Helper method to apply a given material to an object.
    void ApplyCubeMaterial(GameObject obj, Material mat)
    {
        if (mat == null)
            return;

        Renderer rend = obj.GetComponent<Renderer>();
        if (rend != null)
            rend.material = mat;
        else
        {
            Renderer[] childRenderers = obj.GetComponentsInChildren<Renderer>();
            foreach (Renderer child in childRenderers)
                child.material = mat;
        }
    }
}
