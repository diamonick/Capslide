using UnityEngine;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour, IUnityAdsListener
{
    // Ad Manager Singleton
    public static AdManager Instance { get; private set; }

    // Constants
    private const int LEVELS_PLAYED_AD_LIMIT = 5;

    // Store IDs
    private readonly string appStoreID = "4011078";
    private readonly string playStoreID = "4011079";

    // Ad Unit IDs (iOS)
    private readonly string interstitialAd_IOS = "Interstitial_iOS";
    private readonly string rewardedAd_IOS = "Rewarded_iOS";

    // Ad Unit IDs (Android)
    private readonly string interstitialAd_ANDROID = "Interstitial_Android";
    private readonly string rewardedAd_ANDROID = "Rewarded_Android";

    public bool isTargetPlayStore;
    public bool isTestAd;
    public bool adIsRunning;
    public bool interstitialAdIsViewed;

    private void Awake()
    {
        interstitialAdIsViewed = false;
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
        Advertisement.AddListener(this);
        InitializeAd();
    }

    /// <summary>
    /// Initialize ad.
    /// </summary>
    private void InitializeAd()
    {
        if (isTargetPlayStore)
            Advertisement.Initialize(playStoreID, isTestAd);
        else
            Advertisement.Initialize(appStoreID, isTestAd);
    }

    /// <summary>
    /// Play interstitial ad.
    /// </summary>
    public void PlayInterstitialAd()
    {
#if UNITY_ANDROID
        if (!Advertisement.IsReady(interstitialAd_ANDROID)) { return; }

        adIsRunning = true;
        Advertisement.Show(interstitialAd_ANDROID);
#endif

#if UNITY_IOS
        if (!Advertisement.IsReady(interstitialAd_IOS)) { return; }

        adIsRunning = true;
        Advertisement.Show(interstitialAd_IOS);
#endif
    }

    /// <summary>
    /// Play rewarded ad.
    /// </summary>
    public void PlayRewardedAd()
    {
        if (adIsRunning)
            return;

#if UNITY_ANDROID
        if (!Advertisement.IsReady(rewardedAd_ANDROID)) { return; }

        adIsRunning = true;
        Advertisement.Show(rewardedAd_ANDROID);
#endif

#if UNITY_IOS
        if (!Advertisement.IsReady(rewardedAd_IOS)) { return; }

        adIsRunning = true;
        Advertisement.Show(rewardedAd_IOS);
#endif
    }

    public void OnUnityAdsReady(string placementId)
    {
        //throw new System.NotImplementedException();
    }

    public void OnUnityAdsDidError(string message)
    {
        //throw new System.NotImplementedException();
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        AudioManager.Instance.MuteAudio();
        //throw new System.NotImplementedException();
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        switch (showResult)
        {
            case ShowResult.Failed:
                if (placementId == rewardedAd_ANDROID || placementId == rewardedAd_IOS)
                {
                    GameplayManager.Instance.RewardTokens();
                }
                if (placementId == interstitialAd_ANDROID || placementId == interstitialAd_IOS)
                    Debug.Log("Interstitial ad failed.");
                break;
            case ShowResult.Skipped:
                if (placementId == interstitialAd_ANDROID || placementId == interstitialAd_IOS)
                    Debug.Log("Interstitial ad skipped.");
                break;
            case ShowResult.Finished:
                if (placementId == rewardedAd_ANDROID || placementId == rewardedAd_IOS)
                {
                    GameplayManager.Instance.RewardTokens();
                }
                if (placementId == interstitialAd_ANDROID || placementId == interstitialAd_IOS)
                    interstitialAdIsViewed = true;
                break;
        }
        adIsRunning = false;
        AudioManager.Instance.UnmuteAudio();
        //throw new System.NotImplementedException();
    }

    /// <summary>
    /// Method to check if you've played enough levels to show an interstitial ad.
    /// </summary>
    /// <param name="score">Score you've earned from the current level.</param>
    /// <returns>TRUE if </returns>
    public bool InterstitialAdReady(int score)
    {
        // Increment this if you've scored at least 256 points.
        if (score >= 256)
            GameManager.Instance.levelsPlayedUntilDisplayAd++;

        bool adReady = GameManager.Instance.levelsPlayedUntilDisplayAd >= LEVELS_PLAYED_AD_LIMIT;

        if (!adReady) { return false; }

        GameManager.Instance.levelsPlayedUntilDisplayAd = 0;
        return adReady;
    }
}
