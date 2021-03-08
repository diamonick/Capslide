using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameplayManager : MonoBehaviour
{
    // Gameplay Manager Singleton
    public static GameplayManager Instance { get; private set; }

    //Constants
    private const int CAPSULE_TOTAL = 15;
    private const int FAKE_CAPSULE_TOTAL = 10;
    private const float TOKEN_SPAWN_INTERVAL = 20f;
    private const float COUNTDOWN_TIME = 3f;
    private const float FAKE_SPAWN_TIME = 8f;
    private const float FAKE_FAST_SPAWN_TIME = 4f;

    [Header("Properties"), Space(8)]
    [SerializeField] private GameObject startupMenu;
    [SerializeField] private Menu gameplayMenu;
    [SerializeField] private GameObject gameplayCanvas;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject pauseAssets;
    [SerializeField] private int score;
    [SerializeField] public int tokensEarned;
    [SerializeField] private int capsuleDispenserCount;
    [SerializeField] private float timeUntilDispense;
    [SerializeField] private float timeUntilDispenseFake;
    [SerializeField] private GameObject capsuleDispenser;
    [SerializeField] private Capsule[] capsules;
    [SerializeField] private Capsule[] fakeCapsules;
    [SerializeField] private bool gameOver = false;
    [SerializeField] private GameObject rightWall;
    [SerializeField] private GameObject leftWall;
    [SerializeField] private Transform capsuleOrigin;
    public bool onDrag = false;
    private List<Color> capsuleColors = new List<Color>();
    private bool gameStarted = false;
    private bool finalStretch = false;

    [Header("Countdown"), Space(8)]
    [SerializeField] private GameObject countdownCanvas;
    [SerializeField] private float timeUntilStartGame;
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private Image countdownFillTimer;
    public bool timeStopped = false;

    [Header("UI"), Space(8)]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text capsuleCountText;
    [SerializeField] private GameObject dragCanvas;
    [SerializeField] private Image dragTimeBar;
    private float fullTime;
    [SerializeField] private Image fillTimer;
    public Image overlay;

    [Header("Results Screen"), Space(8)]
    [SerializeField] private GameObject resultsScreen;
    [SerializeField] private GameObject resultsButtons;
    [SerializeField] private GameObject starIcon;
    [SerializeField] private TMP_Text totalScoreText;
    [SerializeField] private TMP_Text bestScoreText;
    [SerializeField] private GameObject tokenCanvas;
    [SerializeField] private TMP_Text tokenText;
    [SerializeField] private Menu resultsMenu;

    [Header("Token"), Space(8)]
    [SerializeField] private GameObject token;
    [SerializeField] private float timeUntilSpawnToken = TOKEN_SPAWN_INTERVAL;
    private Level level;

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

    // Start is called before the first frame update
    void OnEnable()
    {
        ResetLevel();
    }

    // Update is called once per frame
    void Update()
    {
        CapsuleDispenserTimer();
        TokenSpawnTimer();
        FakeCapsuleDispenserTimer();
    }

    /// <summary>
    /// Sets a coundown timer before starting the game.
    /// </summary>
    public IEnumerator CountdownTimer()
    {
        countdownCanvas.SetActive(true);
        ResetTokenSpawnTimer();
        ForegroundOverlay.Instance.FadeInForeground(0f);
        timeUntilStartGame = COUNTDOWN_TIME;

        while (timeUntilStartGame > 0f)
        {
            timeUntilStartGame -= Time.deltaTime;
            timeUntilStartGame = Mathf.Max(0f, timeUntilStartGame);
            countdownText.text = $"{(int)timeUntilStartGame + 1}";
            countdownFillTimer.fillAmount = (timeUntilStartGame % 1f);

            if ((timeUntilStartGame % 1f) < 0.05f && timeUntilStartGame < 3f)
                AudioManager.Instance.PlaySFX("Tick");
            yield return new WaitForEndOfFrame();
        }
        level = GameManager.Instance.currentLevel;
        level.gameObject.SetActive(true);
        countdownCanvas.SetActive(false);
        gameplayCanvas.SetActive(true);
        StartGame();
        ResetManager();
        AudioManager.Instance.PlayMusic("Main Theme", 0.5f);
        yield return new WaitForSeconds(1f);

        DispenseCapsule();
    }

    /// <summary>
    /// Sets a timer for dispensing capsules.
    /// </summary>
    private void CapsuleDispenserTimer()
    {
        if (GameOver() || DispenserIsEmpty() || timeStopped)
            return;

        fillTimer.fillAmount = (timeUntilDispense / fullTime);

        if (timeUntilDispense > 0f)
        {
            timeUntilDispense -= Time.deltaTime;
            timeUntilDispense = Mathf.Max(0f, timeUntilDispense);
        }
        else
            DispenseCapsule(true);
    }

    /// <summary>
    /// Sets a timer for spawning tokens.
    /// </summary>
    private void TokenSpawnTimer()
    {
        if (GameOver())
            return;

        if (timeUntilSpawnToken > 0f)
        {
            timeUntilSpawnToken -= Time.deltaTime;
            timeUntilSpawnToken = Mathf.Max(0f, timeUntilSpawnToken);
        }
        else
            SpawnToken();
    }

    public void DragOn(float dragTime, float interval)
    {
        if (!dragCanvas.gameObject.activeSelf)
            dragCanvas.gameObject.SetActive(true);

        dragTimeBar.fillAmount = dragTime / interval;
    }

    public void DragOff()
    {
        dragTimeBar.fillAmount = 0f;
        dragCanvas.gameObject.SetActive(false);
    }

    /// <summary>
    /// Sets a timer for dispensing fake capsules. Time is based on how long you hold the slider(s).
    /// </summary>
    public void FakeCapsuleDispenserTimer()
    {
        if (GameOver() || !onDrag || (NoCapsulesInGame() && capsuleDispenserCount == 0))
            return;

        if (timeUntilDispenseFake > 0f)
        {
            timeUntilDispenseFake -= Time.deltaTime;
            timeUntilDispenseFake = Mathf.Max(0f, timeUntilDispenseFake);
        }
        else
            DispenseFakeCapsule();
    }
    /// <summary>
    /// Dispense fake capsule from the top screen.
    /// </summary>
    public void DispenseFakeCapsule()
    {
        if (GameOver())
            return;

        foreach (Capsule capsule in fakeCapsules)
        {
            if (!capsule.gameObject.activeSelf)
            {
                capsule.gameObject.SetActive(true);
                capsule.transform.position = capsuleOrigin.position;
                capsule.FakeCapsule();
                break;
            }
        }

        SetFakeSpawnTimer();
    }

    /// <summary>
    /// Dispense capsule from the top screen.
    /// </summary>
    public void DispenseCapsule(bool isCompleted = false)
    {
        if (GameOver())
            return;

        do
        {
            Capsule newCapsule = capsules[GetExistingCapsules()];
            newCapsule.gameObject.SetActive(true);

            if (isCompleted && capsuleDispenserCount > 2)
                newCapsule.GetComponent<Capsule>().StarCapsule();

            capsuleDispenserCount--;
        }
        while (LastThreeCapsules());

        if (capsuleDispenserCount == 5 && !finalStretch)
        {
            AudioManager.Instance.PlayMusic("Main Theme", 0.5f, 1.2f);
            finalStretch = true;
        }

        timeStopped = false;
        timeUntilDispense = (DispenserIsEmpty() ? 0f : 20f);
        fillTimer.fillAmount = (DispenserIsEmpty() ? 0f : (timeUntilDispense / fullTime));
        fullTime = timeUntilDispense;
        capsuleCountText.text = $"{capsuleDispenserCount}";
    }

    /// <summary>
    /// Spawn token at a random location.
    /// </summary>
    public void SpawnToken()
    {
        Transform[] spawnLocations = level.spawnLocations;
        int index = Random.Range(0, spawnLocations.Length);

        token.SetActive(true);
        token.transform.position = spawnLocations[index].position;
        timeUntilSpawnToken = TOKEN_SPAWN_INTERVAL;
    }

    /// <summary>
    /// Reset timer for spawning token.
    /// </summary>
    public void ResetTokenSpawnTimer() => timeUntilSpawnToken = TOKEN_SPAWN_INTERVAL;

    /// <summary>
    /// End the game.
    /// </summary>
    public void EndGame()
    {
        gameStarted = false;
        gameOver = true;
        StartCoroutine(ShowResults());
    }

    public IEnumerator ShowResults()
    {
        float rightX = 1594f;
        float leftX = -766f;
        float xTo = Screen.width / 2f;
        Color fullColor = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 1f);

        yield return new WaitForSeconds(1f);
        StartCoroutine(Ease.TranslateXTo(rightWall, rightX, 0.5f, 2, Easing.EaseOut));
        yield return StartCoroutine(Ease.TranslateXTo(leftWall, leftX, 0.5f, 2, Easing.EaseOut));
        yield return StartCoroutine(Ease.ChangeImageColor(overlay, fullColor, 1f));

        ForegroundOverlay.Instance.FadeOutForeground(0f);
        GameManager.Instance.currentLevel.gameObject.SetActive(false);
        token.SetActive(false);
        pauseMenu.SetActive(false);
        gameplayCanvas.SetActive(false);
        countdownCanvas.SetActive(false);
        resultsScreen.SetActive(true);
        foreach (GameObject resultCanvas in resultsMenu.menuCanvases)
            resultCanvas.transform.position = new Vector3(xTo, resultCanvas.transform.position.y, 0f);
        yield return StartCoroutine(SetupResults());

        resultsButtons.SetActive(true);
    }

    public IEnumerator SetupResults()
    {
        int totalScore = 0;
        int tokenCount = tokensEarned;
        yield return new WaitForSeconds(1f);

        while (totalScore < score)
        {
            AddScorePerFrame(ref totalScore);
            totalScoreText.text = $"{totalScore}";
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(0.5f);

        SaveHighscore();
        yield return new WaitForSeconds(0.5f);

        tokenCanvas.SetActive(true);
        tokenText.text = $"You got {tokenCount}";
        GameManager.Instance.tokens += tokensEarned;
        GameManager.Instance.levelsPlayed++;
        yield return new WaitForSeconds(1f);

        GameManager.Instance.Save();
    }

    private void SaveHighscore()
    {
        int trueScore = GameManager.Instance.levelHighscores[level.ID];
        if (score > trueScore)
        {
            starIcon.SetActive(true);
            GameManager.Instance.levelHighscores[level.ID] = score;
            trueScore = score;
        }

        bestScoreText.gameObject.SetActive(true);
        bestScoreText.text = $"Best: {trueScore}";
    }

    private void AddScorePerFrame(ref int currentScore)
    {
        int pointsToAdd = GetRate(currentScore);
        currentScore += pointsToAdd;
        currentScore = Mathf.Min(currentScore, score);
    }
    private int GetRate(int currentScore)
    {
        if (currentScore < 100)
            return 2;
        else if (currentScore < 500)
            return 4;
        else if (currentScore < 1000)
            return 8;
        else if (currentScore < 2500)
            return 16;
        else
            return 32;
    }

    /// <summary>
    /// Adds points to your score.
    /// </summary>
    /// <param name="s">The amount of points to add.</param>
    public void SetScore(int s)
    {
        score += s;
        score = Mathf.Max(0, score);
        scoreText.text = $"{score}";
    }

    /// <summary>
    /// Toggles when to enter/exit the Pause Menu.
    /// </summary>
    public void TogglePause()
    {
        if (!gameStarted)
            return;

        if (GetCapsulesInGame() <= 1 && DispenserIsEmpty() && timeStopped && GetFakeCapsulesInGame() <= 1)
            return;

        if (Time.timeScale == 1f)
            PauseGame();
        else
            ResumeGame();
    }

    /// <summary>
    /// Pauses the game.
    /// </summary>
    private void PauseGame()
    {
        Time.timeScale = 0f;
        pauseAssets.SetActive(true);
        gameplayCanvas.gameObject.SetActive(false);
    }

    /// <summary>
    /// Resumes the game.
    /// </summary>
    private void ResumeGame(bool trueResume = true)
    {
        if (trueResume)
            gameplayCanvas.gameObject.SetActive(true);

        Time.timeScale = 1f;
        pauseAssets.SetActive(false);
    }

    public void ResetMainVariables()
    {
        gameStarted = false;
        gameOver = false;
        onDrag = false;
        timeStopped = true;
        fillTimer.fillAmount = 1f;

        totalScoreText.text = "0";
        score = 0;
        tokensEarned = 0;
        capsuleDispenserCount = CAPSULE_TOTAL;
        timeUntilDispense = 20f;
        timeUntilDispenseFake = FAKE_SPAWN_TIME;

        if (level != null)
            level.gameObject.SetActive(false);

        finalStretch = false;
        starIcon.SetActive(false);
        token.SetActive(false);
        bestScoreText.gameObject.SetActive(false);
        tokenCanvas.SetActive(false);
        resultsButtons.SetActive(false);
        resultsScreen.SetActive(false);
        dragCanvas.gameObject.SetActive(false);
        DeactivateCapsules();
        DeactivateFakeCapsules();
    }

    /// <summary>
    /// Resets the current level.
    /// </summary>
    public void ResetLevel()
    {
        ResumeGame(false);
        ResetMainVariables();
        pauseMenu.SetActive(true);
        AudioManager.Instance.Stop("Main Theme");
        StartCoroutine(CountdownTimer());
    }

    /// <summary>
    /// Go to Startup Menu from Results screen
    /// </summary>
    public void GoToStartupMenu(GameObject menuScreen)
    {
        ResumeGame(false);
        ResetMainVariables();
        GameManager.Instance.currentLevel.gameObject.SetActive(false);
        GameManager.Instance.GoToStartupMenu(gameplayMenu);
    }

    /// <summary>
    /// Resets the Gameplay Manager's variables.
    /// </summary>
    public void ResetManager()
    {
        DeactivateCapsules();
        DeactivateFakeCapsules();
        SetCapsuleColors();
        DeactivateResultsScreen();
        rightWall.transform.position = new Vector3(1234f, rightWall.transform.position.y, rightWall.transform.position.z);
        leftWall.transform.position = new Vector3(-406f, leftWall.transform.position.y, leftWall.transform.position.z);
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 0f);
        scoreText.text = $"{score}";
        capsuleCountText.text = $"{capsuleDispenserCount}";
    }

    private int GetExistingCapsules() => CAPSULE_TOTAL - capsuleDispenserCount;

    /// <summary>
    /// Checks if there are currently no capsules in the level.
    /// </summary>
    /// <returns>TRUE if there are no capsules in the level. Otherwise, FALSE.</returns>
    public bool NoCapsulesInGame()
    {
        int remainingCapsules = GetCapsulesInGame();
        return remainingCapsules == 0;
    }

    /// <summary>
    /// Get the number of capsules currently visible in the game.
    /// </summary>
    public int GetCapsulesInGame()
    {
        int remainingCapsules = 0;
        for (int i = 0; i < CAPSULE_TOTAL; i++)
        {
            if (!capsules[i].gameObject.activeSelf)
                continue;
            remainingCapsules++;
        }

        return remainingCapsules;
    }

    /// <summary>
    /// Get the number of fake capsules currently visible in the game.
    /// </summary>
    public int GetFakeCapsulesInGame()
    {
        int remainingCapsules = 0;
        for (int i = 0; i < fakeCapsules.Length; i++)
        {
            if (!fakeCapsules[i].gameObject.activeSelf)
                continue;
            remainingCapsules++;
        }

        return remainingCapsules;
    }

    /// <summary>
    /// Set the Fake Capsule Dispenser Timer based on the number of capsules inthe dispenser.
    /// </summary>
    private void SetFakeSpawnTimer()
    {
        if (finalStretch)
            timeUntilDispenseFake = FAKE_FAST_SPAWN_TIME;
        else
            timeUntilDispenseFake = FAKE_SPAWN_TIME;
    }

    /// <summary>
    /// Checks if capsule dispenser is empty.
    /// </summary>
    /// <returns>TRUE if capsuleDispenserCount is equal to 0. Otherwise, FALSE.</returns>
    public bool DispenserIsEmpty() => capsuleDispenserCount == 0;


    /// <summary>
    /// Checks if there are currently no fake capsules in the level.
    /// </summary>
    /// <returns>TRUE if there are 0 remaining fake capsules in the level. Otherwise, FALSE.</returns>
    public bool NoFakeCapsulesInGame()
    {
        int remainingCapsules = 0;
        for (int i = 0; i < FAKE_CAPSULE_TOTAL; i++)
        {
            if (!fakeCapsules[i].gameObject.activeSelf)
                continue;
            remainingCapsules++;
        }

        return remainingCapsules == 0;
    }

    /// <summary>
    /// Checks if there are 3 capsules remaining in the capsule dispenser.
    /// </summary>
    /// <returns></returns>
    private bool LastThreeCapsules() => capsuleDispenserCount > 0 && capsuleDispenserCount < 3;

    /// <summary>
    /// Checks if current game session is over.
    /// </summary>
    /// <returns>TRUE if game is over. Otherwise, FALSE.</returns>
    public bool GameOver() => gameOver || !gameStarted;

    private void DeactivateCapsules()
    {
        foreach (Capsule capsule in capsules)
        {
            capsule.transform.position = capsuleOrigin.position;
            capsule.gameObject.SetActive(false);
        }
    }

    private void DeactivateFakeCapsules()
    {
        foreach (Capsule capsule in fakeCapsules)
        {
            capsule.transform.position = capsuleOrigin.position;
            capsule.gameObject.SetActive(false);
        }
    }

    private void DeactivateResultsScreen()
    {
        foreach (Capsule capsule in capsules)
            capsule.gameObject.SetActive(false);
    }

    private void StartGame() => gameStarted = true;
    public bool GameStarted() => gameStarted;

    /// <summary>
    /// Sets all the capsules to a random color based on the color palette.
    /// </summary>
    private void SetCapsuleColors()
    {
        int colorCount = PaletteManager.Instance.mainPalette.possibleCapsuleColors.Length;

        capsuleColors.Clear();
        for (int i = 0; i < colorCount; i++)
            capsuleColors.Add(PaletteManager.Instance.mainPalette.possibleCapsuleColors[i]);

        // Set colors for normal capsules
        foreach (Capsule capsule in capsules)
        {
            int rand = Random.Range(0, colorCount);
            capsule.SPR.color = capsuleColors[rand];
        }

        // Set colors for fake capsules
        foreach (Capsule capsule in fakeCapsules)
        {
            int rand = Random.Range(0, colorCount);
            capsule.SPR.color = capsuleColors[rand];
        }
    }
}
