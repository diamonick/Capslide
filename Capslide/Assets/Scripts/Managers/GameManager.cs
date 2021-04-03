using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Game Manager Singleton
    public static GameManager Instance { get; private set; }

    [Header("Game Manager")]
    [SerializeField] private Menu startupMenu;
    [SerializeField] private Menu levelSelectMenu;
    [SerializeField] private Menu paletteMenu;
    [SerializeField] private Menu settingsMenu;
    [SerializeField] private GameObject gameplayMenu;
    [SerializeField] private bool canSelect;

    // Level Select Variables
    [Header("Level Select"), Space(8)]
    public int[] levelHighscores = new int[6];
    [SerializeField] private TMP_Text[] highscoreTexts = new TMP_Text[6];
    [Range(0, 999)]
    public int levelsPlayed;
    public int levelsPlayedUntilDisplayAd;
    [HideInInspector] public Level currentLevel;

    [Header("Game Info"), Space(8)]
    [Range(0,999)]
    public int tokens;
    [Range(0,256)]
    public int tokensNeededForTokenPlayer;

    // Palette Variables
    [Header("Palette")]
    public PaletteManager paletteManager;
    public bool[] palettesUnlocked;
    [SerializeField] private TMP_Text tokenText;

    // Settings Variables
    [Header("Settings"), Space(8)]
    public bool bgmON;
    public bool sfxON;
    public bool screenShake;
    //public bool powerSaving;
    [SerializeField] private Button bgmButton;
    [SerializeField] private Button sfxButton;
    [SerializeField] private Button screenShakeButton;
    //[SerializeField] private Button powerSavingButton;
    [SerializeField] private Image[] toggleFills;
    private readonly Vector2 toggleOffPos = new Vector2(-54f, 0f);
    private readonly Vector2 toggleOnPos = new Vector2(54f, 0f);

    private void Awake()
    {
        // Set target framerate to 60FPS
        Application.targetFrameRate = 60;

        // Disable multiple touch inputs
        Input.multiTouchEnabled = false;

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

    private void OnEnable()
    {
        //SaveSystem.Erase();
        Load();

        if (bgmON)
            AudioManager.Instance.PlayMusic("Main Theme", 0.5f);
    }

    public void Save() => SaveSystem.Save(this);
    public void Load()
    {
        CapslideData data = SaveSystem.Load();

        if (data == null)
            return;

        tokens = data.tokens;
        tokensNeededForTokenPlayer = data.tokensNeededForTokenPlayer;

        for (int i = 0; i < levelHighscores.Length; i++)
            levelHighscores[i] = data.levelHighscores[i];

        for (int i = 0; i < palettesUnlocked.Length; i++)
            palettesUnlocked[i] = data.paletteUnlocked[i];

        paletteManager.mainPalette = paletteManager.paletteList[data.paletteID];

        bgmON = data.bgmToggle;
        sfxON = data.sfxToggle;
        screenShake = data.screenShakeToggle;
        //powerSaving = data.powerSavingToggle;

        levelsPlayed = data.levelsPlayed;
        levelsPlayedUntilDisplayAd = data.levelsPlayedUntilDisplayAd;
    }

    public void LoadPalettesUnlocked()
    {
        for (int i = 0; i < palettesUnlocked.Length; i++)
        {
            paletteManager.palettes[i].isUnlocked = palettesUnlocked[i];
            paletteManager.palettes[i].SetPaletteButton();
        }
    }

    /// <summary>
    /// Go to a specified menu screen.
    /// </summary>
    public void GoToMenuScreen(Menu menuScreen) => StartCoroutine(ShiftMenu(menuScreen, startupMenu));

    /// <summary>
    /// Go to Settings menu.
    /// </summary>
    public void GoToSettings() => StartCoroutine(ShiftMenu(startupMenu, settingsMenu));

    /// <summary>
    /// Go to Palette menu.
    /// </summary>
    public void GoToPalette() => StartCoroutine(ShiftMenu(startupMenu, paletteMenu));

    /// <summary>
    /// Go to Level Select menu.
    /// </summary>
    public void GoToLevelSelect() => StartCoroutine(ShiftMenu(startupMenu, levelSelectMenu));

    /// <summary>
    /// Go to Startup Menu.
    /// </summary>
    public void GoToStartupMenu(Menu menuScreen) => StartCoroutine(ShiftMenu(menuScreen, startupMenu));

    /// <summary>
    /// Go to specified level.
    /// </summary>
    public void GoToLevel(Level level) => StartCoroutine(ShiftToLevel(levelSelectMenu, level));

    private IEnumerator ShiftToLevel(Menu menuSrc, Level level)
    {
        if (!canSelect)
            yield return null;

        float xAway = menuSrc.transform.position.x;
        float xTo = menuSrc.transform.position.x + (Screen.width / 2f);
        canSelect = false;

        AudioManager.Instance.PlaySFX("Click");
        StartCoroutine(AudioManager.Instance.FadeOut(0.01f));
        ForegroundOverlay.Instance.FadeInForeground(0.2f);
        foreach (GameObject menuCanvas in menuSrc.menuCanvases)
            StartCoroutine(Ease.TranslateXTo(menuCanvas, xAway, 0.25f, 2, Easing.EaseOut));
        yield return new WaitForSeconds(0.25f);

        canSelect = true;
        menuSrc.gameObject.SetActive(false);
        gameplayMenu.SetActive(true);
        currentLevel = level;
    }

    private IEnumerator ShiftMenu(Menu menuSrc, Menu menuDest)
    {
        if (!canSelect)
            yield return null;

        float xAway = menuSrc.transform.position.x;
        float xTo = menuSrc.transform.position.x + (Screen.width / 2f);
        canSelect = false;

        AudioManager.Instance.PlaySFX("Click");
        ForegroundOverlay.Instance.FadeInForeground(0.2f);
        foreach (GameObject menuCanvas in menuSrc.menuCanvases)
            StartCoroutine(Ease.TranslateXTo(menuCanvas, xAway, 0.25f, 2, Easing.EaseOut));
        yield return new WaitForSeconds(0.25f);

        foreach (GameObject menuCanvas in menuSrc.menuCanvases)
            StartCoroutine(Ease.TranslateXTo(menuCanvas, xTo, 0f, 2, Easing.EaseOut));
        menuSrc.gameObject.SetActive(false);
        menuDest.gameObject.SetActive(true);

        if (settingsMenu.gameObject.activeSelf)
        {
            SetButtonPosition(bgmButton, toggleFills[0], bgmON);
            SetButtonPosition(sfxButton, toggleFills[1], sfxON);
            SetButtonPosition(screenShakeButton, toggleFills[2], screenShake);
            //SetButtonPosition(powerSavingButton, toggleFills[3], powerSaving);
        }

        if (levelSelectMenu.gameObject.activeSelf)
        {
            for (int i = 0; i < levelHighscores.Length; i++)
                highscoreTexts[i].text = $"{levelHighscores[i]}";
        }
        else if (paletteMenu.gameObject.activeSelf)
        {
            LoadPalettesUnlocked();
            SetTokenText();
        }

        ForegroundOverlay.Instance.FadeOutForeground(0.2f);
        foreach (GameObject menuCanvas in menuDest.menuCanvases)
        {
            menuCanvas.transform.position = new Vector3(xAway, menuCanvas.transform.position.y, menuCanvas.transform.position.z);
            StartCoroutine(Ease.TranslateXTo(menuCanvas, xTo, 0.25f, 2, Easing.EaseOut));
        }
        yield return new WaitForSeconds(0.25f);

        canSelect = true;
    }

    public void SetTokenText() => tokenText.text = $"{tokens}";

    /// <summary>
    /// Toggle BGM (Background Music) setting.
    /// </summary>
    public void ToggleBGM()
    {
        bgmON = !bgmON;
        AudioManager.Instance.ToggleMusic();
        ShiftButton(bgmButton, toggleFills[0], bgmON);
    }

    /// <summary>
    /// Toggle SFX (Sound Effects) setting.
    /// </summary>
    public void ToggleSFX()
    {
        sfxON = !sfxON;
        ShiftButton(sfxButton, toggleFills[1], sfxON);
    }

    /// <summary>
    /// Toggle Screen Shake setting.
    /// </summary>
    public void ToggleScreenShake()
    {
        screenShake = !screenShake;
        ShiftButton(screenShakeButton, toggleFills[2], screenShake);
    }

    /// <summary>
    /// Toggle Power Saving setting
    /// </summary>
    //public void TogglePowerSaving()
    //{
    //    powerSaving = !powerSaving;

    //    // Turn Power Saving: ON
    //    if (powerSaving)
    //        Application.targetFrameRate = 30;
    //    // Turn Power Saving: OFF
    //    else
    //        Application.targetFrameRate = 60;

    //    ShiftButton(powerSavingButton, toggleFills[3], powerSaving);
    //}

    /// <summary>
    /// Shifts the button left or right.
    /// </summary>
    private void ShiftButton(Button button, Image toggleFill, bool isToggled)
    {
        Color fillColor = new Color(toggleFill.color.r, toggleFill.color.g, toggleFill.color.b, 1f);
        Color noFillColor = new Color(toggleFill.color.r, toggleFill.color.g, toggleFill.color.b, 0f);

        if (isToggled)
        {
            StartCoroutine(Ease.ChangeImageColor(toggleFill, fillColor, 0.5f));
            StartCoroutine(Ease.AnchoredTranslateTo(button.image, toggleOnPos, 0.5f, 2, Easing.EaseOut));
        }
        else
        {
            StartCoroutine(Ease.ChangeImageColor(toggleFill, noFillColor, 0.5f));
            StartCoroutine(Ease.AnchoredTranslateTo(button.image, toggleOffPos, 0.5f, 2, Easing.EaseOut));
        }

        Save();
        AudioManager.Instance.PlaySFX("Toggle");
    }

    /// <summary>
    /// Set the button's position.
    /// </summary>
    private void SetButtonPosition(Button button, Image toggleFill, bool isToggled)
    {
        Color fillColor = new Color(toggleFill.color.r, toggleFill.color.g, toggleFill.color.b, 1f);
        Color noFillColor = new Color(toggleFill.color.r, toggleFill.color.g, toggleFill.color.b, 0f);

        if (isToggled)
        {
            toggleFill.color = fillColor;
            button.image.rectTransform.anchoredPosition = toggleOnPos;
        }
        else
        {
            toggleFill.color = noFillColor;
            button.image.rectTransform.anchoredPosition = toggleOffPos;
        }
    }

    /// <summary>
    /// Method to check for any achievements to award to the player.
    /// </summary>
    public void CheckPotentialAwards()
    {
        switch (currentLevel.ID)
        {
            case 0:
                CloudOnceServices.Instance.AwardArenaCleared();
                break;
            case 1:
                CloudOnceServices.Instance.AwardDiamondCleared();
                break;
            case 2:
                CloudOnceServices.Instance.AwardOrbitCleared();
                break;
            case 3:
                CloudOnceServices.Instance.AwardPinballCleared();
                break;
            case 4:
                CloudOnceServices.Instance.AwardWarpCleared();
                break;
            case 5:
                CloudOnceServices.Instance.AwardClockCleared();
                break;
        }

        CloudOnceServices.Instance.AwardTokenPlayer();
        CloudOnceServices.Instance.AwardCapslider();
    }
}
