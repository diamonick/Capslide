using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Game Manager Singleton
    public static GameManager Instance { get; private set; }

    [Header("Game Info"), Space(8)]
    public Palette mainPalette;
    public int tokens;


    // Settings Variables
    public bool screenShakeON;

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

    public Color GetColor(int index) => mainPalette.colors[index];
    public Color GetScoreColor() => mainPalette.scoreColor;
    public Color GetCapsuleTimerColor() => mainPalette.capsuleTimerColor;
}
