using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public PlayerMovement movement;

    void OnCollisionEnter(Collision collisionInfo)
    {
        if (collisionInfo.collider.CompareTag("Obstacle"))
        {
            // Fracture the obstacle.
            DestructionBehavior obstacleDestruction = collisionInfo.collider.GetComponent<DestructionBehavior>();
            if (obstacleDestruction != null)
            {
                obstacleDestruction.FractureObject();
            }

            // Disable player movement.
            movement.enabled = false;

            // Fracture the player.
            DestructionBehavior playerDestruction = GetComponent<DestructionBehavior>();
            if (playerDestruction != null)
            {
                playerDestruction.FractureObject();
            }
        }
    }
}
