using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Manages the Avatar Creation screen.
/// Attached to the Canvas GameObject.
///
/// Tab buttons call ShowPanel("Body" | "Hair" | "Outfit" | "Accessory").
/// Swatch/option buttons call the appropriate On* methods.
/// "Start Game" button calls OnStartGame().
///
/// All buttons are wired at runtime via name-based lookup — no manual inspector wiring needed.
/// </summary>
public class AvatarCreationUI : MonoBehaviour
{
    private AvatarCustomizer previewCustomizer;
    private AvatarData workingData;

    // Category panels (auto-found by name)
    private GameObject bodyPanel;
    private GameObject hairPanel;
    private GameObject outfitPanel;
    private GameObject accessoryPanel;

    private CanvasGroup fadeOverlay;

    void Start()
    {
        if (GameManager.Instance == null)
            new GameObject("GameManager").AddComponent<GameManager>();

        // Clone current avatar data so edits are non-destructive until confirmed
        workingData = new AvatarData
        {
            bodyIndex       = GameManager.Instance.AvatarData.bodyIndex,
            skinColorIndex  = GameManager.Instance.AvatarData.skinColorIndex,
            hairIndex       = GameManager.Instance.AvatarData.hairIndex,
            hairColorIndex  = GameManager.Instance.AvatarData.hairColorIndex,
            outfitIndex     = GameManager.Instance.AvatarData.outfitIndex,
            accessoryFlags  = (bool[])GameManager.Instance.AvatarData.accessoryFlags.Clone()
        };

        previewCustomizer = FindObjectOfType<AvatarCustomizer>();

        // Auto-find panels
        bodyPanel      = GameObject.Find("BodyPanel");
        hairPanel      = GameObject.Find("HairPanel");
        outfitPanel    = GameObject.Find("OutfitPanel");
        accessoryPanel = GameObject.Find("AccessoryPanel");

        // Auto-find fade overlay
        var fadeGO = GameObject.Find("FadeOverlay");
        if (fadeGO != null) fadeOverlay = fadeGO.GetComponent<CanvasGroup>();

        // Wire tab buttons
        WireButton("Tab_Body",      () => ShowPanel("Body"));
        WireButton("Tab_Hair",      () => ShowPanel("Hair"));
        WireButton("Tab_Outfit",    () => ShowPanel("Outfit"));
        WireButton("Tab_Extras",    () => ShowPanel("Accessory"));
        WireButton("StartButton",   OnStartGame);

        // Wire body panel buttons
        WireButton("Body_Slim",     () => { workingData.bodyIndex = 0; RefreshPreview(); });
        WireButton("Body_Regular",  () => { workingData.bodyIndex = 1; RefreshPreview(); });

        // Wire hair style buttons
        for (int i = 0; i < 4; i++)
        {
            int idx = i;
            WireButton($"Hair_{i}", () => { workingData.hairIndex = idx; RefreshPreview(); });
        }

        // Wire skin swatches
        for (int i = 0; i < 5; i++)
        {
            int idx = i;
            WireButton($"SkinSwatch_{i}", () => { workingData.skinColorIndex = idx; RefreshPreview(); });
        }

        // Wire hair colour swatches
        for (int i = 0; i < 6; i++)
        {
            int idx = i;
            WireButton($"HairColor_{i}", () => { workingData.hairColorIndex = idx; RefreshPreview(); });
        }

        // Wire outfit buttons
        for (int i = 0; i < 4; i++)
        {
            int idx = i;
            WireButton($"Outfit_{i}", () => { workingData.outfitIndex = idx; RefreshPreview(); });
        }

        // Wire accessory toggle buttons
        for (int i = 0; i < 3; i++)
        {
            int idx = i;
            WireButton($"Acc_{i}", () =>
            {
                workingData.accessoryFlags[idx] = !workingData.accessoryFlags[idx];
                RefreshPreview();
            });
        }

        ShowPanel("Body");
        StartCoroutine(FadeIn());
        RefreshPreview();
    }

    void WireButton(string goName, UnityEngine.Events.UnityAction action)
    {
        var go = GameObject.Find(goName);
        if (go == null) return;
        var btn = go.GetComponent<Button>();
        if (btn != null) btn.onClick.AddListener(action);
    }

    public void ShowPanel(string panelName)
    {
        if (bodyPanel      != null) bodyPanel.SetActive(panelName == "Body");
        if (hairPanel      != null) hairPanel.SetActive(panelName == "Hair");
        if (outfitPanel    != null) outfitPanel.SetActive(panelName == "Outfit");
        if (accessoryPanel != null) accessoryPanel.SetActive(panelName == "Accessory");
    }

    void RefreshPreview()
    {
        previewCustomizer?.ApplyData(workingData);
    }

    public void OnStartGame()
    {
        GameManager.Instance.AvatarData = workingData;
        StartCoroutine(FadeAndLoad("GameWorld"));
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
