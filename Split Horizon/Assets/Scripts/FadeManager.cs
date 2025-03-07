using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    public Image fadeImage;
    public TextMeshProUGUI levelText;  // Reference to the level text
    public float fadeDuration = 1f;    // Duration for fade transitions

    private void Start()
    {
        // Start with a fully opaque black fade and level text fully visible.
        fadeImage.color = new Color(0, 0, 0, 1);
        if (levelText != null)
        {
            levelText.text = "Level " + SceneManager.GetActiveScene().buildIndex;
            levelText.color = new Color(levelText.color.r, levelText.color.g, levelText.color.b, 1);
        }
        StartCoroutine(FadeIn());
    }

    public void LoadSceneWithFade(int sceneIndex)
    {
        StartCoroutine(FadeOutAndLoadScene(sceneIndex));
    }

    private IEnumerator FadeIn()
    {
        float elapsed = 0f;
        // Fade from opaque to transparent over fadeDuration.
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, elapsed / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            if (levelText != null)
            {
                // Level text fades concurrently with the fade image.
                levelText.color = new Color(levelText.color.r, levelText.color.g, levelText.color.b, alpha);
            }
            yield return null;
        }
        // Ensure both are fully transparent at the end.
        fadeImage.color = new Color(0, 0, 0, 0);
        if (levelText != null)
            levelText.color = new Color(levelText.color.r, levelText.color.g, levelText.color.b, 0);
    }

    private IEnumerator FadeOutAndLoadScene(int sceneIndex)
    {
        // Prepare level text for the next scene.
        if (levelText != null)
        {
            levelText.text = "Level " + sceneIndex;
            levelText.color = new Color(levelText.color.r, levelText.color.g, levelText.color.b, 0);
        }

        float elapsed = 0f;
        // Fade from transparent to opaque over fadeDuration.
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, elapsed / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            if (levelText != null)
            {
                // Level text fades concurrently with the fade image.
                levelText.color = new Color(levelText.color.r, levelText.color.g, levelText.color.b, alpha);
            }
            yield return null;
        }
        // Load the next scene after fade-out.
        SceneManager.LoadScene(sceneIndex);
    }
}