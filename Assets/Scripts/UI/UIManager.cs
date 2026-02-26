using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages in-game HUD elements for the GameWorld scene.
///
/// Scene setup:
///   HUD Canvas
///     EnterPromptPanel (GameObject)
///       EnterPromptText  (TextMeshProUGUI)
///       EnterPromptButton (Button)
///
/// BuildingEntrance scripts call ShowEnterPrompt / HideEnterPrompt.
/// </summary>
public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject        enterPromptPanel;
    [SerializeField] private TextMeshProUGUI   enterPromptText;
    [SerializeField] private Button            enterPromptButton;

    private Action _onPromptConfirm;

    private void Awake()
    {
        enterPromptPanel.SetActive(false);
        enterPromptButton.onClick.AddListener(OnEnterPromptClicked);
    }

    // -----------------------------------------------------------------------
    // Public API — called by BuildingEntrance
    // -----------------------------------------------------------------------

    /// <summary>Show the "Tap to Enter / Exit" prompt above a door.</summary>
    public void ShowEnterPrompt(string message, Action onConfirm)
    {
        _onPromptConfirm    = onConfirm;
        enterPromptText.text = message;
        enterPromptPanel.SetActive(true);
    }

    /// <summary>Hide the prompt (e.g. player walked away from the door).</summary>
    public void HideEnterPrompt()
    {
        enterPromptPanel.SetActive(false);
        _onPromptConfirm = null;
    }

    // -----------------------------------------------------------------------
    // Private
    // -----------------------------------------------------------------------

    private void OnEnterPromptClicked()
    {
        _onPromptConfirm?.Invoke();
    }
}
