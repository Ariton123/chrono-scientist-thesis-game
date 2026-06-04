using UnityEngine;
using TMPro;

public class LevelPerformanceController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text mistakesText;

    [Header("Timing")]
    [SerializeField] private float levelDurationSeconds = 90f;

    [Tooltip("Gold if completed with this much time remaining or more.")]
    [SerializeField] private float goldRemainingThreshold = 60f;

    [Tooltip("Silver if completed with this much time remaining or more.")]
    [SerializeField] private float silverRemainingThreshold = 30f;

    [Header("Timer Warning")]
    [SerializeField] private float warningTimeThreshold = 20f;
    [SerializeField] private Color normalTimerColor = Color.white;
    [SerializeField] private Color warningTimerColor = Color.red;
    [SerializeField] private float warningBlinkSpeed = 6f;

    [Header("Mistake Badge")]
    [SerializeField] private int maxMistakesForAstragalosBadge = 1;

    [Header("Reward Keys")]
    [Tooltip("Example: Reward_Level1_Nibbles")]
    [SerializeField] private string rewardUnlockKey;

    [Header("Logging")]
    [Tooltip("Example: Stage1_Level1, Stage2_Level5, Stage3_Level9")]
    [SerializeField] private string stageId = "Stage1_Level1";

    [Header("Fail Flow")]
    [SerializeField] private Stage1AssemblyController assemblyController;

    private float remainingTime;
    private int mistakes;

    private bool isRunning = false;
    private bool isCompleted = false;
    private bool hasFailed = false;

    public bool IsRunning => isRunning;
    public bool IsCompleted => isCompleted;
    public bool HasFailed => hasFailed;

    public int Mistakes => mistakes;
    public float RemainingTime => remainingTime;
    public float ElapsedTime => Mathf.Max(0f, levelDurationSeconds - remainingTime);

    private void Start()
    {
        ResetVisualsOnly();
    }

    private void Update()
    {
        if (!isRunning || isCompleted || hasFailed)
            return;

        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            UpdateTimerUI();
            FailLevel();
            return;
        }

        UpdateTimerUI();
    }

    public void BeginLevel()
    {
        remainingTime = levelDurationSeconds;
        mistakes = 0;

        isRunning = true;
        isCompleted = false;
        hasFailed = false;

        UpdateTimerUI();
        UpdateMistakesUI();
        ResetTimerColor();

        Debug.Log("[Performance] Level timer started.");

        SessionCSVLogger.LogEvent(
            "STAGE_START",
            stageId,
            GetCurrentLanguageForLog(),
            GetCurrentGenderForLog()
        );
    }

    public void RegisterMistake()
    {
        if (!isRunning || isCompleted || hasFailed)
            return;

        mistakes++;
        UpdateMistakesUI();

        Debug.Log($"[Performance] Mistake registered. Total mistakes: {mistakes}");

        SessionCSVLogger.LogEvent(
            "MISTAKE",
            stageId,
            GetCurrentLanguageForLog(),
            GetCurrentGenderForLog(),
            mistakes: mistakes
        );
    }

    public void CompleteLevel()
    {
        if (!isRunning || isCompleted || hasFailed)
            return;

        isRunning = false;
        isCompleted = true;

        ResetTimerColor();

        RewardsPanelController.CardRank earnedRank = CalculateRank();

        bool earnedAstragalosBadge =
            earnedRank == RewardsPanelController.CardRank.Gold &&
            mistakes <= maxMistakesForAstragalosBadge;

        SavePerformanceResult(earnedRank, earnedAstragalosBadge);

        SessionCSVLogger.LogEvent(
            "STAGE_COMPLETE",
            stageId,
            GetCurrentLanguageForLog(),
            GetCurrentGenderForLog(),
            ElapsedTime,
            mistakes,
            rank: earnedRank.ToString(),
            astragalosBadge: earnedAstragalosBadge
        );

        SessionCSVLogger.LogEvent(
            "REWARD_CARD_UNLOCKED",
            stageId,
            GetCurrentLanguageForLog(),
            GetCurrentGenderForLog(),
            ElapsedTime,
            mistakes,
            rank: earnedRank.ToString(),
            astragalosBadge: earnedAstragalosBadge,
            extra: rewardUnlockKey
        );

        SessionCSVLogger.LogEvent(
            "CARD_RANK_ASSIGNED",
            stageId,
            GetCurrentLanguageForLog(),
            GetCurrentGenderForLog(),
            ElapsedTime,
            mistakes,
            rank: earnedRank.ToString(),
            astragalosBadge: earnedAstragalosBadge
        );

        if (earnedAstragalosBadge)
        {
            SessionCSVLogger.LogEvent(
                "ASTRAGALOS_BADGE_AWARDED",
                stageId,
                GetCurrentLanguageForLog(),
                GetCurrentGenderForLog(),
                ElapsedTime,
                mistakes,
                rank: earnedRank.ToString(),
                astragalosBadge: true
            );
        }

        RewardsPanelController.RegisterLevelStats(mistakes, ElapsedTime);

        Debug.Log($"[Performance] Level completed. Rank: {earnedRank}, Mistakes: {mistakes}, Astragalos badge: {earnedAstragalosBadge}");
    }

    public void StopLevel()
    {
        isRunning = false;
        ResetTimerColor();
    }

    private void FailLevel()
    {
        if (hasFailed || isCompleted)
            return;

        isRunning = false;
        hasFailed = true;

        if (timerText != null)
            timerText.color = warningTimerColor;

        Debug.Log("[Performance] Timer reached 0. Level failed.");

        SessionCSVLogger.LogEvent(
            "STAGE_FAIL",
            stageId,
            GetCurrentLanguageForLog(),
            GetCurrentGenderForLog(),
            ElapsedTime,
            mistakes
        );

        if (assemblyController != null)
            assemblyController.FailAssembly();
        else
            Debug.LogWarning("[Performance] Missing Stage1AssemblyController reference. Cannot show retry overlay.");
    }

    private RewardsPanelController.CardRank CalculateRank()
    {
        if (remainingTime >= goldRemainingThreshold)
            return RewardsPanelController.CardRank.Gold;

        if (remainingTime >= silverRemainingThreshold)
            return RewardsPanelController.CardRank.Silver;

        return RewardsPanelController.CardRank.Bronze;
    }

    private void SavePerformanceResult(RewardsPanelController.CardRank rank, bool earnedAstragalosBadge)
    {
        if (string.IsNullOrEmpty(rewardUnlockKey))
        {
            Debug.LogWarning("[Performance] Missing rewardUnlockKey. Rank, badge, and stats will not be saved.");
            return;
        }

        string rankKey = rewardUnlockKey + "_Rank";
        string badgeKey = rewardUnlockKey + "_AstragalosBadge";
        string mistakesKey = rewardUnlockKey + "_Mistakes";
        string timeKey = rewardUnlockKey + "_CompletionTime";

        string lastRankKey = rewardUnlockKey + "_LastRank";
        string lastBadgeKey = rewardUnlockKey + "_LastAstragalosBadge";
        string lastMistakesKey = rewardUnlockKey + "_LastMistakes";
        string lastTimeKey = rewardUnlockKey + "_LastCompletionTime";

        float completionTime = ElapsedTime;

        // 1) ALWAYS save latest run.
        // This is used only by the post-dialogue card.
        PlayerPrefs.SetInt(lastRankKey, (int)rank);
        PlayerPrefs.SetInt(lastBadgeKey, earnedAstragalosBadge ? 1 : 0);
        PlayerPrefs.SetInt(lastMistakesKey, mistakes);
        PlayerPrefs.SetFloat(lastTimeKey, completionTime);

        // 2) BEST RANK achieved.
        // Bronze = 0, Silver = 1, Gold = 2, so higher is better.
        if (!PlayerPrefs.HasKey(rankKey))
        {
            PlayerPrefs.SetInt(rankKey, (int)rank);
        }
        else
        {
            RewardsPanelController.CardRank previousBestRank =
                (RewardsPanelController.CardRank)PlayerPrefs.GetInt(rankKey, (int)RewardsPanelController.CardRank.Bronze);

            if (rank > previousBestRank)
                PlayerPrefs.SetInt(rankKey, (int)rank);
        }

        // 3) FEWEST MISTAKES achieved.
        if (!PlayerPrefs.HasKey(mistakesKey))
        {
            PlayerPrefs.SetInt(mistakesKey, mistakes);
        }
        else
        {
            int previousBestMistakes = PlayerPrefs.GetInt(mistakesKey, int.MaxValue);

            if (mistakes < previousBestMistakes)
                PlayerPrefs.SetInt(mistakesKey, mistakes);
        }

        // 4) FASTEST COMPLETION TIME achieved.
        if (!PlayerPrefs.HasKey(timeKey))
        {
            PlayerPrefs.SetFloat(timeKey, completionTime);
        }
        else
        {
            float previousBestTime = PlayerPrefs.GetFloat(timeKey, float.MaxValue);

            if (completionTime < previousBestTime)
                PlayerPrefs.SetFloat(timeKey, completionTime);
        }

        // 5) GOLDEN ASTRAGALOS BADGE is "ever earned".
        // Once earned, never remove it unless we reset the session.
        if (earnedAstragalosBadge)
        {
            PlayerPrefs.SetInt(badgeKey, 1);
        }
        else if (!PlayerPrefs.HasKey(badgeKey))
        {
            PlayerPrefs.SetInt(badgeKey, 0);
        }

        PlayerPrefs.Save();

        Debug.Log($"[Performance] Saved latest run and updated independent best records for {rewardUnlockKey}. Rank: {rank}, Mistakes: {mistakes}, Time: {completionTime}, Badge this run: {earnedAstragalosBadge}");
    }

    private void ResetVisualsOnly()
    {
        remainingTime = levelDurationSeconds;
        mistakes = 0;

        UpdateTimerUI();
        UpdateMistakesUI();
        ResetTimerColor();
    }

    private void UpdateTimerUI()
    {
        if (timerText == null)
            return;

        int minutes = Mathf.FloorToInt(remainingTime / 60f);
        int seconds = Mathf.FloorToInt(remainingTime % 60f);

        timerText.text = $"{minutes:00}:{seconds:00}";

        if (isRunning && !isCompleted && !hasFailed && remainingTime <= warningTimeThreshold)
        {
            float blink = Mathf.Abs(Mathf.Sin(Time.time * warningBlinkSpeed));
            timerText.color = Color.Lerp(normalTimerColor, warningTimerColor, blink);
        }
        else if (!hasFailed)
        {
            timerText.color = normalTimerColor;
        }
    }

    private void UpdateMistakesUI()
    {
        if (mistakesText == null)
            return;

        mistakesText.text = mistakes.ToString();
    }

    private void ResetTimerColor()
    {
        if (timerText != null)
            timerText.color = normalTimerColor;
    }

    private string GetCurrentLanguageForLog()
    {
        if (LanguageManager.Instance == null)
            return "";

        // Uses reflection so this does not break if the language property name changes.
        System.Type languageManagerType = LanguageManager.Instance.GetType();

        System.Reflection.PropertyInfo currentLanguageProperty =
            languageManagerType.GetProperty("CurrentLanguage");

        if (currentLanguageProperty != null)
        {
            object value = currentLanguageProperty.GetValue(LanguageManager.Instance, null);
            return value != null ? value.ToString() : "";
        }

        System.Reflection.FieldInfo currentLanguageField =
            languageManagerType.GetField("CurrentLanguage");

        if (currentLanguageField != null)
        {
            object value = currentLanguageField.GetValue(LanguageManager.Instance);
            return value != null ? value.ToString() : "";
        }

        return "";
    }

    private string GetCurrentGenderForLog()
    {
        if (CharacterSelectionManager.Instance == null)
            return "";

        return CharacterSelectionManager.Instance.CurrentGender.ToString();
    }
}