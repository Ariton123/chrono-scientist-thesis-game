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
    }

    public void RegisterMistake()
    {
        if (!isRunning || isCompleted || hasFailed)
            return;

        mistakes++;
        UpdateMistakesUI();

        Debug.Log($"[Performance] Mistake registered. Total mistakes: {mistakes}");
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
            Debug.LogWarning("[Performance] Missing rewardUnlockKey. Rank and badge will not be saved.");
            return;
        }

        string rankKey = rewardUnlockKey + "_Rank";
        string badgeKey = rewardUnlockKey + "_AstragalosBadge";
        string mistakesKey = rewardUnlockKey + "_Mistakes";
        string timeKey = rewardUnlockKey + "_CompletionTime";

        PlayerPrefs.SetInt(rankKey, (int)rank);
        PlayerPrefs.SetInt(badgeKey, earnedAstragalosBadge ? 1 : 0);
        PlayerPrefs.SetInt(mistakesKey, mistakes);
        PlayerPrefs.SetFloat(timeKey, ElapsedTime);

        PlayerPrefs.Save();

        Debug.Log($"[Performance] Saved result for {rewardUnlockKey}. Rank: {rank}, Badge: {earnedAstragalosBadge}");
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
}