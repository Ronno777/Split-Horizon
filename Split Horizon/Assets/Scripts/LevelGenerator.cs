using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Prefab Settings")]
    public GameObject cubePrefab;         // CubeType1: Normal cube (floats with vertical randomness)
    public GameObject cubeType2Prefab;      // CubeType2 prefab
    public GameObject cubeType3Prefab;      // CubeType3 prefab
    public GameObject cubeType4Prefab;      // CubeType4 prefab (needs rotation fix and extra Y offset)
    public GameObject cubeType5Prefab;      // CubeType5 prefab (needs rotation fix and extra Y offset)

    [Header("Rotation Offsets (Euler Angles) for CubeTypes 2-5")]
    public Vector3 cubeType2Rotation;
    public Vector3 cubeType3Rotation;
    public Vector3 cubeType4Rotation;  // e.g., (90, 0, 0) to orient vertically
    public Vector3 cubeType5Rotation;  // e.g., (90, 0, 0) to orient vertically

    [Header("Custom Y Offsets for CubeTypes")]
    public float cubeType4YOffset = 1f;   // Extra upward offset for CubeType4
    public float cubeType5YOffset = 0.5f;   // Extra upward offset for CubeType5

    [Header("Platform Settings")]
    public GameObject ground;             // Ground object.

    [Header("CubeType1 Settings (Floating Cubes)")]
    public int cubeCount = 20;            // Number of CubeType1 cubes.
    public float spawnYOffset = 0.1f;       // Base offset above the platform.
    public float spawnHeightRange = 7f;     // Additional vertical range above the platform for CubeType1.

    [Header("Other Cube Type Counts (Flush with the Platform)")]
    public int cubeType2Count = 10;
    public int cubeType3Count = 10;
    public int cubeType4Count = 10;
    public int cubeType5Count = 10;

    [Header("Density Settings")]
    [Tooltip("Exponent for biasing the Z coordinate. Values < 1 will bias spawns toward higher Z.")]
    public float densityExponent = 0.5f;

    void Start()
    {
        // Calculate ground boundaries.
        Vector3 groundPos = ground.transform.position;
        Vector3 groundScale = ground.transform.localScale;
        float groundXMin = groundPos.x - groundScale.x / 2f;
        float groundXMax = groundPos.x + groundScale.x / 2f;
        float groundZMin = groundPos.z - groundScale.z / 2f;
        float groundZMax = groundPos.z + groundScale.z / 2f;

        // Determine the top Y-coordinate of the ground.
        float groundTopY = groundPos.y + groundScale.y / 2f;

        // Get CubeType1's actual height using its collider.
        float cubeHeight = 1f;
        Collider cubeCollider = cubePrefab.GetComponent<Collider>();
        if (cubeCollider != null)
            cubeHeight = cubeCollider.bounds.size.y;
        float cubeHalfHeight = cubeHeight / 2f;

        // Function to generate a biased Z coordinate.
        float GetRandomZ()
        {
            // Using Random.value with exponent densityExponent biases toward higher Z if densityExponent < 1.
            return groundZMin + Mathf.Pow(Random.value, densityExponent) * (groundZMax - groundZMin);
        }

        // Spawn CubeType1: Floating cubes with random vertical offset.
        for (int i = 0; i < cubeCount; i++)
        {
            float randomX = Random.Range(groundXMin, groundXMax);
            float randomZ = GetRandomZ();
            float randomYOffset = Random.Range(0f, spawnHeightRange);
            Vector3 spawnPos = new Vector3(
                randomX,
                groundTopY + cubeHalfHeight + spawnYOffset + randomYOffset,
                randomZ
            );
            Instantiate(cubePrefab, spawnPos, Quaternion.identity);
        }

        // Spawn CubeType2 (flush with the platform).
        Quaternion cubeType2Quat = Quaternion.Euler(cubeType2Rotation);
        for (int i = 0; i < cubeType2Count; i++)
        {
            float randomX = Random.Range(groundXMin, groundXMax);
            float randomZ = GetRandomZ();
            Vector3 spawnPos = new Vector3(
                randomX,
                groundTopY + cubeHalfHeight + spawnYOffset,
                randomZ
            );
            Instantiate(cubeType2Prefab, spawnPos, cubeType2Quat);
        }

        // Spawn CubeType3 (flush with the platform).
        Quaternion cubeType3Quat = Quaternion.Euler(cubeType3Rotation);
        for (int i = 0; i < cubeType3Count; i++)
        {
            float randomX = Random.Range(groundXMin, groundXMax);
            float randomZ = GetRandomZ();
            Vector3 spawnPos = new Vector3(
                randomX,
                groundTopY + cubeHalfHeight + spawnYOffset,
                randomZ
            );
            Instantiate(cubeType3Prefab, spawnPos, cubeType3Quat);
        }

        // Spawn CubeType4 (flush with the platform, plus custom Y offset).
        Quaternion cubeType4Quat = Quaternion.Euler(cubeType4Rotation) * cubeType4Prefab.transform.rotation;
        for (int i = 0; i < cubeType4Count; i++)
        {
            float randomX = Random.Range(groundXMin, groundXMax);
            float randomZ = GetRandomZ();
            Vector3 spawnPos = new Vector3(
                randomX,
                groundTopY + cubeHalfHeight + spawnYOffset + cubeType4YOffset,
                randomZ
            );
            Instantiate(cubeType4Prefab, spawnPos, cubeType4Quat);
        }

        // Spawn CubeType5 (flush with the platform, plus custom Y offset).
        Quaternion cubeType5Quat = Quaternion.Euler(cubeType5Rotation) * cubeType5Prefab.transform.rotation;
        for (int i = 0; i < cubeType5Count; i++)
        {
            float randomX = Random.Range(groundXMin, groundXMax);
            float randomZ = GetRandomZ();
            Vector3 spawnPos = new Vector3(
                randomX,
                groundTopY + cubeHalfHeight + spawnYOffset + cubeType5YOffset,
                randomZ
            );
            Instantiate(cubeType5Prefab, spawnPos, cubeType5Quat);
        }
    }
}
