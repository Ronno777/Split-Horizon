using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreAndProgress : MonoBehaviour
{
    [Header("References")]
    public Transform player;             // The player's Transform
    public Transform endPoint;           // The "END" prefab's Transform
    public TextMeshProUGUI scoreText;    // Text for displaying the player's z-position
    public Image progressCircle;         // UI circle image that fills as the player progresses
    public TextMeshProUGUI gameOverText; // GameOver text attached to the same canvas

    [Header("Settings")]
    [Tooltip("The player's starting Z position for progress calculation.")]
    public float startZ = 0f;            // Typically 0 if your player starts at z=0.

    private void Start()
    {
        // Ensure all references are assigned.
        if (player == null || endPoint == null || scoreText == null || progressCircle == null || gameOverText == null)
        {
            Debug.LogError("ScoreAndProgress: One or more references are not assigned. Disabling script.");
            enabled = false;
            return;
        }

        // Initialize the progress circle and hide the GameOver text.
        progressCircle.fillAmount = 0f;
        gameOverText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (player == null)
        {
            // If player is null, activate the GameOver text.
            gameOverText.gameObject.SetActive(true);
            return;
        }
        else
        {
            // Ensure the GameOver text is hidden while the player exists.
            gameOverText.gameObject.SetActive(false);
        }

        float playerZ = player.position.z;
        float endZ = endPoint.position.z;

        // Freeze the score text if player's z reaches or exceeds the end.
        if (playerZ >= endZ)
        {
            scoreText.text = endZ.ToString("0");
        }
        else if (playerZ > 0)
        {
            scoreText.text = playerZ.ToString("0");
        }
        else
        {
            scoreText.text = "";
        }

        // Update the progress circle.
        if (endZ > startZ) // Avoid division by zero.
        {
            float clampedZ = Mathf.Clamp(playerZ, startZ, endZ);
            float progress = (clampedZ - startZ) / (endZ - startZ);
            progressCircle.fillAmount = progress;
        }
    }
}
