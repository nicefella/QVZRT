using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Manages a full-screen black Image used for fade-in / fade-out transitions.
///
/// Setup:
///   1. Add a Canvas (Screen Space – Overlay, Sort Order 99) to the scene.
///   2. Add a child Image that covers the full canvas. Set its colour to black.
///   3. Attach this script to the Canvas (or any GameObject in the scene).
///   4. Assign the Image to the fadeImage field.
///
/// The image starts transparent and is brought to alpha 1 on FadeOut,
/// then back to 0 on FadeIn.
/// </summary>
public class FadeScreen : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 0.5f;

    private void Awake()
    {
        // Always start transparent
        SetAlpha(0f);
    }

    // -----------------------------------------------------------------------
    // Public API
    // -----------------------------------------------------------------------

    /// <summary>Fade to black then load a new scene.</summary>
    public void FadeToScene(string sceneName)
    {
        StartCoroutine(FadeOutThenLoad(sceneName));
    }

    /// <summary>Fade from black to clear (call after a scene has loaded).</summary>
    public IEnumerator FadeIn()
    {
        yield return StartCoroutine(Fade(1f, 0f));
    }

    /// <summary>Fade from clear to black.</summary>
    public IEnumerator FadeOut()
    {
        yield return StartCoroutine(Fade(0f, 1f));
    }

    // -----------------------------------------------------------------------
    // Private helpers
    // -----------------------------------------------------------------------

    private IEnumerator FadeOutThenLoad(string sceneName)
    {
        yield return StartCoroutine(Fade(0f, 1f));
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator Fade(float fromAlpha, float toAlpha)
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            SetAlpha(Mathf.Lerp(fromAlpha, toAlpha, elapsed / fadeDuration));
            yield return null;
        }
        SetAlpha(toAlpha);
    }

    private void SetAlpha(float a)
    {
        if (fadeImage == null) return;
        Color c = fadeImage.color;
        c.a = a;
        fadeImage.color = c;
    }
}
