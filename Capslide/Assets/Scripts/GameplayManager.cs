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
    private const int CAPSULE_TOTAL = 20;
    private const float TOKEN_SPAWN_INTERVAL = 20f;

    [Header("Properties"), Space(8)]
    [SerializeField] private int score;
    [SerializeField] private int capsuleDispenserCount;
    [SerializeField] private float timeUntilDispense;
    [SerializeField] private GameObject capsuleDispenser;
    [SerializeField] private GameObject[] capsules;
    private List<Color> capsuleColors = new List<Color>();

    [Header("UI"), Space(8)]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text capsuleCountText;
    private float fullTime;
    [SerializeField] private Image fillTimer;

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

        ResetGame();
    }

    // Start is called before the first frame update
    void Start()
    {
        SetCapsuleColors();
        scoreText.text = $"{score}";
        capsuleCountText.text = $"{capsuleDispenserCount}";
        DispenseCapsule();
    }

    // Update is called once per frame
    void Update()
    {
        CapsuleDispenserTimer();
        TokenSpawnTimer();
    }

    private void CapsuleDispenserTimer()
    {
        fillTimer.fillAmount = (timeUntilDispense / fullTime);

        if (timeUntilDispense > 0f)
        {
            timeUntilDispense -= Time.deltaTime;
            timeUntilDispense = Mathf.Max(0f, timeUntilDispense);
        }
        else
            DispenseCapsule(true);
    }
    private void TokenSpawnTimer()
    {
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
        if (DispenserIsEmpty())
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

    public void SetScore(int s)
    {
        score += s;
        scoreText.text = $"{score}";
    }

    private void ResetGame()
    {
        score = 0;
        capsuleDispenserCount = CAPSULE_TOTAL;
        timeUntilDispense = 30f;
    }

    private int GetExistingCapsules() => CAPSULE_TOTAL - capsuleDispenserCount;

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
    private bool DispenserIsEmpty() => capsuleDispenserCount == 0;

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
