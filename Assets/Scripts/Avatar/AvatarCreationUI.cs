using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Wires up the Avatar Creation screen UI to AvatarCustomizer.
///
/// Scene layout expected:
///
///   [Left half]
///     RenderTexture Camera → preview of the avatar model
///
///   [Right half]
///     Tab buttons: Body | Hair | Outfit | Accessories
///     Panels (one per tab, only one active at a time):
///       BodyPanel
///         ← → buttons (bodyPrevButton, bodyNextButton)
///         Skin colour swatch buttons (skinColorButtons[])
///       HairPanel
///         ← → buttons
///         Hair colour swatches (hairColorButtons[])
///       OutfitPanel
///         ← → buttons
///       AccessoryPanel
///         Hat Toggle, Glasses Toggle, Bag Toggle
///
///   StartButton  → fades to GameWorld
/// </summary>
public class AvatarCreationUI : MonoBehaviour
{
    [SerializeField] private AvatarCustomizer previewer;
    [SerializeField] private FadeScreen       fadeScreen;

    // ------------------------------------------------------------------
    // Tab buttons
    // ------------------------------------------------------------------
    [Header("Tab Buttons")]
    [SerializeField] private Button bodyTabButton;
    [SerializeField] private Button hairTabButton;
    [SerializeField] private Button outfitTabButton;
    [SerializeField] private Button accessoryTabButton;

    // ------------------------------------------------------------------
    // Panels
    // ------------------------------------------------------------------
    [Header("Panels")]
    [SerializeField] private GameObject bodyPanel;
    [SerializeField] private GameObject hairPanel;
    [SerializeField] private GameObject outfitPanel;
    [SerializeField] private GameObject accessoryPanel;

    // ------------------------------------------------------------------
    // Body panel controls
    // ------------------------------------------------------------------
    [Header("Body Panel")]
    [SerializeField] private Button   bodyPrevButton;
    [SerializeField] private Button   bodyNextButton;
    [SerializeField] private Button[] skinColorButtons;

    // ------------------------------------------------------------------
    // Hair panel controls
    // ------------------------------------------------------------------
    [Header("Hair Panel")]
    [SerializeField] private Button   hairPrevButton;
    [SerializeField] private Button   hairNextButton;
    [SerializeField] private Button[] hairColorButtons;

    // ------------------------------------------------------------------
    // Outfit panel controls
    // ------------------------------------------------------------------
    [Header("Outfit Panel")]
    [SerializeField] private Button outfitPrevButton;
    [SerializeField] private Button outfitNextButton;

    // ------------------------------------------------------------------
    // Accessory panel controls
    // ------------------------------------------------------------------
    [Header("Accessory Panel")]
    [SerializeField] private Toggle hatToggle;
    [SerializeField] private Toggle glassesToggle;
    [SerializeField] private Toggle bagToggle;

    // ------------------------------------------------------------------
    // Start button
    // ------------------------------------------------------------------
    [Header("Navigation")]
    [SerializeField] private Button startButton;

    // ------------------------------------------------------------------
    // Internal state
    // ------------------------------------------------------------------
    private AvatarData _data;

    // ===================================================================
    // Unity lifecycle
    // ===================================================================

    private void Start()
    {
        _data = GameManager.Instance.AvatarData;

        WireTabButtons();
        WireBodyPanel();
        WireHairPanel();
        WireOutfitPanel();
        WireAccessoryPanel();

        startButton.onClick.AddListener(OnStartClicked);

        // Apply whatever was already saved (defaults or returning from world)
        previewer.Apply(_data);

        // Start on the Body tab
        ShowPanel(bodyPanel);

        StartCoroutine(fadeScreen.FadeIn());
    }

    // ===================================================================
    // Wiring helpers
    // ===================================================================

    private void WireTabButtons()
    {
        bodyTabButton.onClick.AddListener(()      => ShowPanel(bodyPanel));
        hairTabButton.onClick.AddListener(()      => ShowPanel(hairPanel));
        outfitTabButton.onClick.AddListener(()    => ShowPanel(outfitPanel));
        accessoryTabButton.onClick.AddListener(() => ShowPanel(accessoryPanel));
    }

    private void WireBodyPanel()
    {
        bodyPrevButton.onClick.AddListener(() => CycleBody(-1));
        bodyNextButton.onClick.AddListener(() => CycleBody(1));

        for (int i = 0; i < skinColorButtons.Length; i++)
        {
            int idx = i;   // capture by value
            skinColorButtons[i].onClick.AddListener(() => SetSkinColor(idx));
        }
    }

    private void WireHairPanel()
    {
        hairPrevButton.onClick.AddListener(() => CycleHair(-1));
        hairNextButton.onClick.AddListener(() => CycleHair(1));

        for (int i = 0; i < hairColorButtons.Length; i++)
        {
            int idx = i;
            hairColorButtons[i].onClick.AddListener(() => SetHairColor(idx));
        }
    }

    private void WireOutfitPanel()
    {
        outfitPrevButton.onClick.AddListener(() => CycleOutfit(-1));
        outfitNextButton.onClick.AddListener(() => CycleOutfit(1));
    }

    private void WireAccessoryPanel()
    {
        hatToggle.onValueChanged.AddListener(val     => SetAccessory(0, val));
        glassesToggle.onValueChanged.AddListener(val => SetAccessory(1, val));
        bagToggle.onValueChanged.AddListener(val     => SetAccessory(2, val));

        // Reflect current state
        hatToggle.SetIsOnWithoutNotify(_data.accessories.Length > 0 && _data.accessories[0]);
        glassesToggle.SetIsOnWithoutNotify(_data.accessories.Length > 1 && _data.accessories[1]);
        bagToggle.SetIsOnWithoutNotify(_data.accessories.Length > 2 && _data.accessories[2]);
    }

    // ===================================================================
    // Panel management
    // ===================================================================

    private void ShowPanel(GameObject active)
    {
        bodyPanel.SetActive(false);
        hairPanel.SetActive(false);
        outfitPanel.SetActive(false);
        accessoryPanel.SetActive(false);
        active.SetActive(true);
    }

    // ===================================================================
    // Data mutations — update _data AND live preview simultaneously
    // ===================================================================

    private void CycleBody(int dir)
    {
        _data.bodyIndex = Wrap(_data.bodyIndex + dir, previewer.BodyCount);
        previewer.SetBody(_data.bodyIndex);
    }

    private void SetSkinColor(int idx)
    {
        _data.skinColorIndex = idx;
        previewer.SetSkinColor(idx);
    }

    private void CycleHair(int dir)
    {
        _data.hairIndex = Wrap(_data.hairIndex + dir, previewer.HairCount);
        previewer.SetHair(_data.hairIndex);
    }

    private void SetHairColor(int idx)
    {
        _data.hairColorIndex = idx;
        previewer.SetHairColor(idx);
    }

    private void CycleOutfit(int dir)
    {
        _data.outfitIndex = Wrap(_data.outfitIndex + dir, previewer.OutfitCount);
        previewer.SetOutfit(_data.outfitIndex);
    }

    private void SetAccessory(int slot, bool active)
    {
        if (slot < _data.accessories.Length)
            _data.accessories[slot] = active;
        previewer.SetAccessory(slot, active);
    }

    private void OnStartClicked()
    {
        startButton.interactable = false;
        fadeScreen.FadeToScene("GameWorld");
    }

    // ===================================================================
    // Utility
    // ===================================================================

    /// <summary>Modulo that handles negative values correctly.</summary>
    private static int Wrap(int value, int count)
    {
        return ((value % count) + count) % count;
    }
}
