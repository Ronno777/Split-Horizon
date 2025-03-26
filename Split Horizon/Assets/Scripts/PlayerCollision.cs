using System.Collections;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public PlayerMovement movement;
    public float fractureDelay = 0.1f; // delay in real-time seconds

    void OnCollisionEnter(Collision collisionInfo)
    {
        if (collisionInfo.collider.CompareTag("Obstacle"))
        {
            // Cache the destruction behavior components immediately.
            DestructionBehavior obstacleDestruction = collisionInfo.collider.GetComponent<DestructionBehavior>();
            DestructionBehavior playerDestruction = GetComponent<DestructionBehavior>();

            // Disable player movement.
            movement.enabled = false;

            // Trigger slow-motion effect and end game via GameManager.
            FindObjectOfType<GameManager>().TriggerSlowMoAndEndGame();

            // Start coroutine with cached references.
            StartCoroutine(FractureAfterDelay(obstacleDestruction, playerDestruction));
        }
    }

    private IEnumerator FractureAfterDelay(DestructionBehavior obstacleDestruction, DestructionBehavior playerDestruction)
    {
        // Wait briefly in real time so slow motion is active.
        yield return new WaitForSecondsRealtime(fractureDelay);

        if (obstacleDestruction != null)
        {
            obstacleDestruction.FractureObject();
        }
        if (playerDestruction != null)
        {
            playerDestruction.FractureObject();
        }
    }
}
