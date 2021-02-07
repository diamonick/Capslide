using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="New Palette", menuName="Color Palette")]
public class Palette : ScriptableObject
{
    public int ID;
    public string paletteName;
    public int tokenCost;
    public Color[] colors = new Color[5];
    public Color[] possibleCapsuleColors;

    [Header("UI"), Space(8)]
    public Color scoreColor;
    public Color capsuleTimerColor;
}
