using UnityEngine;

/// <summary>
/// Applies an AvatarData snapshot to the avatar GameObject.
///
/// Setup in the Inspector:
///   hairStyles[]   — assign the child hair GameObjects (Hair_0, Hair_1, etc.)
///   outfits[]      — assign the child outfit GameObjects (Outfit_0, Outfit_1, etc.)
///   accessories[]  — [0]=Hat  [1]=Glasses  [2]=Bag child GameObjects
///
/// The script looks for a child named "Body" to change the skin colour material.
/// </summary>
public class AvatarCustomizer : MonoBehaviour
{
    [Header("Hair Style GameObjects (children of this avatar)")]
    public GameObject[] hairStyles;

    [Header("Outfit GameObjects (children of this avatar)")]
    public GameObject[] outfits;

    [Header("Accessory GameObjects — index 0:Hat, 1:Glasses, 2:Bag")]
    public GameObject[] accessories;

    [Header("Skin Colours")]
    public Color[] skinColors = new Color[]
    {
        new Color(1.00f, 0.87f, 0.75f),
        new Color(0.95f, 0.75f, 0.55f),
        new Color(0.75f, 0.55f, 0.35f),
        new Color(0.50f, 0.33f, 0.17f),
        new Color(0.30f, 0.18f, 0.08f)
    };

    [Header("Hair Colours")]
    public Color[] hairColors = new Color[]
    {
        Color.black,
        new Color(0.55f, 0.35f, 0.10f),
        new Color(0.95f, 0.85f, 0.45f),
        Color.white,
        new Color(0.80f, 0.15f, 0.15f),
        new Color(0.60f, 0.20f, 0.80f)
    };

    private Renderer bodyRenderer;

    void Awake()
    {
        var bodyGO = transform.Find("Body");
        if (bodyGO != null)
            bodyRenderer = bodyGO.GetComponent<Renderer>();
    }

    void Start()
    {
        // Apply saved data when entering the GameWorld
        if (GameManager.Instance != null)
            ApplyData(GameManager.Instance.AvatarData);
    }

    /// <summary>Apply all choices from an AvatarData snapshot.</summary>
    public void ApplyData(AvatarData data)
    {
        if (data == null) return;

        // Hair style
        for (int i = 0; i < hairStyles.Length; i++)
            if (hairStyles[i] != null)
                hairStyles[i].SetActive(i == data.hairIndex);

        // Hair colour
        if (data.hairIndex < hairStyles.Length && hairStyles[data.hairIndex] != null)
        {
            var r = hairStyles[data.hairIndex].GetComponent<Renderer>();
            if (r != null && data.hairColorIndex < hairColors.Length)
                r.material.color = hairColors[data.hairColorIndex];
        }

        // Outfit
        for (int i = 0; i < outfits.Length; i++)
            if (outfits[i] != null)
                outfits[i].SetActive(i == data.outfitIndex);

        // Accessories
        for (int i = 0; i < accessories.Length; i++)
            if (accessories[i] != null && i < data.accessoryFlags.Length)
                accessories[i].SetActive(data.accessoryFlags[i]);

        // Skin colour
        if (bodyRenderer != null && data.skinColorIndex < skinColors.Length)
            bodyRenderer.material.color = skinColors[data.skinColorIndex];
    }
}
