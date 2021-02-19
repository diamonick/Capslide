using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Game Manager Singleton
    public static GameManager Instance { get; private set; }

    [Header("Game Manager")]
    [SerializeField] private GameObject startupMenu;
    [SerializeField] private GameObject settingsMenu;

    [Header("Game Info"), Space(8)]
    public Palette mainPalette;
    public int tokens;

    // Settings Variables
    [Header("Settings"), Space(8)]
    public bool screenShake;
    public bool powerSaving;
    [SerializeField] private Button bgmButton;
    [SerializeField] private Button sfxButton;
    [SerializeField] private Button screenShakeButton;
    [SerializeField] private Button powerSavingButton;
    private readonly Vector2 toggleOffPos = new Vector2(-54f, 0f);
    private readonly Vector2 toggleOnPos = new Vector2(54f, 0f);

    private void Awake()
    {
        Application.targetFrameRate = 60;
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

    /// <summary>
    /// Go to a specified menu screen.
    /// </summary>
    public void GoToMenuScreen(GameObject menuScreen)
    {
        menuScreen.SetActive(false);
        startupMenu.SetActive(true);
    }
    /// <summary>
    /// Go to Settings menu.
    /// </summary>
    public void GoToSettings()
    {
        startupMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    /// <summary>
    /// Toggle BGM (Background Music) setting.
    /// </summary>
    public void ToggleBGM()
    {
        ShiftButton(bgmButton, screenShake);
    }

    /// <summary>
    /// Toggle SFX (Sound Effects) setting.
    /// </summary>
    public void ToggleSFX()
    {
        ShiftButton(sfxButton, screenShake);
    }

    /// <summary>
    /// Toggle Screen Shake setting.
    /// </summary>
    public void ToggleScreenShake()
    {
        screenShake = !screenShake;

        // Turn Screen Shake: ON
        if (screenShake)
        {

        }
        // Turn Screen Shake: OFF
        else
        {

        }
        ShiftButton(screenShakeButton, screenShake);
    }

    /// <summary>
    /// Toggle Power Saving setting
    /// </summary>
    public void TogglePowerSaving()
    {
        powerSaving = !powerSaving;

        // Turn Power Saving: ON
        if (powerSaving)
        {
            Application.targetFrameRate = 30;
        }
        // Turn Power Saving: OFF
        else
        {
            Application.targetFrameRate = 60;
        }
        ShiftButton(powerSavingButton, powerSaving);
    }

    /// <summary>
    /// Shifts the button left or right.
    /// </summary>
    private void ShiftButton(Button button, bool isToggled)
    {
        if (isToggled)
            StartCoroutine(Ease.AnchoredTranslateTo(button.image, toggleOnPos, 0.5f, 2, Easing.EaseOut));
        else
            StartCoroutine(Ease.AnchoredTranslateTo(button.image, toggleOffPos, 0.5f, 2, Easing.EaseOut));
    }
    
    // Color Getter methods
    public Color GetColor(int index) => mainPalette.colors[index];
    public Color GetScoreColor() => mainPalette.scoreColor;
    public Color GetCapsuleTimerColor() => mainPalette.capsuleTimerColor;
    public Color GetBackButtonColor() => mainPalette.backButtonColor;
    public Color GetRetryButtonColor() => mainPalette.retryButtonColor;
    public Color GetRewardedAdButtonColor() => mainPalette.rewardedAdButtonColor;
}
