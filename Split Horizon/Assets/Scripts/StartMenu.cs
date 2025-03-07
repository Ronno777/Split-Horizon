using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public void StartGame()
    {
        // Use FadeManager to load the next scene with fade transition
        FindObjectOfType<FadeManager>().LoadSceneWithFade(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
