using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Attached to the Canvas in the MainMenu scene.
/// Auto-finds the FadeOverlay and PlayButton by name at runtime — no manual wiring needed.
/// </summary>
public class MenuManager : MonoBehaviour
{
    private CanvasGroup fadeOverlay;

    void Start()
    {
        // Ensure GameManager exists when starting directly from this scene (editor testing)
        if (GameManager.Instance == null)
            new GameObject("GameManager").AddComponent<GameManager>();

        // Auto-find fade overlay
        var fadeGO = GameObject.Find("FadeOverlay");
        if (fadeGO != null) fadeOverlay = fadeGO.GetComponent<CanvasGroup>();

        // Auto-find the play button and wire it
        var playBtn = GameObject.Find("PlayButton");
        if (playBtn != null)
        {
            var btn = playBtn.GetComponent<Button>();
            if (btn != null) btn.onClick.AddListener(OnPlayButtonClicked);
        }

        // Fade in on scene start
        StartCoroutine(FadeIn());
    }

    public void OnPlayButtonClicked()
    {
        StartCoroutine(FadeAndLoad("AvatarCreation"));
    }

    IEnumerator FadeIn()
    {
        if (fadeOverlay == null) yield break;
        fadeOverlay.alpha = 1f;
        float t = 1f;
        while (t > 0f)
        {
            t -= Time.deltaTime * 2f;
            fadeOverlay.alpha = Mathf.Clamp01(t);
            yield return null;
        }
    }

    IEnumerator FadeAndLoad(string sceneName)
    {
        if (fadeOverlay != null)
        {
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * 2f;
                fadeOverlay.alpha = Mathf.Clamp01(t);
                yield return null;
            }
        }
        GameManager.Instance.LoadScene(sceneName);
    }
}
