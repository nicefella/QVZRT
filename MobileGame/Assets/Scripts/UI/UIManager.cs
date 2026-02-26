using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Manages the HUD in the GameWorld scene:
/// - Fade overlay (black screen transition)
/// - "Enter / Exit" prompt that appears near building doors
///
/// Auto-finds FadeOverlay and EnterPrompt GameObjects by name.
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private CanvasGroup fadeOverlay;
    private GameObject enterPrompt;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        var fadeGO = GameObject.Find("FadeOverlay");
        if (fadeGO != null) fadeOverlay = fadeGO.GetComponent<CanvasGroup>();

        enterPrompt = GameObject.Find("EnterPrompt");
        if (enterPrompt != null) enterPrompt.SetActive(false);

        // Fade in when the scene loads
        StartCoroutine(FadeIn(0.6f));
    }

    /// <summary>Show or hide the door interaction prompt.</summary>
    public void ShowEnterPrompt(bool show)
    {
        if (enterPrompt != null) enterPrompt.SetActive(show);
    }

    /// <summary>
    /// Fade to black, run the action (e.g. swap interior/exterior),
    /// then fade back in.
    /// </summary>
    public void FadeAndExecute(System.Action action, float halfDuration = 0.4f)
    {
        StartCoroutine(FadeOutIn(action, halfDuration));
    }

    IEnumerator FadeIn(float duration)
    {
        if (fadeOverlay == null) yield break;
        fadeOverlay.alpha = 1f;
        float t = 1f;
        while (t > 0f)
        {
            t -= Time.deltaTime / duration;
            fadeOverlay.alpha = Mathf.Clamp01(t);
            yield return null;
        }
    }

    IEnumerator FadeOutIn(System.Action action, float duration)
    {
        // Fade OUT (to black)
        if (fadeOverlay != null)
        {
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / duration;
                fadeOverlay.alpha = Mathf.Clamp01(t);
                yield return null;
            }
        }

        action?.Invoke();
        yield return null; // one frame gap

        // Fade IN (back to visible)
        if (fadeOverlay != null)
        {
            float t = 1f;
            while (t > 0f)
            {
                t -= Time.deltaTime / duration;
                fadeOverlay.alpha = Mathf.Clamp01(t);
                yield return null;
            }
        }
    }
}
