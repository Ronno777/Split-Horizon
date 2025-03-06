using UnityEngine;

public class CeilingLevelGenerator : MonoBehaviour
{
    [Header("Prefab Settings")]
    public GameObject cubePrefab;         // CubeType1: Normal cube (floating with random downward offset)
    public GameObject cubeType2Prefab;      // CubeType2 prefab
    public GameObject cubeType3Prefab;      // CubeType3 prefab
    public GameObject cubeType4Prefab;      // CubeType4 prefab (needs rotation fix & extra Y offset)
    public GameObject cubeType5Prefab;      // CubeType5 prefab (needs rotation fix & extra Y offset)

    [Header("Rotation Offsets (Euler Angles) for CubeTypes 2-5")]
    public Vector3 cubeType2Rotation;
    public Vector3 cubeType3Rotation;
    public Vector3 cubeType4Rotation;
    public Vector3 cubeType5Rotation;

    [Header("Custom Y Offsets for Ceiling CubeTypes")]
    public float cubeType4YOffset = 1f;     // Extra downward offset for CubeType4
    public float cubeType5YOffset = 0.5f;     // Extra downward offset for CubeType5

    [Header("Ceiling Settings")]
    public GameObject ceiling;            // The ceiling platform (e.g., at (0,15,40)).
    public int cubeCount = 20;            // Number of CubeType1 cubes (floating).
    public float spawnYOffset = 0.1f;       // Base offset from the ceiling (used to avoid clipping).
    public float spawnHeightRange = 7f;     // Maximum additional downward offset for CubeType1.

    [Header("Other Cube Type Counts (Flush with the Ceiling)")]
    public int cubeType2Count = 10;
    public int cubeType3Count = 10;
    public int cubeType4Count = 10;
    public int cubeType5Count = 10;

    void Start()
    {
        // Calculate ceiling boundaries.
        Vector3 ceilPos = ceiling.transform.position;
        Vector3 ceilScale = ceiling.transform.localScale;
        float xMin = ceilPos.x - ceilScale.x / 2f;
        float xMax = ceilPos.x + ceilScale.x / 2f;
        float zMin = ceilPos.z - ceilScale.z / 2f;
        float zMax = ceilPos.z + ceilScale.z / 2f;

        // Calculate the bottom of the ceiling.
        float ceilingBottomY = ceilPos.y - ceilScale.y / 2f;

        // Get CubeType1's actual height using its collider.
        float cubeHeight = 1f;
        Collider cubeCol = cubePrefab.GetComponent<Collider>();
        if (cubeCol != null)
            cubeHeight = cubeCol.bounds.size.y;
        float cubeHalfHeight = cubeHeight / 2f;

        // --- Spawn CubeType1 (Floating Cubes) ---
        // For the ceiling, the cube's top aligns with the ceiling's bottom.
        // Then subtract spawnYOffset and a random additional offset to "float" downward.
        for (int i = 0; i < cubeCount; i++)
        {
            float randomX = Random.Range(xMin, xMax);
            float randomZ = Random.Range(zMin, zMax);
            float randomYOffset = Random.Range(0f, spawnHeightRange);
            Vector3 spawnPos = new Vector3(
                randomX,
                ceilingBottomY + cubeHalfHeight - spawnYOffset - randomYOffset,
                randomZ
            );
            GameObject instance = Instantiate(cubePrefab, spawnPos, Quaternion.identity);
            // Reset material to default white.
            Renderer rend = instance.GetComponent<Renderer>();
            if (rend != null)
                rend.material = new Material(Shader.Find("Standard"));
        }

        // Prepare rotation quaternions for flush-spawned cube types.
        Quaternion cubeType2Quat = Quaternion.Euler(cubeType2Rotation);
        Quaternion cubeType3Quat = Quaternion.Euler(cubeType3Rotation);
        Quaternion cubeType4Quat = Quaternion.Euler(cubeType4Rotation) * cubeType4Prefab.transform.rotation;
        Quaternion cubeType5Quat = Quaternion.Euler(cubeType5Rotation) * cubeType5Prefab.transform.rotation;

        // Base spawn Y for flush cubes: cube's top touches the ceiling's bottom.
        float flushSpawnY = ceilingBottomY + cubeHalfHeight - spawnYOffset;

        // --- Spawn CubeType2 (Flush) ---
        for (int i = 0; i < cubeType2Count; i++)
        {
            float randomX = Random.Range(xMin, xMax);
            float randomZ = Random.Range(zMin, zMax);
            Vector3 spawnPos = new Vector3(randomX, flushSpawnY, randomZ);
            GameObject instance = Instantiate(cubeType2Prefab, spawnPos, cubeType2Quat);
            Renderer rend = instance.GetComponent<Renderer>();
            if (rend != null)
                rend.material = new Material(Shader.Find("Standard"));
        }

        // --- Spawn CubeType3 (Flush) ---
        for (int i = 0; i < cubeType3Count; i++)
        {
            float randomX = Random.Range(xMin, xMax);
            float randomZ = Random.Range(zMin, zMax);
            Vector3 spawnPos = new Vector3(randomX, flushSpawnY, randomZ);
            GameObject instance = Instantiate(cubeType3Prefab, spawnPos, cubeType3Quat);
            Renderer rend = instance.GetComponent<Renderer>();
            if (rend != null)
                rend.material = new Material(Shader.Find("Standard"));
        }

        // --- Spawn CubeType4 (Flush with extra downward offset) ---
        for (int i = 0; i < cubeType4Count; i++)
        {
            float randomX = Random.Range(xMin, xMax);
            float randomZ = Random.Range(zMin, zMax);
            Vector3 spawnPos = new Vector3(randomX, flushSpawnY - cubeType4YOffset, randomZ);
            GameObject instance = Instantiate(cubeType4Prefab, spawnPos, cubeType4Quat);
            Renderer rend = instance.GetComponent<Renderer>();
            if (rend != null)
                rend.material = new Material(Shader.Find("Standard"));
        }

        // --- Spawn CubeType5 (Flush with extra downward offset) ---
        for (int i = 0; i < cubeType5Count; i++)
        {
            float randomX = Random.Range(xMin, xMax);
            float randomZ = Random.Range(zMin, zMax);
            Vector3 spawnPos = new Vector3(randomX, flushSpawnY - cubeType5YOffset, randomZ);
            GameObject instance = Instantiate(cubeType5Prefab, spawnPos, cubeType5Quat);
            Renderer rend = instance.GetComponent<Renderer>();
            if (rend != null)
                rend.material = new Material(Shader.Find("Standard"));
        }
    }
}
