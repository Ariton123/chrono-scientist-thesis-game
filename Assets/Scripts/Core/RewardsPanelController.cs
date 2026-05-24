using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardsPanelController : MonoBehaviour
{
    private enum RewardsTab
    {
        Rewards,
        Details
    }

    public enum CardRank
    {
        Bronze,
        Silver,
        Gold
    }

    [System.Serializable]
    public class DiscoveryCard
    {
        [Header("Card Identity")]
        public string cardID;
        public string unlockPlayerPrefsKey;

        [Header("Small Card UI")]
        public Button cardButton;
        public Image cardImage;
        public Image characterEmoticonImage;
        public TMP_Text cardTitleText;

        [Header("Small Card Sprites")]
        public Sprite lockedCardSprite;
        public Sprite unlockedCardSprite;

        [Header("Small Card Emoticons")]
        public Sprite lockedCharacterEmoticonSprite;
        public Sprite unlockedCharacterEmoticonSprite;

        [Header("Opened Card UI")]
        public Sprite openedCardSprite;
        public string openedCardTitle;

        [TextArea(3, 8)]
        public string openedCardDescription;

        [Header("Opened Card Astragalos Badge")]
        public Sprite blankAstragalosBadgeSprite;
        public Sprite earnedAstragalosBadgeSprite;

        [Header("Fallback Card Rank")]
        [Tooltip("Used only if no saved rank exists yet.")]
        public CardRank rank = CardRank.Bronze;

        [Header("Side Character")]
        public bool unlocksSideCharacter = true;
    }

    [Header("Main Panel")]
    [SerializeField] private GameObject rewardsPanel;

    [Header("Title Switch")]
    [SerializeField] private Button rewardsTitleButton;
    [SerializeField] private Button detailsTitleButton;
    [SerializeField] private TMP_Text rewardsTitleText;
    [SerializeField] private TMP_Text slashTitleText;
    [SerializeField] private TMP_Text detailsTitleText;

    [SerializeField] private Color selectedTitleColor = Color.black;
    [SerializeField] private Color unselectedTitleColor = new Color(0.45f, 0.35f, 0.25f, 1f);
    [SerializeField] private Color slashTitleColor = Color.black;

    [Header("Title Underlines")]
    [SerializeField] private GameObject rewardsUnderline;
    [SerializeField] private GameObject detailsUnderline;

    [Header("Content")]
    [SerializeField] private GameObject rewardsContent;
    [SerializeField] private GameObject detailsContent;

    [Header("Discovery Cards")]
    [SerializeField] private DiscoveryCard[] discoveryCards;

    [Header("Opened Card Popup")]
    [Tooltip("Assign the full OpenedCardPopup object here, not only the inner card panel.")]
    [SerializeField] private GameObject openedCardPopup;

    [SerializeField] private Image openedCardImage;
    [SerializeField] private TMP_Text openedCardTitleText;
    [SerializeField] private TMP_Text openedCardDescriptionText;

    [Header("Opened Card Performance")]
    [SerializeField] private Image rankTintOverlay;
    [SerializeField] private TMP_Text openedRankText;
    [SerializeField] private Image openedAstragalosBadgeImage;

    [Header("Rank Tint Colors")]
    [SerializeField] private Color goldTintColor = new Color(1f, 0.78f, 0.15f, 0.18f);
    [SerializeField] private Color silverTintColor = new Color(0.85f, 0.9f, 1f, 0.16f);
    [SerializeField] private Color bronzeTintColor = new Color(0.75f, 0.38f, 0.12f, 0.18f);

    [Header("Details Texts")]
    [SerializeField] private TMP_Text totalDiscoveryCardsText;
    [SerializeField] private TMP_Text goldCardsText;
    [SerializeField] private TMP_Text silverCardsText;
    [SerializeField] private TMP_Text bronzeCardsText;
    [SerializeField] private TMP_Text averageMistakesText;
    [SerializeField] private TMP_Text averageCompletionTimeText;
    [SerializeField] private TMP_Text sideCharactersUnlockedText;

    [Header("Extra Details Texts")]
    [SerializeField] private TMP_Text astragalosBadgesText;

    private RewardsTab currentTab = RewardsTab.Rewards;

    private const int TotalPossibleDiscoveryCards = 12;
    private const int TotalPossibleSideCharacters = 3;
    private const int TotalPossibleAstragalosBadges = 3;

    private void Awake()
    {
        if (rewardsPanel != null)
            rewardsPanel.SetActive(false);

        if (openedCardPopup != null)
            openedCardPopup.SetActive(false);

        WireButtons();
    }

    private void OnEnable()
    {
        RefreshAll();
    }

    private void WireButtons()
    {
        if (rewardsTitleButton != null)
        {
            rewardsTitleButton.onClick.RemoveListener(ShowRewardsTab);
            rewardsTitleButton.onClick.AddListener(ShowRewardsTab);
        }

        if (detailsTitleButton != null)
        {
            detailsTitleButton.onClick.RemoveListener(ShowDetailsTab);
            detailsTitleButton.onClick.AddListener(ShowDetailsTab);
        }

        if (discoveryCards == null)
            return;

        foreach (DiscoveryCard card in discoveryCards)
        {
            if (card == null || card.cardButton == null)
                continue;

            DiscoveryCard capturedCard = card;

            card.cardButton.onClick.RemoveAllListeners();
            card.cardButton.onClick.AddListener(() => TryOpenCard(capturedCard));
        }
    }

    public void OpenRewards()
    {
        if (rewardsPanel != null)
            rewardsPanel.SetActive(true);

        ShowRewardsTab();
        CloseOpenedCard();
        RefreshAll();
    }

    public void CloseRewards()
    {
        CloseOpenedCard();

        if (rewardsPanel != null)
            rewardsPanel.SetActive(false);
    }

    public void ToggleRewards()
    {
        if (rewardsPanel == null)
            return;

        bool shouldOpen = !rewardsPanel.activeSelf;
        rewardsPanel.SetActive(shouldOpen);

        if (shouldOpen)
        {
            ShowRewardsTab();
            CloseOpenedCard();
            RefreshAll();
        }
        else
        {
            CloseOpenedCard();
        }
    }

    public void CloseOpenedCard()
    {
        if (openedCardPopup != null)
            openedCardPopup.SetActive(false);
    }

    public void ShowRewardsTab()
    {
        currentTab = RewardsTab.Rewards;
        CloseOpenedCard();
        RefreshTabs();
    }

    public void ShowDetailsTab()
    {
        currentTab = RewardsTab.Details;
        CloseOpenedCard();
        RefreshTabs();
        RefreshDetails();
    }

    private void RefreshAll()
    {
        RefreshTitleSwitch();
        RefreshTabs();
        RefreshCards();
        RefreshDetails();
    }

    private void RefreshTitleSwitch()
    {
        if (rewardsTitleText != null)
        {
            rewardsTitleText.text = LanguageManager.Instance != null
                ? LanguageManager.Instance.GetText("REWARDS")
                : "REWARDS";
        }

        if (slashTitleText != null)
        {
            slashTitleText.text = "|";
            slashTitleText.color = slashTitleColor;
        }

        if (detailsTitleText != null)
        {
            detailsTitleText.text = LanguageManager.Instance != null
                ? LanguageManager.Instance.GetText("DETAILS")
                : "DETAILS";
        }
    }

    private void RefreshTabs()
    {
        bool rewardsSelected = currentTab == RewardsTab.Rewards;

        if (rewardsContent != null)
            rewardsContent.SetActive(rewardsSelected);

        if (detailsContent != null)
            detailsContent.SetActive(!rewardsSelected);

        ApplyTitleState(rewardsTitleText, rewardsSelected);
        ApplyTitleState(detailsTitleText, !rewardsSelected);

        if (slashTitleText != null)
        {
            slashTitleText.color = slashTitleColor;
            slashTitleText.alpha = 1f;
            slashTitleText.fontStyle = FontStyles.Bold;

            RectTransform slashRect = slashTitleText.GetComponent<RectTransform>();
            if (slashRect != null)
                slashRect.localScale = Vector3.one;

            slashTitleText.ForceMeshUpdate();
        }

        if (rewardsUnderline != null)
            rewardsUnderline.SetActive(rewardsSelected);

        if (detailsUnderline != null)
            detailsUnderline.SetActive(!rewardsSelected);
    }

    private void ApplyTitleState(TMP_Text text, bool selected)
    {
        if (text == null)
            return;

        text.color = selected ? selectedTitleColor : unselectedTitleColor;
        text.alpha = selected ? 1f : 0.60f;
        text.fontStyle = selected ? FontStyles.Bold : FontStyles.Normal;

        RectTransform rect = text.GetComponent<RectTransform>();
        if (rect != null)
            rect.localScale = selected ? Vector3.one * 1.08f : Vector3.one * 0.95f;

        text.ForceMeshUpdate();
    }

    private void RefreshCards()
    {
        if (discoveryCards == null)
            return;

        foreach (DiscoveryCard card in discoveryCards)
        {
            if (card == null)
                continue;

            bool unlocked = IsCardUnlocked(card);

            if (card.cardButton != null)
                card.cardButton.interactable = unlocked;

            if (card.cardImage != null)
            {
                card.cardImage.sprite = unlocked ? card.unlockedCardSprite : card.lockedCardSprite;
                card.cardImage.enabled = card.cardImage.sprite != null;
                card.cardImage.preserveAspect = false;
                card.cardImage.color = Color.white;
            }

            if (card.characterEmoticonImage != null)
            {
                Sprite emoticonSprite = unlocked
                    ? card.unlockedCharacterEmoticonSprite
                    : card.lockedCharacterEmoticonSprite;

                card.characterEmoticonImage.sprite = emoticonSprite;
                card.characterEmoticonImage.enabled = emoticonSprite != null;
                card.characterEmoticonImage.preserveAspect = true;
                card.characterEmoticonImage.color = Color.white;
            }

            if (card.cardTitleText != null)
            {
                card.cardTitleText.gameObject.SetActive(unlocked);
                card.cardTitleText.text = card.openedCardTitle;
            }
        }
    }

    private void TryOpenCard(DiscoveryCard card)
    {
        if (card == null)
            return;

        if (!IsCardUnlocked(card))
            return;

        if (openedCardPopup != null)
        {
            openedCardPopup.SetActive(true);
            openedCardPopup.transform.SetAsLastSibling();
        }

        if (openedCardImage != null)
        {
            openedCardImage.sprite = card.openedCardSprite;
            openedCardImage.enabled = card.openedCardSprite != null;
            openedCardImage.preserveAspect = true;
            openedCardImage.color = Color.white;
        }

        if (openedCardTitleText != null)
            openedCardTitleText.text = card.openedCardTitle;

        if (openedCardDescriptionText != null)
            openedCardDescriptionText.text = card.openedCardDescription;

        CardRank earnedRank = GetSavedRank(card);
        bool earnedAstragalos = HasEarnedAstragalosBadge(card);

        ApplyOpenedCardPerformance(card, earnedRank, earnedAstragalos);
    }

    private bool IsCardUnlocked(DiscoveryCard card)
    {
        if (card == null || string.IsNullOrEmpty(card.unlockPlayerPrefsKey))
            return false;

        return PlayerPrefs.GetInt(card.unlockPlayerPrefsKey, 0) == 1;
    }

    private CardRank GetSavedRank(DiscoveryCard card)
    {
        if (card == null || string.IsNullOrEmpty(card.unlockPlayerPrefsKey))
            return CardRank.Bronze;

        string rankKey = card.unlockPlayerPrefsKey + "_Rank";

        if (!PlayerPrefs.HasKey(rankKey))
            return card.rank;

        return (CardRank)PlayerPrefs.GetInt(rankKey, (int)card.rank);
    }

    private bool HasEarnedAstragalosBadge(DiscoveryCard card)
    {
        if (card == null || string.IsNullOrEmpty(card.unlockPlayerPrefsKey))
            return false;

        string badgeKey = card.unlockPlayerPrefsKey + "_AstragalosBadge";
        return PlayerPrefs.GetInt(badgeKey, 0) == 1;
    }

    private void ApplyOpenedCardPerformance(DiscoveryCard card, CardRank rank, bool earnedAstragalos)
    {
        if (rankTintOverlay != null)
        {
            rankTintOverlay.enabled = true;
            rankTintOverlay.raycastTarget = false;

            switch (rank)
            {
                case CardRank.Gold:
                    rankTintOverlay.color = goldTintColor;
                    break;

                case CardRank.Silver:
                    rankTintOverlay.color = silverTintColor;
                    break;

                case CardRank.Bronze:
                    rankTintOverlay.color = bronzeTintColor;
                    break;
            }
        }

        if (openedRankText != null)
        {
            openedRankText.text = $"CARD RANK: {rank.ToString().ToUpper()}";
        }

        if (openedAstragalosBadgeImage != null)
        {
            Sprite badgeSprite = earnedAstragalos
                ? card.earnedAstragalosBadgeSprite
                : card.blankAstragalosBadgeSprite;

            openedAstragalosBadgeImage.sprite = badgeSprite;
            openedAstragalosBadgeImage.enabled = badgeSprite != null;
            openedAstragalosBadgeImage.preserveAspect = true;
            openedAstragalosBadgeImage.color = Color.white;
        }
    }

    private void RefreshDetails()
    {
        int unlockedCards = 0;
        int gold = 0;
        int silver = 0;
        int bronze = 0;
        int sideCharacters = 0;
        int astragalosBadges = 0;

        if (discoveryCards != null)
        {
            foreach (DiscoveryCard card in discoveryCards)
            {
                if (card == null || !IsCardUnlocked(card))
                    continue;

                unlockedCards++;

                if (card.unlocksSideCharacter)
                    sideCharacters++;

                if (HasEarnedAstragalosBadge(card))
                    astragalosBadges++;

                CardRank savedRank = GetSavedRank(card);

                switch (savedRank)
                {
                    case CardRank.Gold:
                        gold++;
                        break;

                    case CardRank.Silver:
                        silver++;
                        break;

                    case CardRank.Bronze:
                        bronze++;
                        break;
                }
            }
        }

        if (totalDiscoveryCardsText != null)
            totalDiscoveryCardsText.text = $"Total Discovery Cards: {unlockedCards}/{TotalPossibleDiscoveryCards}";

        if (goldCardsText != null)
            goldCardsText.text = $"Gold: {gold}";

        if (silverCardsText != null)
            silverCardsText.text = $"Silver: {silver}";

        if (bronzeCardsText != null)
            bronzeCardsText.text = $"Bronze: {bronze}";

        if (averageMistakesText != null)
            averageMistakesText.text = $"Average number of mistakes per level: {GetAverageMistakesText()}";

        if (averageCompletionTimeText != null)
            averageCompletionTimeText.text = $"Average Level Completion Time: {GetAverageCompletionTimeText()}";

        if (sideCharactersUnlockedText != null)
            sideCharactersUnlockedText.text = $"Side Characters unlocked: {sideCharacters}/{TotalPossibleSideCharacters}";

        if (astragalosBadgesText != null)
            astragalosBadgesText.text = $"Golden Astragalos Badges: {astragalosBadges}/{TotalPossibleAstragalosBadges}";
    }

    private string GetAverageMistakesText()
    {
        int completedLevels = PlayerPrefs.GetInt("CompletedPlayableLevels", 0);

        if (completedLevels <= 0)
            return "-";

        int totalMistakes = PlayerPrefs.GetInt("TotalMistakes", 0);
        float average = (float)totalMistakes / completedLevels;

        return average.ToString("0.0");
    }

    private string GetAverageCompletionTimeText()
    {
        int completedLevels = PlayerPrefs.GetInt("CompletedPlayableLevels", 0);

        if (completedLevels <= 0)
            return "-";

        float totalTime = PlayerPrefs.GetFloat("TotalCompletionTime", 0f);
        float average = totalTime / completedLevels;

        return FormatTime(average);
    }

    private string FormatTime(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60f);
        int remainingSeconds = Mathf.FloorToInt(seconds % 60f);

        return $"{minutes:00}:{remainingSeconds:00}";
    }

    public static void UnlockCard(string unlockKey)
    {
        if (string.IsNullOrEmpty(unlockKey))
            return;

        PlayerPrefs.SetInt(unlockKey, 1);
        PlayerPrefs.Save();

        Debug.Log($"[Rewards] Unlocked card: {unlockKey}");
    }

    public static void RegisterLevelStats(int mistakes, float completionTimeSeconds)
    {
        int completedLevels = PlayerPrefs.GetInt("CompletedPlayableLevels", 0);
        int totalMistakes = PlayerPrefs.GetInt("TotalMistakes", 0);
        float totalTime = PlayerPrefs.GetFloat("TotalCompletionTime", 0f);

        completedLevels++;
        totalMistakes += Mathf.Max(0, mistakes);
        totalTime += Mathf.Max(0f, completionTimeSeconds);

        PlayerPrefs.SetInt("CompletedPlayableLevels", completedLevels);
        PlayerPrefs.SetInt("TotalMistakes", totalMistakes);
        PlayerPrefs.SetFloat("TotalCompletionTime", totalTime);
        PlayerPrefs.Save();

        Debug.Log($"[Rewards] Registered stats. Mistakes: {mistakes}, Time: {completionTimeSeconds}");
    }

    [ContextMenu("DEBUG Unlock Level 1 Nibbles")]
    private void DebugUnlockLevel1()
    {
        UnlockCard("Reward_Level1_Nibbles");
        RefreshAll();
    }

    [ContextMenu("DEBUG Unlock Level 5 Yeti")]
    private void DebugUnlockLevel5()
    {
        UnlockCard("Reward_Level5_Yeti");
        RefreshAll();
    }

    [ContextMenu("DEBUG Unlock Level 9 Achilles")]
    private void DebugUnlockLevel9()
    {
        UnlockCard("Reward_Level9_Achilles");
        RefreshAll();
    }

    [ContextMenu("DEBUG Give Level 1 Gold Astragalos")]
    private void DebugGiveLevel1GoldAstragalos()
    {
        PlayerPrefs.SetInt("Reward_Level1_Nibbles", 1);
        PlayerPrefs.SetInt("Reward_Level1_Nibbles_Rank", (int)CardRank.Gold);
        PlayerPrefs.SetInt("Reward_Level1_Nibbles_AstragalosBadge", 1);
        PlayerPrefs.Save();

        RefreshAll();

        Debug.Log("[Rewards] DEBUG: Level 1 Gold + Astragalos awarded.");
    }

    [ContextMenu("DEBUG Give Level 5 Silver")]
    private void DebugGiveLevel5Silver()
    {
        PlayerPrefs.SetInt("Reward_Level5_Yeti", 1);
        PlayerPrefs.SetInt("Reward_Level5_Yeti_Rank", (int)CardRank.Silver);
        PlayerPrefs.SetInt("Reward_Level5_Yeti_AstragalosBadge", 0);
        PlayerPrefs.Save();

        RefreshAll();

        Debug.Log("[Rewards] DEBUG: Level 5 Silver awarded.");
    }

    [ContextMenu("DEBUG Give Level 9 Bronze")]
    private void DebugGiveLevel9Bronze()
    {
        PlayerPrefs.SetInt("Reward_Level9_Achilles", 1);
        PlayerPrefs.SetInt("Reward_Level9_Achilles_Rank", (int)CardRank.Bronze);
        PlayerPrefs.SetInt("Reward_Level9_Achilles_AstragalosBadge", 0);
        PlayerPrefs.Save();

        RefreshAll();

        Debug.Log("[Rewards] DEBUG: Level 9 Bronze awarded.");
    }

    [ContextMenu("DEBUG Reset Rewards")]
    private void DebugResetRewards()
    {
        DeleteRewardKeys("Reward_Level1_Nibbles");
        DeleteRewardKeys("Reward_Level5_Yeti");
        DeleteRewardKeys("Reward_Level9_Achilles");

        PlayerPrefs.DeleteKey("CompletedPlayableLevels");
        PlayerPrefs.DeleteKey("TotalMistakes");
        PlayerPrefs.DeleteKey("TotalCompletionTime");

        PlayerPrefs.Save();
        RefreshAll();

        Debug.Log("[Rewards] Reset all rewards.");
    }

    private void DeleteRewardKeys(string rewardKey)
    {
        PlayerPrefs.DeleteKey(rewardKey);
        PlayerPrefs.DeleteKey(rewardKey + "_Rank");
        PlayerPrefs.DeleteKey(rewardKey + "_AstragalosBadge");
        PlayerPrefs.DeleteKey(rewardKey + "_Mistakes");
        PlayerPrefs.DeleteKey(rewardKey + "_CompletionTime");
    }
}