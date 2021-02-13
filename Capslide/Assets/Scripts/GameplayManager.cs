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
    private const int CAPSULE_TOTAL = 10;
    private const float TOKEN_SPAWN_INTERVAL = 20f;
    private const float COUNTDOWN_TIME = 3f;

    [Header("Properties"), Space(8)]
    [SerializeField] private GameObject startupMenu;
    [SerializeField] private GameObject gameplayCanvas;
    [SerializeField] private GameObject level;
    [SerializeField] private int score;
    [SerializeField] public int tokensEarned;
    [SerializeField] private int capsuleDispenserCount;
    [SerializeField] private float timeUntilDispense;
    [SerializeField] private GameObject capsuleDispenser;
    [SerializeField] private GameObject[] capsules;
    [SerializeField] private bool gameOver = false;
    [SerializeField] private GameObject rightWall;
    [SerializeField] private GameObject leftWall;
    [SerializeField] private Transform capsuleOrigin;
    private List<Color> capsuleColors = new List<Color>();
    private bool gameStarted = false;

    [Header("Countdown"), Space(8)]
    [SerializeField] private GameObject countdownCanvas;
    [SerializeField] private float timeUntilStartGame;
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private Image countdownFillTimer;


    [Header("UI"), Space(8)]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text capsuleCountText;
    private float fullTime;
    [SerializeField] private Image fillTimer;
    public Image overlay;

    [Header("Results Screen"), Space(8)]
    [SerializeField] private GameObject resultsScreen;
    [SerializeField] private GameObject resultsButtons;
    [SerializeField] private TMP_Text totalScoreText;
    [SerializeField] private TMP_Text bestScoreText;
    [SerializeField] private GameObject tokenCanvas;
    [SerializeField] private TMP_Text tokenText;

    [Header("Token"), Space(8)]
    [SerializeField] private GameObject token;
    [SerializeField] private float timeUntilSpawnToken = TOKEN_SPAWN_INTERVAL;
    [SerializeField] private Transform[] spawnLocations;

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
    void Start()
    {
        ResetLevel();
    }

    // Update is called once per frame
    void Update()
    {
        CapsuleDispenserTimer();
        TokenSpawnTimer();
    }

    /// <summary>
    /// Sets a coundown timer before starting the game.
    /// </summary>
    public IEnumerator CountdownTimer()
    {
        countdownCanvas.SetActive(true);
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 1f);
        timeUntilStartGame = COUNTDOWN_TIME;

        while (timeUntilStartGame > 0f)
        {
            timeUntilStartGame -= Time.deltaTime;
            timeUntilStartGame = Mathf.Max(0f, timeUntilStartGame);
            countdownText.text = $"{(int)timeUntilStartGame + 1}";
            countdownFillTimer.fillAmount = (timeUntilStartGame % 1f);
            yield return new WaitForEndOfFrame();
        }
        countdownCanvas.SetActive(false);
        StartGame();
        ResetManager();
        yield return new WaitForSeconds(1f);

        DispenseCapsule();
    }

    /// <summary>
    /// Sets a timer for dispensing capsules.
    /// </summary>
    private void CapsuleDispenserTimer()
    {
        if (GameOver() || DispenserIsEmpty())
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

    /// <summary>
    /// Dispense capsule from the top screen.
    /// </summary>
    public void DispenseCapsule(bool isCompleted = false)
    {
        if (GameOver())
            return;

        do
        {
            GameObject newCapsule = capsules[GetExistingCapsules()];
            newCapsule.SetActive(true);

            if (isCompleted && capsuleDispenserCount > 4)
                newCapsule.GetComponent<Capsule>().StarCapsule();

            capsuleDispenserCount--;
        }
        while (capsuleDispenserCount > 0 && capsuleDispenserCount < 5);

        timeUntilDispense = 30f;
        fullTime = timeUntilDispense;
        capsuleCountText.text = $"{capsuleDispenserCount}";
    }

    /// <summary>
    /// Spawn token at a random location.
    /// </summary>
    public void SpawnToken()
    {
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
        Color fullColor = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 1f);

        yield return new WaitForSeconds(1f);
        StartCoroutine(Ease.TranslateXTo(rightWall, rightX, 0.5f, 2, Easing.EaseOut));
        yield return StartCoroutine(Ease.TranslateXTo(leftWall, leftX, 0.5f, 2, Easing.EaseOut));
        yield return StartCoroutine(Ease.ChangeImageColor(overlay, fullColor, 1f));

        level.SetActive(false);
        resultsScreen.SetActive(true);
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

        bestScoreText.gameObject.SetActive(true);
        bestScoreText.text = $"Best: {score}";
        yield return new WaitForSeconds(0.5f);

        tokenCanvas.SetActive(true);
        tokenText.text = $"You got {tokenCount}";
        GameManager.Instance.tokens += tokensEarned;
        yield return new WaitForSeconds(1f);
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
            return 1;
        else if (currentScore < 500)
            return 2;
        else if(currentScore < 2000)
            return 4;
        else
            return 8;
    }

    /// <summary>
    /// Adds points to your score.
    /// </summary>
    /// <param name="s">The amount of points to add.</param>
    public void SetScore(int s)
    {
        score += s;
        scoreText.text = $"{score}";
    }

    /// <summary>
    /// Resets the current level.
    /// </summary>
    public void ResetLevel()
    {
        gameOver = false;

        totalScoreText.text = "0";
        score = 0;
        tokensEarned = 0;
        capsuleDispenserCount = CAPSULE_TOTAL;
        timeUntilDispense = 30f;

        bestScoreText.gameObject.SetActive(false);
        tokenCanvas.SetActive(false);
        resultsButtons.SetActive(false);
        resultsScreen.SetActive(false);
        StartCoroutine(CountdownTimer());
    }

    /// <summary>
    /// Go to Startup Menu
    /// </summary>
    public void GoToStartupMenu()
    {
        gameOver = false;

        totalScoreText.text = "0";
        score = 0;
        tokensEarned = 0;
        capsuleDispenserCount = CAPSULE_TOTAL;
        timeUntilDispense = 30f;

        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 0f);
        bestScoreText.gameObject.SetActive(false);
        tokenCanvas.SetActive(false);
        resultsButtons.SetActive(false);
        resultsScreen.SetActive(false);
        level.SetActive(false);
        gameplayCanvas.SetActive(false);
        startupMenu.SetActive(true);
    }

    /// <summary>
    /// Resets the Gameplay Manager's variables.
    /// </summary>
    public void ResetManager()
    {
        DeactivateCapsules();
        SetCapsuleColors();
        DeactivateResultsScreen();
        rightWall.transform.position = new Vector3(1234f, rightWall.transform.position.y, rightWall.transform.position.z);
        leftWall.transform.position = new Vector3(-406f, leftWall.transform.position.y, leftWall.transform.position.z);
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 0f);
        level.SetActive(true);
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
        int remainingCapsules = 0;
        for (int i = 0; i < CAPSULE_TOTAL; i++)
        {
            if (!capsules[i].activeSelf)
                continue;
            remainingCapsules++;
        }

        return remainingCapsules == 0;
    }

    /// <summary>
    /// Checks if capsule dispenser is empty.
    /// </summary>
    /// <returns>TRUE if capsule dispenser is empty. Otherwise, FALSE.</returns>
    public bool DispenserIsEmpty() => capsuleDispenserCount == 0;

    /// <summary>
    /// Checks if current game session is over.
    /// </summary>
    /// <returns>TRUE if game is over. Otherwise, FALSE.</returns>
    public bool GameOver() => gameOver || !gameStarted;

    private void DeactivateCapsules()
    {
        foreach (GameObject capsule in capsules)
        {
            capsule.transform.position = capsuleOrigin.position;
            capsule.SetActive(false);
        }
    }

    private void DeactivateResultsScreen()
    {
        foreach (GameObject capsule in capsules)
            capsule.SetActive(false);
    }

    private void StartGame() => gameStarted = true;

    /// <summary>
    /// Sets all the capsules to a random color based on the color palette.
    /// </summary>
    private void SetCapsuleColors()
    {
        int colorCount = GameManager.Instance.mainPalette.possibleCapsuleColors.Length;
        for (int i = 0; i < colorCount; i++)
            capsuleColors.Add(GameManager.Instance.mainPalette.possibleCapsuleColors[i]);

        foreach (GameObject capsule in capsules)
        {
            int rand = Random.Range(0, colorCount);
            capsule.GetComponent<SpriteRenderer>().color = capsuleColors[rand];
        }
    }
}
