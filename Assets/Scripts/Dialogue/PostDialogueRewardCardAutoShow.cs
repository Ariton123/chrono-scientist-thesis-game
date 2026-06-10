using System.Collections;
using UnityEngine;

public class PostDialogueRewardCardAutoShow : MonoBehaviour
{
    [Header("Rewards Controller")]
    [SerializeField] private RewardsPanelController rewardsPanelController;

    [Header("Card To Show")]
    [Tooltip("Example: Reward_Level1_Nibbles")]
    [SerializeField] private string rewardUnlockKey;

    [Header("Timing")]
    [SerializeField] private bool showOnStart = true;
    [SerializeField] private float showDelay = 0.75f;

    [Header("Stats Mode")]
    [Tooltip("Post-dialogue cards should show the latest completed run, not the best record saved in Rewards.")]
    [SerializeField] private bool useLatestRunStats = true;

    [Header("Safety")]
    [Tooltip("If true, the card can still appear even if the unlock PlayerPrefs was not found yet.")]
    [SerializeField] private bool forceOpen = false;

    private bool alreadyShown = false;

    private void Awake()
    {
        if (rewardsPanelController == null)
            rewardsPanelController = FindFirstObjectByType<RewardsPanelController>(FindObjectsInactive.Include);
    }

    private void Start()
    {
        if (showOnStart)
            ShowAfterDelay();
    }

    public void ShowAfterDelay()
    {
        if (alreadyShown)
            return;

        StartCoroutine(ShowRoutine());
    }

    public void ShowNow()
    {
        if (alreadyShown)
            return;

        alreadyShown = true;
        OpenCard();
    }

    private IEnumerator ShowRoutine()
    {
        alreadyShown = true;

        if (showDelay > 0f)
            yield return new WaitForSecondsRealtime(showDelay);

        OpenCard();
    }

    private void OpenCard()
    {
        if (rewardsPanelController == null)
        {
            Debug.LogWarning("[PostDialogueRewardCardAutoShow] Missing RewardsPanelController reference.");
            return;
        }

        if (string.IsNullOrEmpty(rewardUnlockKey))
        {
            Debug.LogWarning("[PostDialogueRewardCardAutoShow] Missing rewardUnlockKey.");
            return;
        }

        Debug.Log($"[PostDialogueRewardCardAutoShow] Opening card: {rewardUnlockKey}. ForceOpen: {forceOpen}, UseLatestRunStats: {useLatestRunStats}");

        rewardsPanelController.OpenDiscoveryCardByUnlockKey(
            rewardUnlockKey,
            forceOpen,
            useLatestRunStats
        );
    }
}