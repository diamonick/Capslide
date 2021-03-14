using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CapslideData
{
    public int paletteID;
    public int tokens;
    public int tokensNeededForTokenPlayer;
    public int[] levelHighscores;

    // Palette
    public bool[] paletteUnlocked;

    // Settings
    public bool bgmToggle;
    public bool sfxToggle;
    public bool screenShakeToggle;
    public bool powerSavingToggle;

    // Miscellaneous
    public int levelsPlayed;

    public CapslideData(GameManager GM)
    {
        paletteID = GM.paletteManager.mainPalette.ID;
        tokens = GM.tokens;
        tokensNeededForTokenPlayer = GM.tokensNeededForTokenPlayer;

        levelHighscores = new int[6];
        for (int i = 0; i < levelHighscores.Length; i++)
            levelHighscores[i] = GM.levelHighscores[i];

        paletteUnlocked = new bool[64];
        for (int i = 0; i < paletteUnlocked.Length; i++)
            paletteUnlocked[i] = GM.palettesUnlocked[i];

        bgmToggle = GM.bgmON;
        sfxToggle = GM.sfxON;
        screenShakeToggle = GM.screenShake;
        powerSavingToggle = GM.powerSaving;

        levelsPlayed = GM.levelsPlayed;
    }
}
