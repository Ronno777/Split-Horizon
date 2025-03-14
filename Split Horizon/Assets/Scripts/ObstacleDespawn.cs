using UnityEngine;

public class ObstacleDespawn : MonoBehaviour
{
    // Called when another collider enters the trigger collider attached to this GameObject.
    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider belongs to a cube.
        if (other.CompareTag("Obstacle"))
        {
            Destroy(other.gameObject);
        }
    }
}
