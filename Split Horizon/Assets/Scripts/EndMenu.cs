using UnityEngine;
using UnityEngine.SceneManagement;

public class EndMenu : MonoBehaviour
{
    public void RestartGame()
    {
        FindObjectOfType<FadeManager>().LoadSceneWithFade(0); // Loads the start menu (build #0)
    }
}
