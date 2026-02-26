using System;

/// <summary>
/// Holds all avatar customisation choices. Passed through scenes via GameManager.
/// </summary>
[Serializable]
public class AvatarData
{
    public int bodyIndex       = 0;   // 0 = Slim, 1 = Regular
    public int skinColorIndex  = 0;   // index into AvatarCustomizer.skinColors
    public int hairIndex       = 0;   // which hair style GameObject to show
    public int hairColorIndex  = 0;   // index into AvatarCustomizer.hairColors
    public int outfitIndex     = 0;   // which outfit GameObject to show
    public bool[] accessoryFlags = new bool[3]; // [0]=Hat, [1]=Glasses, [2]=Bag
}
