using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CloudOnce;

public class CloudOnceServices : MonoBehaviour
{
    public enum Level
    {
        Arena = 0,
        Diamond = 1,
        Orbit = 2,
        Pinball = 3,
        Warp = 4,
        Clock = 5
    }

    private readonly Dictionary<Level, CloudOnce.Internal.UnifiedLeaderboard> levelLeaderboards = new Dictionary<Level, CloudOnce.Internal.UnifiedLeaderboard>()
    {
        {Level.Arena, Leaderboards.Arena},
        {Level.Diamond, Leaderboards.Diamond},
        {Level.Orbit, Leaderboards.Orbit},
        {Level.Pinball, Leaderboards.Pinball},
        {Level.Warp, Leaderboards.Warp},
        {Level.Clock, Leaderboards.Clock}
    };

    // CloudOnce Services Singleton
    public static CloudOnceServices Instance { get; private set; }

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

    /// <summary>
    /// Submit your highscore to the leaderboard.
    /// </summary>
    /// <param name="level">Current level.</param>
    /// <param name="score">The player's highscore to submit.</param>
    public void SubmitScoreToLeaderboard(Level level, int score) => levelLeaderboards[level].SubmitScore(score);

    // --------------------------------------------------------------------------------------------------------------------------------------
    // Achievement Methods

    /// <summary>
    /// Award player the "Arena Cleared" achievement.
    /// </summary>
    public void AwardArenaCleared()
    {
        if (Achievements.ArenaCleared.IsUnlocked) { return; }
        Achievements.ArenaCleared.Unlock();
    }

    /// <summary>
    /// Award player the "Diamond Cleared" achievement.
    /// </summary>
    public void AwardDiamondCleared()
    {
        if (Achievements.DiamondCleared.IsUnlocked) { return; }
        Achievements.DiamondCleared.Unlock();
    }

    /// <summary>
    /// Award player the "Orbit Cleared" achievement.
    /// </summary>
    public void AwardOrbitCleared()
    {
        if (Achievements.OrbitCleared.IsUnlocked) { return; }
        Achievements.OrbitCleared.Unlock();
    }

    /// <summary>
    /// Award player the "Pinball Cleared" achievement.
    /// </summary>
    public void AwardPinballCleared()
    {
        if (Achievements.PinballCleared.IsUnlocked) { return; }
        Achievements.PinballCleared.Unlock();
    }

    /// <summary>
    /// Award player the "Warp Cleared" achievement.
    /// </summary>
    public void AwardWarpCleared()
    {
        if (Achievements.WarpCleared.IsUnlocked) { return; }
        Achievements.WarpCleared.Unlock();
    }

    /// <summary>
    /// Award player the "Clock Cleared" achievement.
    /// </summary>
    public void AwardClockCleared()
    {
        if (Achievements.ClockCleared.IsUnlocked) { return; }
        Achievements.ClockCleared.Unlock();
    }

    /// <summary>
    /// Award player the "Token Player" achievement.
    /// </summary>
    public void AwardTokenPlayer()
    {
        if (Achievements.TokenPlayer.IsUnlocked) { return; }

        int currentProgress = GameManager.Instance.tokensNeededForTokenPlayer;
        Achievements.TokenPlayer.Increment(currentProgress, 256.0);
    }

    /// <summary>
    /// Award player the "Star Player" achievement.
    /// </summary>
    public void AwardStarPlayer()
    {
        if (Achievements.StarPlayer.IsUnlocked) { return; }
        Achievements.StarPlayer.Unlock();
    }

    /// <summary>
    /// Award player the "Capslider" achievement.
    /// </summary>
    public void AwardCapslider()
    {
        if (Achievements.Capslider.IsUnlocked) { return; }

        int currentProgress = GameManager.Instance.levelsPlayed;
        Achievements.Capslider.Increment(currentProgress, 25.0);
    }

    /// <summary>
    /// Award player the "Color Me Impressed" achievement.
    /// </summary>
    public void AwardColorMeImpressed()
    {
        if (Achievements.ColorMeImpressed.IsUnlocked) { return; }

        int currentProgress = 0;

        foreach (bool paletteUnlocked in GameManager.Instance.palettesUnlocked)
            currentProgress += paletteUnlocked ? 1 : 0;

        Achievements.ColorMeImpressed.Increment(currentProgress, 64.0);
    }
}
