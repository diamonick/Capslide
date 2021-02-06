using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameplayManager : MonoBehaviour
{
    //Constants
    private const int CAPSULE_TOTAL = 20;

    [Header("Properties"), Space(8)]
    [SerializeField] private int score;
    [SerializeField] private int capsuleDispenserCount;
    [SerializeField] private float timeUntilDispense;
    [SerializeField] private GameObject capsuleDispenser;
    [SerializeField] private GameObject[] capsules;

    [Header("UI"), Space(8)]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text capsuleCountText;
    private float fullTime;
    [SerializeField] private Image fillTimer;
    // Game Manager Singleton
    public static GameplayManager Instance { get; private set; }

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
        scoreText.text = $"{score}";
        capsuleCountText.text = $"{capsuleDispenserCount}";
        DispenseCapsule();
    }

    // Update is called once per frame
    void Update()
    {
        CapsuleDispenserTimer();
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
            DispenseCapsule();
    }

    /// <summary>
    /// Dispense capsule from the top screen.
    /// </summary>
    public void DispenseCapsule()
    {
        if (DispenserIsEmpty())
            return;

        capsules[GetExistingCapsules()].SetActive(true);

        capsuleDispenserCount--;
        timeUntilDispense = 30f - ((GetExistingCapsules()) * 2);
        fullTime = timeUntilDispense;
        capsuleCountText.text = $"{capsuleDispenserCount}";
    }

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
}
