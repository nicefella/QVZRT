using UnityEngine;

/// <summary>
/// Applies AvatarData choices (body, hair, outfit, accessories) to the
/// GameObjects / Materials that make up the avatar model.
///
/// Attach to the root of the avatar prefab.
/// Wire all inspector arrays in the Unity editor.
///
/// Anatomy expected on the avatar prefab:
///
///   AvatarRoot  ← this script
///     Body_0, Body_1, Body_2      ← bodyMeshes  (only one active at a time)
///     Hair_0, Hair_1, Hair_2      ← hairStyles  (only one active at a time)
///     Outfit_0, Outfit_1, ...     ← outfits     (only one active at a time)
///     Hat                         ← accessory slot 0
///     Glasses                     ← accessory slot 1
///     Bag                         ← accessory slot 2
///
/// The Renderer on each body mesh is expected to expose a material
/// whose colour represents skin tone (skinMaterialIndex selects which
/// material slot on the Renderer).
///
/// Each hair GameObject should have a Renderer — they are all listed in
/// hairRenderers (order must match hairStyles order).
/// </summary>
public class AvatarCustomizer : MonoBehaviour
{
    [Header("Body")]
    [SerializeField] private GameObject[] bodyMeshes;
    [SerializeField] private Color[]      skinColors;
    [SerializeField] private Renderer     skinRenderer;
    [Tooltip("Which material slot on skinRenderer holds the skin colour.")]
    [SerializeField] private int          skinMaterialIndex = 0;

    [Header("Hair")]
    [SerializeField] private GameObject[] hairStyles;
    [SerializeField] private Color[]      hairColors;
    [Tooltip("One Renderer per hair style (same order as hairStyles).")]
    [SerializeField] private Renderer[]   hairRenderers;

    [Header("Outfits")]
    [SerializeField] private GameObject[] outfits;

    [Header("Accessories")]
    [SerializeField] private GameObject hatObject;
    [SerializeField] private GameObject glassesObject;
    [SerializeField] private GameObject bagObject;

    // -----------------------------------------------------------------------
    // Public counts — read by AvatarCreationUI for prev/next cycling
    // -----------------------------------------------------------------------

    public int BodyCount      => bodyMeshes.Length;
    public int SkinColorCount => skinColors.Length;
    public int HairCount      => hairStyles.Length;
    public int HairColorCount => hairColors.Length;
    public int OutfitCount    => outfits.Length;

    // -----------------------------------------------------------------------
    // Apply a full AvatarData snapshot
    // -----------------------------------------------------------------------

    /// <summary>Apply all fields from an AvatarData object at once.</summary>
    public void Apply(AvatarData data)
    {
        SetBody(data.bodyIndex);
        SetSkinColor(data.skinColorIndex);
        SetHair(data.hairIndex);
        SetHairColor(data.hairColorIndex);
        SetOutfit(data.outfitIndex);
        SetAccessory(0, data.accessories.Length > 0 && data.accessories[0]);
        SetAccessory(1, data.accessories.Length > 1 && data.accessories[1]);
        SetAccessory(2, data.accessories.Length > 2 && data.accessories[2]);
    }

    // -----------------------------------------------------------------------
    // Individual setters — called live from AvatarCreationUI
    // -----------------------------------------------------------------------

    public void SetBody(int index)
    {
        for (int i = 0; i < bodyMeshes.Length; i++)
            if (bodyMeshes[i] != null)
                bodyMeshes[i].SetActive(i == index);
    }

    public void SetSkinColor(int index)
    {
        if (skinRenderer == null || index < 0 || index >= skinColors.Length) return;
        Material[] mats = skinRenderer.materials;
        if (skinMaterialIndex >= mats.Length) return;
        mats[skinMaterialIndex].color = skinColors[index];
        skinRenderer.materials = mats;
    }

    public void SetHair(int index)
    {
        for (int i = 0; i < hairStyles.Length; i++)
            if (hairStyles[i] != null)
                hairStyles[i].SetActive(i == index);
    }

    public void SetHairColor(int index)
    {
        if (index < 0 || index >= hairColors.Length) return;
        foreach (Renderer r in hairRenderers)
        {
            if (r == null) continue;
            Material[] mats = r.materials;
            for (int m = 0; m < mats.Length; m++)
                mats[m].color = hairColors[index];
            r.materials = mats;
        }
    }

    public void SetOutfit(int index)
    {
        for (int i = 0; i < outfits.Length; i++)
            if (outfits[i] != null)
                outfits[i].SetActive(i == index);
    }

    /// <summary>
    /// Toggle an accessory slot.
    /// slot 0 = hat, 1 = glasses, 2 = bag.
    /// </summary>
    public void SetAccessory(int slot, bool active)
    {
        switch (slot)
        {
            case 0: if (hatObject)     hatObject.SetActive(active);     break;
            case 1: if (glassesObject) glassesObject.SetActive(active); break;
            case 2: if (bagObject)     bagObject.SetActive(active);     break;
        }
    }
}
