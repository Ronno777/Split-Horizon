using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody rb;
    public float forwardForce = 2000f;
    public float sidewaysForce = 500f;
    public float gravityForce = 9.81f;
    public float fallMultiplier = 2f;
    public float flipDuration = 1f;
    public float detachForce = 2f;  // Adjust for how gently the player detaches

    [Header("Level End Settings")]
    // Set this to the END prefab's transform so that once the player reaches or passes it, forward force stops.
    public Transform levelEnd;

    private bool isGravityFlipped = false;
    private bool isFlipping = false;
    public bool GravityFlipped { get { return isGravityFlipped; } }
    public bool IsFlipping { get { return isFlipping; } }

    // Expose current flip progress (0 = just started, 1 = finished)
    private float currentFlipProgress = 1f;
    public float FlipProgress { get { return currentFlipProgress; } }

    void Start()
    {
        rb.useGravity = false;             // Use custom gravity.
        rb.freezeRotation = true;          // We control rotation manually.
    }

    void FixedUpdate()
    {
        // Apply custom gravity.
        Vector3 gravityDir = isGravityFlipped ? Vector3.up : Vector3.down;
        float currentGravity = gravityForce;
        if (Vector3.Dot(rb.velocity, gravityDir) > 0)
        {
            currentGravity *= fallMultiplier;
        }
        rb.AddForce(gravityDir * currentGravity, ForceMode.Acceleration);

        // Only add forward force if the player's z is less than the levelEnd's z.
        if (levelEnd != null && rb.position.z < levelEnd.position.z)
        {
            rb.AddForce(0, 0, forwardForce * Time.deltaTime);
        }

        // Only allow lateral movement if the player is in positive z.
        if (rb.position.z > 0)
        {
            if (Input.GetKey("d"))
                rb.AddForce(sidewaysForce * Time.deltaTime, 0, 0, ForceMode.VelocityChange);
            if (Input.GetKey("a"))
                rb.AddForce(-sidewaysForce * Time.deltaTime, 0, 0, ForceMode.VelocityChange);
        }

        // Check for out-of-bounds (y too low or too high) and trigger destruction.
        if (rb.position.y < -2f || rb.position.y > 24f)
        {
            DestructionBehavior playerDestruction = GetComponent<DestructionBehavior>();
            if (playerDestruction != null)
            {
                playerDestruction.FractureObject();
            }
            FindObjectOfType<GameManager>().TriggerSlowMoAndEndGame();
        }
    }

    void Update()
    {
        // Only allow flip input if the player is in positive z.
        if (rb.position.z > 0 && Input.GetKeyDown(KeyCode.Space) && !isFlipping)
        {
            StartCoroutine(FlipGravityCoroutine());
        }
    }

    IEnumerator FlipGravityCoroutine()
    {
        isFlipping = true;
        // Toggle gravity.
        isGravityFlipped = !isGravityFlipped;

        // Apply a small impulse in the new gravity direction.
        Vector3 detachDir = isGravityFlipped ? Vector3.up : Vector3.down;
        rb.AddForce(detachDir * detachForce, ForceMode.Impulse);

        Quaternion startRot = transform.rotation;
        Quaternion endRot = startRot * Quaternion.Euler(0f, 0f, 180f);
        float elapsed = 0f;
        while (elapsed < flipDuration)
        {
            elapsed += Time.deltaTime;
            currentFlipProgress = Mathf.Clamp01(elapsed / flipDuration);
            transform.rotation = Quaternion.Slerp(startRot, endRot, currentFlipProgress);
            yield return null;
        }
        transform.rotation = endRot;
        currentFlipProgress = 1f;
        isFlipping = false;
    }
}
