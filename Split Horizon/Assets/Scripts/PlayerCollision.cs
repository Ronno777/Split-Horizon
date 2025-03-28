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
            // Check if invincibility is active.
            CheatCodes cheatCodes = GetComponent<CheatCodes>();
            if (cheatCodes == null)
            {
                cheatCodes = FindObjectOfType<CheatCodes>();
            }
            if (cheatCodes != null && cheatCodes.invincible)
            {
                Debug.Log("Invincible! Now ignoring obstacle collision.");
                Collider playerCollider = GetComponent<Collider>();
                if (playerCollider != null)
                {
                    Physics.IgnoreCollision(playerCollider, collisionInfo.collider, true);
                }
                return;
            }

            // Cache the destruction behavior components.
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
