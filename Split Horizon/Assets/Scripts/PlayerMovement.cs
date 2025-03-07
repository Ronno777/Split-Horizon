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
        Vector3 gravityDir = isGravityFlipped ? Vector3.up : Vector3.down;
        float currentGravity = gravityForce;
        if (Vector3.Dot(rb.velocity, gravityDir) > 0)
        {
            currentGravity *= fallMultiplier;
        }
        rb.AddForce(gravityDir * currentGravity, ForceMode.Acceleration);

        rb.AddForce(0, 0, forwardForce * Time.deltaTime);
        if (Input.GetKey("d"))
            rb.AddForce(sidewaysForce * Time.deltaTime, 0, 0, ForceMode.VelocityChange);
        if (Input.GetKey("a"))
            rb.AddForce(-sidewaysForce * Time.deltaTime, 0, 0, ForceMode.VelocityChange);

        if (rb.position.y < -2f || rb.position.y > 24f)
        {
            DestructionBehavior playerDestruction = GetComponent<DestructionBehavior>();
            if (playerDestruction != null)
            {
                playerDestruction.FractureObject();
            }
            FindObjectOfType<GameManager>().EndGame();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isFlipping)
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
