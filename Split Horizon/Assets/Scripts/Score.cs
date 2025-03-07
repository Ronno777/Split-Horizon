using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public Transform player;
    public TextMeshProUGUI scoreText;

    void Update()
    {
        // Check if the player is null before accessing position
        if (player != null)
        {
            scoreText.text = player.position.z.ToString("0");
        }
        else
        {
            // Optional: Display "Game Over" or a static score if the player is null
            scoreText.text = "Game Over";
        }
    }
}
