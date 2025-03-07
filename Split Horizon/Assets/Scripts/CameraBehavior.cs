using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    public Transform player;                      // Player's transform.
    public PlayerMovement playerMovement;         // Reference to the player's PlayerMovement script.
    public Vector3 offset = new Vector3(0, 2, -5);  // Base offset. The Y component is treated as a magnitude.

    public float fixedPitch = 10f;                // Fixed pitch angle.
    public float fixedYaw = 0f;                   // Fixed yaw angle.
    public float rollSmoothSpeed = 5f;            // Smoothing factor for roll (z rotation).

    // New parameter for lateral tilt (in degrees).
    public float lateralTiltAngle = 5f;

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

        // Get horizontal input from the player (A/D or arrow keys).
        float horizontalInput = Input.GetAxis("Horizontal"); // Value between -1 and 1.
        // Calculate the lateral tilt offset.
        float lateralTilt = horizontalInput * lateralTiltAngle;

        // Determine the target roll based on player's current roll (from flips) plus lateral tilt.
        float targetRoll = player.eulerAngles.z + lateralTilt;
        currentRoll = Mathf.LerpAngle(currentRoll, targetRoll, rollSmoothSpeed * Time.deltaTime);

        // Apply the fixed pitch and yaw with the smoothly updated roll.
        transform.rotation = Quaternion.Euler(fixedPitch, fixedYaw, currentRoll);
    }
}
