using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    bool gameHasEnded = false;
    public float restartDelay = 1f;
    public GameObject completeLevelUI;

    public void CompleteLevel()
    {
        completeLevelUI.SetActive(true);
        Debug.Log("Level Won!");
    }

    // New method to trigger slow motion and then end the game.
    public void TriggerSlowMoAndEndGame()
    {
        if (!gameHasEnded)
        {
            gameHasEnded = true;
            Debug.Log("Game Over - Slow Mo triggered");
            StartCoroutine(SlowMoAndRestart());
        }
    }

    private IEnumerator SlowMoAndRestart()
    {
        // Set time scale very low to nearly freeze time.
        Time.timeScale = 0.01f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        // Wait for 2 real-time seconds (ignores the slowed timeScale).
        yield return new WaitForSecondsRealtime(2f);

        // Reset time scale.
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        // Use FadeManager to load the current scene with a fade transition.
        FadeManager fadeManager = FindObjectOfType<FadeManager>();
        if (fadeManager != null)
        {
            Debug.Log("Starting fade-out transition before scene reload.");
            fadeManager.LoadSceneWithFade(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            Debug.LogWarning("FadeManager not found! Reloading scene without fade.");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
