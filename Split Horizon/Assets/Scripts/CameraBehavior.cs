using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    public Transform player;                      // Player's transform.
    public PlayerMovement playerMovement;         // Reference to the player's PlayerMovement script.
    public Vector3 offset = new Vector3(0, 2, -5);  // Base offset. The Y component is treated as a magnitude.

    public float fixedPitch = 10f;                  // Fixed pitch angle.
    public float fixedYaw = 0f;                     // Fixed yaw angle.
    public float rollSmoothSpeed = 5f;              // Smoothing factor for roll (z rotation).

    private float currentRoll = 0f;

    // Variables for smoothly interpolating the Y offset.
    private float startingYOffset = 0f;
    private bool hasStoredStartingYOffset = false;

    void LateUpdate()
    {
        if (player == null || playerMovement == null)
            return;

        // Determine the target Y offset based on gravity.
        float baseY = Mathf.Abs(offset.y);
        float targetY = playerMovement.GravityFlipped ? -baseY : baseY;
        float effectiveY = targetY;

        // When flipping, gradually interpolate the Y offset.
        if (playerMovement.IsFlipping)
        {
            // Store the starting offset at the moment the flip begins.
            if (!hasStoredStartingYOffset)
            {
                startingYOffset = transform.position.y - player.position.y;
                hasStoredStartingYOffset = true;
            }
            effectiveY = Mathf.Lerp(startingYOffset, targetY, playerMovement.FlipProgress);
        }
        else
        {
            hasStoredStartingYOffset = false;
        }

        // Set camera position with fixed X and Z offsets relative to the player.
        transform.position = new Vector3(
            player.position.x + offset.x,
            player.position.y + effectiveY,
            player.position.z + offset.z
        );

        // Gradually update the camera's roll (Z rotation) to follow the player's flip.
        float targetRoll = player.eulerAngles.z;
        currentRoll = Mathf.LerpAngle(currentRoll, targetRoll, rollSmoothSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(fixedPitch, fixedYaw, currentRoll);
    }
}
