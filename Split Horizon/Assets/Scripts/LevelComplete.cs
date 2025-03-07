using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelComplete : MonoBehaviour
{
    public void LoadNextLevel()
    {
        // Use FadeManager to load the next scene with fade transition
        FindObjectOfType<FadeManager>().LoadSceneWithFade(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
