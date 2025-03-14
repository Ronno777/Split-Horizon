using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public Rigidbody rb;
    public float forwardForce = 2000f;
    public float sidewaysForce = 500f;

    [Header("Gravity Settings")]
    public float gravityForce = 9.81f;
    public float fallMultiplier = 2f;
    public float detachForce = 2f; // Impulse applied when flipping

    [Header("Flip Settings")]
    public float flipDuration = 1f; // Duration of the flip animation

    [Header("Level End Settings")]
    public Transform levelEnd; // Forward force stops when the player's z reaches this

    private bool isGravityFlipped = false;
    private bool isFlipping = false;
    private float currentFlipProgress = 1f;

    public bool GravityFlipped { get { return isGravityFlipped; } }
    public bool IsFlipping { get { return isFlipping; } }
    public float FlipProgress { get { return currentFlipProgress; } }

    void Start()
    {
        rb.useGravity = false;  // Using custom gravity logic
        rb.freezeRotation = true; // Prevent physics from affecting rotation
    }

    void FixedUpdate()
    {
        ApplyCustomGravity();
        ApplyForwardForce();
        ApplyLateralMovement();
        CheckOutOfBounds();
    }

    void Update()
    {
        ProcessInput();
    }

    private void ApplyCustomGravity()
    {
        Vector3 gravityDir = isGravityFlipped ? Vector3.up : Vector3.down;
        float appliedGravity = gravityForce;
        // Increase gravity effect when moving with gravity direction.
        if (Vector3.Dot(rb.velocity, gravityDir) > 0)
        {
            appliedGravity *= fallMultiplier;
        }
        rb.AddForce(gravityDir * appliedGravity, ForceMode.Acceleration);
    }

    private void ApplyForwardForce()
    {
        if (levelEnd != null && rb.position.z < levelEnd.position.z)
        {
            rb.AddForce(Vector3.forward * forwardForce * Time.deltaTime);
        }
    }

    private void ApplyLateralMovement()
    {
        if (rb.position.z > -50)
        {
            if (Input.GetKey("d"))
            {
                rb.AddForce(Vector3.right * sidewaysForce * Time.deltaTime, ForceMode.VelocityChange);
            }
            if (Input.GetKey("a"))
            {
                rb.AddForce(Vector3.left * sidewaysForce * Time.deltaTime, ForceMode.VelocityChange);
            }
        }
    }

    private void CheckOutOfBounds()
    {
        if (rb.position.y < -2f || rb.position.y > 24f)
        {
            DestructionBehavior destruction = GetComponent<DestructionBehavior>();
            if (destruction != null)
            {
                destruction.FractureObject();
            }
            GameManager gm = FindObjectOfType<GameManager>();
            if (gm != null)
            {
                gm.TriggerSlowMoAndEndGame();
            }
        }
    }

    private void ProcessInput()
    {
        // Use spacebar to flip gravity if the player is in positive z and not already flipping.
        if (rb.position.z > 0 && Input.GetKeyDown(KeyCode.Space) && !isFlipping)
        {
            StartCoroutine(FlipGravityCoroutine());
        }
    }

    private IEnumerator FlipGravityCoroutine()
    {
        isFlipping = true;
        // Toggle gravity state.
        isGravityFlipped = !isGravityFlipped;

        // Apply a small impulse in the new gravity direction.
        Vector3 detachDir = isGravityFlipped ? Vector3.up : Vector3.down;
        rb.AddForce(detachDir * detachForce, ForceMode.Impulse);

        // Rotate 180 degrees around the z-axis over flipDuration.
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
