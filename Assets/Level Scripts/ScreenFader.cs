using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    public Image fadeImage;  // Reference to the UI Image for fading
    public float fadeDuration = 1f;  // Duration of fade in/out

    private void Start()
    {
        // Start with the scene fading in
        StartCoroutine(FadeIn());
    }

    public IEnumerator FadeIn()
    {
        // Fade from opaque to transparent
        float elapsedTime = 0f;
        Color color = fadeImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }
    }
}
