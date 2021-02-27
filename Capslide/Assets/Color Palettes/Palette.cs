using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
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

    [Header("Results Screen"), Space(8)]
    public Color backButtonColor;
    public Color retryButtonColor;
    public Color rewardedAdButtonColor;
}
