using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CameraBehavior : MonoBehaviour
{
    public Transform player;                      // Player's transform.
    public PlayerMovement playerMovement;         // Reference to the player's PlayerMovement script.
    public Vector3 offset = new Vector3(0, 2, -5);  // Base offset. The Y component is treated as a magnitude.

    public float fixedPitch = 10f;                // Fixed pitch angle when not flipped.
    public float fixedYaw = 0f;                   // Fixed yaw angle.
    public float rollSmoothSpeed = 5f;            // Smoothing factor for roll (z rotation).

    // New parameter for lateral tilt (in degrees).
    public float lateralTiltAngle = 5f;

    private float currentRoll = 0f;
    private float currentPitch = 0f;  // Smoothly updated pitch.

    // Variables for smoothly interpolating the Y offset.
    private float startingYOffset = 0f;
    private bool hasStoredStartingYOffset = false;

    void Start()
    {
        if (player != null && playerMovement != null)
        {
            // Initialize currentPitch based on the player's current gravity state.
            currentPitch = playerMovement.GravityFlipped ? -fixedPitch : fixedPitch;
        }
    }

    void LateUpdate()
    {
        if (player == null || playerMovement == null)
            return;

        // Determine the target Y offset based on gravity.
        float baseY = Mathf.Abs(offset.y);
        float targetY = playerMovement.GravityFlipped ? -baseY : baseY;
        float effectiveY = targetY;

        // When flipping, interpolate the Y offset.
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

        // Calculate lateral tilt based on horizontal input.
        float horizontalInput = Input.GetAxis("Horizontal"); // Value between -1 and 1.
        float lateralTilt = horizontalInput * lateralTiltAngle;

        // Update roll (Z rotation) smoothly.
        float targetRoll = player.eulerAngles.z + lateralTilt;
        currentRoll = Mathf.LerpAngle(currentRoll, targetRoll, rollSmoothSpeed * Time.deltaTime);

        // Smoothly update pitch toward the desired target.
        float targetPitch = playerMovement.GravityFlipped ? -fixedPitch : fixedPitch;
        // Using the same rollSmoothSpeed for pitch smoothing; adjust as needed.
        currentPitch = Mathf.LerpAngle(currentPitch, targetPitch, rollSmoothSpeed * Time.deltaTime);

        // Apply the final rotation with the computed pitch, fixed yaw, and smoothed roll.
        transform.rotation = Quaternion.Euler(currentPitch, fixedYaw, currentRoll);
    }
}
