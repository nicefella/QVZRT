using UnityEngine;

/// <summary>
/// Plain data class that stores all avatar customisation choices.
/// Held by GameManager and survives scene loads.
/// </summary>
[System.Serializable]
public class AvatarData
{
    // Body
    public int bodyIndex      = 0;
    public int skinColorIndex = 0;

    // Hair
    public int hairIndex      = 0;
    public int hairColorIndex = 0;

    // Outfit
    public int outfitIndex    = 0;

    // Accessories: [0] hat, [1] glasses, [2] bag
    public bool[] accessories = new bool[3];
}
