using System.Collections.Generic;
using UnityEngine;
using TMPro;  // Required for TextMeshProUGUI

public class CheatCodes : MonoBehaviour
{
    // Define cheat codes with 5-character strings.
    [SerializeField]
    private string skipLevelCode = "UUUDD"; // Example: Up, Up, Up, Down, Down

    [SerializeField]
    private string invincibilityCode = "LLRLL"; // Example: Left, Left, Right, Left, Left

    // Queue to hold the latest five arrow inputs.
    private List<char> inputSequence = new List<char>();

    // Reference to the GameManager.
    private GameManager gameManager;

    // Invincibility flag.
    public bool invincible { get; private set; } = false;

    // Reference to the TextMeshProUGUI text for invincibility.
    [SerializeField]
    private TextMeshProUGUI invincibilityText;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        // Ensure the invincibility text is hidden at start.
        if (invincibilityText != null)
        {
            invincibilityText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // Listen for arrow key presses.
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            RegisterInput('U');
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            RegisterInput('D');
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            RegisterInput('L');
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            RegisterInput('R');
        }
    }

    void RegisterInput(char arrow)
    {
        Debug.Log("Arrow key pressed: " + arrow);
        // Add new input and keep only the latest five.
        inputSequence.Add(arrow);
        if (inputSequence.Count > 5)
        {
            inputSequence.RemoveAt(0);
        }
        CheckCheatCode();
    }

    void CheckCheatCode()
    {
        if (inputSequence.Count < 5)
            return;

        // Convert the list to a string.
        string currentCode = new string(inputSequence.ToArray());
        Debug.Log("Current cheat code sequence: " + currentCode);

        // Check if the skip level code was entered.
        if (currentCode == skipLevelCode)
        {
            Debug.Log("Skip Level cheat activated: " + skipLevelCode);

            // Stop the player's movement by freezing its Rigidbody.
            PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
            if (playerMovement != null && playerMovement.rb != null)
            {
                playerMovement.rb.constraints = RigidbodyConstraints.FreezeAll;
                Debug.Log("Player Rigidbody constraints set to FreezeAll.");
            }

            // Complete the level.
            if (gameManager != null)
                gameManager.CompleteLevel();

            inputSequence.Clear();
        }
        // Check if the invincibility code was entered.
        else if (currentCode == invincibilityCode)
        {
            Debug.Log("Invincibility cheat activated: " + invincibilityCode);
            EnableInvincibility();
            inputSequence.Clear();
        }
    }

    void EnableInvincibility()
    {
        invincible = true;
        Debug.Log("Invincibility enabled.");

        // Activate the TextMeshPro text.
        if (invincibilityText != null)
        {
            invincibilityText.gameObject.SetActive(true);
        }
    }
}
