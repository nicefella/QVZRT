using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the Main Menu scene.
///
/// Scene setup:
///   - Canvas
///       - BackgroundImage   (full-screen)
///       - GameTitleText     (TextMeshProUGUI)
///       - HeroImage         (Image)
///       - PlayButton        (Button) → wire to this script's playButton field
///   - FadeCanvas (separate Canvas, Sort Order 99)
///       - FadeImage → wire to FadeScreen component
/// </summary>
public class MenuManager : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private FadeScreen fadeScreen;

    private void Start()
    {
        playButton.onClick.AddListener(OnPlayClicked);

        // Fade in when the menu first loads
        StartCoroutine(fadeScreen.FadeIn());
    }

    private void OnPlayClicked()
    {
        playButton.interactable = false;          // prevent double-tap
        fadeScreen.FadeToScene("AvatarCreation");
    }
}
