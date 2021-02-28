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
    [HideInInspector] public GameObject currentLevel;

    [Header("Game Info"), Space(8)]
    public int tokens;

    // Settings Variables
    [Header("Settings"), Space(8)]
    public bool bgmON;
    public bool sfxON;
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

    private void OnEnable()
    {
        SetButtonPosition(bgmButton, bgmON);
        SetButtonPosition(sfxButton, sfxON);
        SetButtonPosition(screenShakeButton, screenShake);
        SetButtonPosition(powerSavingButton, powerSaving);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            EraseData();
    }

    public void EraseData()
    {
        string filePath = $"{Application.persistentDataPath}/CapslideData.dat";
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log("Data erased!");
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
    public void GoToLevel(GameObject level) => StartCoroutine(ShiftToLevel(levelSelectMenu, level));

    private IEnumerator ShiftToLevel(Menu menuSrc, GameObject level)
    {
        if (!canSelect)
            yield return null;

        float xAway = menuSrc.transform.position.x;
        float xTo = menuSrc.transform.position.x + (Screen.width / 2f);
        canSelect = false;

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

        ForegroundOverlay.Instance.FadeInForeground(0.2f);
        foreach (GameObject menuCanvas in menuSrc.menuCanvases)
            StartCoroutine(Ease.TranslateXTo(menuCanvas, xAway, 0.25f, 2, Easing.EaseOut));
        yield return new WaitForSeconds(0.25f);

        menuSrc.gameObject.SetActive(false);
        menuDest.gameObject.SetActive(true);

        if (levelSelectMenu.gameObject.activeSelf)
        {
            for (int i = 0; i < levelHighscores.Length; i++)
                highscoreTexts[i].text = $"{levelHighscores[i]}";
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

    /// <summary>
    /// Toggle BGM (Background Music) setting.
    /// </summary>
    public void ToggleBGM()
    {
        bgmON = !bgmON;
        AudioManager.Instance.ToggleMusic();
        ShiftButton(bgmButton, bgmON);
    }

    /// <summary>
    /// Toggle SFX (Sound Effects) setting.
    /// </summary>
    public void ToggleSFX()
    {
        sfxON = !sfxON;
        ShiftButton(sfxButton, sfxON);
    }

    /// <summary>
    /// Toggle Screen Shake setting.
    /// </summary>
    public void ToggleScreenShake()
    {
        screenShake = !screenShake;
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
            Application.targetFrameRate = 30;
        // Turn Power Saving: OFF
        else
            Application.targetFrameRate = 60;

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

        AudioManager.Instance.PlaySFX("Toggle");
    }

    /// <summary>
    /// Set the button's position.
    /// </summary>
    private void SetButtonPosition(Button button, bool isToggled)
    {
        if (isToggled)
            button.image.rectTransform.anchoredPosition = toggleOnPos;
        else
            button.image.rectTransform.anchoredPosition = toggleOffPos;
    }
}
