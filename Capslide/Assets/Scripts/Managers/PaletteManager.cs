using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaletteManager : MonoBehaviour
{
    // Palette Manager Singleton
    public static PaletteManager Instance { get; private set; }

    public Palette mainPalette;
    public PaletteButton[] palettes;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
            Debug.LogWarning($"WARNING: There can only be one instance of this class.");
        }
    }

    public void Equip(PaletteButton pb)
    {
        if (pb.isEquipped || !pb.isUnlocked)
            return;

        foreach (PaletteButton paletteButton in palettes)
            if (paletteButton.isEquipped)
                paletteButton.Unequip();

        pb.Equip();
    }

    // Color Getter methods
    public Color GetColor(int index) => mainPalette.colors[index];
    public Color GetScoreColor() => mainPalette.scoreColor;
    public Color GetCapsuleTimerColor() => mainPalette.capsuleTimerColor;
    public Color GetBackButtonColor() => mainPalette.backButtonColor;
    public Color GetRetryButtonColor() => mainPalette.retryButtonColor;
    public Color GetRewardedAdButtonColor() => mainPalette.rewardedAdButtonColor;
    public Color GetTokenColor() => mainPalette.tokenColor;
}
