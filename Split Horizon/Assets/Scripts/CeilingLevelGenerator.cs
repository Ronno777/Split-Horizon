using UnityEngine;

public class CeilingLevelGenerator : MonoBehaviour
{
    public GameObject cubePrefab;      // Your Cube prefab.
    public GameObject ceiling;         // The ceiling platform (e.g., at 0,15,40).
    public int cubeCount = 20;         // Number of cubes to spawn.
    public float spawnYOffset = 0.1f;    // Extra offset to avoid clipping.
    public float spawnHeightRange = 7f;  // Maximum additional downward offset.

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

        // Determine the cube's actual height using its collider.
        float cubeHeight = 1f;
        Collider cubeCol = cubePrefab.GetComponent<Collider>();
        if (cubeCol != null)
        {
            cubeHeight = cubeCol.bounds.size.y;
        }
        float cubeHalfHeight = cubeHeight / 2f;

        // Spawn cubes on the underside of the ceiling with a downward random offset.
        for (int i = 0; i < cubeCount; i++)
        {
            float randomX = Random.Range(xMin, xMax);
            float randomZ = Random.Range(zMin, zMax);
            // Random extra downward offset (0 to spawnHeightRange).
            float randomYOffset = Random.Range(0f, spawnHeightRange);
            // For ceiling, the base spawn position is when the cube's top touches the ceiling's bottom.
            // Then we subtract an extra random offset to make the cubes float down.
            Vector3 spawnPos = new Vector3(
                randomX,
                ceilingBottomY + cubeHalfHeight - spawnYOffset - randomYOffset,
                randomZ
            );
            GameObject cubeInstance = Instantiate(cubePrefab, spawnPos, Quaternion.identity);

            // Optionally reset material to default white.
            Renderer rend = cubeInstance.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material = new Material(Shader.Find("Standard"));
            }
        }
    }
}
