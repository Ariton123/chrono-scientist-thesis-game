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
    private enum ConfirmationAction
    {
        None,
        ResetRewards,
        DownloadCsv
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
        public Sprite openedCardBackSprite;

        [Tooltip("Fallback title if no localization key is assigned.")]
        public string openedCardTitle;

        [Tooltip("Optional localization key for opened card title.")]
        public string openedCardTitleKey;

        [Header("Opened Card Front / Back Text")]
        [Tooltip("Fallback front text if no localization key is assigned.")]
        [TextArea(3, 10)]
        public string openedCardFrontDescription;

        [Tooltip("Optional localization key for front text.")]
        public string openedCardFrontDescriptionKey;

        [Tooltip("Fallback back intro text if no localization key is assigned.")]
        [TextArea(5, 14)]
        public string openedCardBackDescription;

        [Tooltip("Optional localization key for back intro text.")]
        public string openedCardBackDescriptionKey;

        [Header("Opened Card Astragalos Badge")]
        public Sprite blankAstragalosBadgeSprite;
        public Sprite earnedAstragalosBadgeSprite;

        [Header("Opened Card Back Page Icons")]
        [Tooltip("Sprites shown inside the shared back-side BoneIconGroup when this card is opened.")]
        public Sprite[] backBoneIconSprites;

        [Header("Opened Card Back Page Row Texts")]
        [Tooltip("Localization keys for the row texts shown beside the back-side icons.")]
        public string[] backRowTextKeys;

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

    [Header("Page Navigation")]
    [SerializeField] private Button nextPageButton;
    [SerializeField] private Button previousPageButton;

    [Header("Discovery Cards")]
    [SerializeField] private DiscoveryCard[] discoveryCards;

    [Header("Opened Card Popup")]
    [Tooltip("Assign the full OpenedCardPopup object here, not only the inner card panel.")]
    [SerializeField] private GameObject openedCardPopup;

    [SerializeField] private Image openedCardImage;
    [SerializeField] private TMP_Text openedCardTitleText;

    [Header("Opened Card Text Areas")]
    [SerializeField] private TMP_Text openedFrontDescriptionText;
    [SerializeField] private TMP_Text openedBackDescriptionText;

    [Header("Opened Card Front / Back Controls")]
    [SerializeField] private Button flipCardButton;
    [SerializeField] private TMP_Text flipCardButtonText;
    [SerializeField] private TMP_Text openedPageSideText;

    [Header("Opened Card Page Visual Groups")]
    [Tooltip("One shared group used for all opened-card back-side bone icons and row texts.")]
    [SerializeField] private GameObject backBoneIconsGroup;

    [Tooltip("Image slots inside the shared BoneIconGroup. The script swaps their sprites per card.")]
    [SerializeField] private Image[] backBoneIconImages;

    [Tooltip("TMP row texts inside the shared BoneIconGroup. The script swaps their localization keys per card.")]
    [SerializeField] private TMP_Text[] backRowTexts;

    [SerializeField] private GameObject astragalosBadgeGroup;

    [Header("Opened Card Performance")]
    [Tooltip("This is the RankTintOverlay image. It swaps Gold/Silver/Bronze sprites.")]
    [SerializeField] private Image rankTintOverlay;

    [SerializeField] private TMP_Text openedRankText;
    [SerializeField] private Image openedAstragalosBadgeImage;

    [Header("Rank Overlay Sprites")]
    [SerializeField] private Sprite goldRankOverlaySprite;
    [SerializeField] private Sprite silverRankOverlaySprite;
    [SerializeField] private Sprite bronzeRankOverlaySprite;

    [Header("Opened Card Animation")]
    [SerializeField] private UIPopupCardAnimator openedCardAnimator;

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

    [Header("Evaluation Controls")]
    [SerializeField] private Button resetRewardsButton;
    [SerializeField] private TMP_Text resetRewardsButtonText;

    [SerializeField] private Button downloadCsvButton;
    [SerializeField] private TMP_Text downloadCsvButtonText;

    [Header("Confirmation Overlay")]
    [SerializeField] private GameObject confirmationOverlay;
    [SerializeField] private TMP_Text confirmationMessageText;

    [SerializeField] private Button confirmationYesButton;
    [SerializeField] private TMP_Text confirmationYesButtonText;

    [SerializeField] private Button confirmationNoButton;
    [SerializeField] private TMP_Text confirmationNoButtonText;

    [Header("Opened Card Flip Animation")]
    [SerializeField] private UICardFlipAnimator cardFlipAnimator;

    private RewardsTab currentTab = RewardsTab.Rewards;

    private DiscoveryCard currentOpenedCard;
    private bool showingBackSide = false;
    private bool currentOpenedCardUsesLatestRunStats = false;
    private ConfirmationAction pendingConfirmationAction = ConfirmationAction.None;

    private const int TotalPossibleDiscoveryCards = 3;
    private const int TotalPossibleSideCharacters = 3;
    private const int TotalPossibleAstragalosBadges = 3;

    private void Awake()
    {
        if (rewardsPanel != null)
            rewardsPanel.SetActive(false);

        if (openedCardPopup != null)
            openedCardPopup.SetActive(false);

        if (confirmationOverlay != null)
            confirmationOverlay.SetActive(false);

        HideBackBoneIcons();

        if (openedCardAnimator == null && openedCardPopup != null)
            openedCardAnimator = openedCardPopup.GetComponentInChildren<UIPopupCardAnimator>(true);

        WireButtons();
    }

    private void OnEnable()
    {
        if (LanguageManager.Instance != null)
            LanguageManager.Instance.OnLanguageChanged += HandleLanguageChanged;

        RefreshAll();
    }

    private void OnDisable()
    {
        if (LanguageManager.Instance != null)
            LanguageManager.Instance.OnLanguageChanged -= HandleLanguageChanged;
    }

    private void HandleLanguageChanged(GameLanguage language)
    {
        RefreshAll();

        if (currentOpenedCard != null)
            RefreshOpenedCardSideUI();

        if (confirmationOverlay != null && confirmationOverlay.activeSelf)
            RefreshConfirmationMessage();
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

        if (nextPageButton != null)
        {
            nextPageButton.onClick.RemoveListener(ShowDetailsTab);
            nextPageButton.onClick.AddListener(ShowDetailsTab);
        }

        if (previousPageButton != null)
        {
            previousPageButton.onClick.RemoveListener(ShowRewardsTab);
            previousPageButton.onClick.AddListener(ShowRewardsTab);
        }

        if (flipCardButton != null)
        {
            flipCardButton.onClick.RemoveListener(ToggleOpenedCardSide);
            flipCardButton.onClick.AddListener(ToggleOpenedCardSide);
        }

        if (resetRewardsButton != null)
        {
            resetRewardsButton.onClick.RemoveListener(RequestResetRewards);
            resetRewardsButton.onClick.AddListener(RequestResetRewards);
        }

        if (downloadCsvButton != null)
        {
            downloadCsvButton.onClick.RemoveListener(RequestDownloadCsv);
            downloadCsvButton.onClick.AddListener(RequestDownloadCsv);
        }

        if (confirmationYesButton != null)
        {
            confirmationYesButton.onClick.RemoveListener(ConfirmPendingAction);
            confirmationYesButton.onClick.AddListener(ConfirmPendingAction);
        }

        if (confirmationNoButton != null)
        {
            confirmationNoButton.onClick.RemoveListener(CloseConfirmationOverlay);
            confirmationNoButton.onClick.AddListener(CloseConfirmationOverlay);
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
        CloseConfirmationOverlay();

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
            CloseConfirmationOverlay();
        }
    }

    public void OpenDiscoveryCardByUnlockKey(string rewardUnlockKey, bool forceOpen = false, bool useLatestRunStats = false)
    {
        if (string.IsNullOrEmpty(rewardUnlockKey))
        {
            Debug.LogWarning("[Rewards] Cannot open discovery card. Reward unlock key is empty.");
            return;
        }

        if (discoveryCards == null || discoveryCards.Length == 0)
        {
            Debug.LogWarning("[Rewards] Cannot open discovery card. No discovery cards assigned.");
            return;
        }

        // Important for post-dialogue scenes:
        // If the opened card popup is inside the rewards panel,
        // the parent panel must be active or the popup will not be visible.
        if (rewardsPanel != null)
        {
            rewardsPanel.SetActive(true);
            rewardsPanel.transform.SetAsLastSibling();
        }

        transform.SetAsLastSibling();

        RefreshAll();

        foreach (DiscoveryCard card in discoveryCards)
        {
            if (card == null)
                continue;

            if (card.unlockPlayerPrefsKey == rewardUnlockKey)
            {
                TryOpenCard(card, forceOpen, useLatestRunStats);
                return;
            }
        }

        Debug.LogWarning($"[Rewards] No discovery card found with unlock key: {rewardUnlockKey}");
    }

    public void CloseOpenedCard()
    {
        currentOpenedCard = null;
        showingBackSide = false;
        currentOpenedCardUsesLatestRunStats = false;

        HideBackBoneIcons();

        if (openedCardTitleText != null)
            openedCardTitleText.gameObject.SetActive(true);

        if (astragalosBadgeGroup != null)
            astragalosBadgeGroup.SetActive(true);

        if (openedFrontDescriptionText != null)
            openedFrontDescriptionText.gameObject.SetActive(true);

        if (openedBackDescriptionText != null)
            openedBackDescriptionText.gameObject.SetActive(false);

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
        RefreshEvaluationControlsLocalization();
    }



    private void RefreshTitleSwitch()
    {
        if (rewardsTitleText != null)
            rewardsTitleText.text = GetLocalizedText("REWARDS", "REWARDS");

        if (slashTitleText != null)
        {
            slashTitleText.text = "|";
            slashTitleText.color = slashTitleColor;
        }

        if (detailsTitleText != null)
            detailsTitleText.text = GetLocalizedText("DETAILS", "DETAILS");
    }

    private void RefreshTabs()
    {
        bool rewardsSelected = currentTab == RewardsTab.Rewards;

        if (rewardsContent != null)
            rewardsContent.SetActive(rewardsSelected);

        if (detailsContent != null)
            detailsContent.SetActive(!rewardsSelected);

        if (nextPageButton != null)
            nextPageButton.gameObject.SetActive(rewardsSelected);

        if (previousPageButton != null)
            previousPageButton.gameObject.SetActive(!rewardsSelected);

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
                card.cardTitleText.text = GetCardTitle(card);
            }
        }
    }

    private void TryOpenCard(DiscoveryCard card, bool forceOpen = false, bool useLatestRunStats = false)
    {
        if (card == null)
            return;

        if (!forceOpen && !IsCardUnlocked(card))
            return;

        currentOpenedCard = card;
        showingBackSide = false;
        currentOpenedCardUsesLatestRunStats = useLatestRunStats;

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
        {
            openedCardTitleText.gameObject.SetActive(true);
            openedCardTitleText.text = GetCardTitle(card);
        }

        HideBackBoneIcons();

        CardRank earnedRank = GetSavedRank(card);
        bool earnedAstragalos = HasEarnedAstragalosBadge(card);

        ApplyOpenedCardPerformance(card, earnedRank, earnedAstragalos);
        ShowOpenedCardFront();

        if (openedCardAnimator == null && openedCardPopup != null)
            openedCardAnimator = openedCardPopup.GetComponentInChildren<UIPopupCardAnimator>(true);

        if (openedCardAnimator != null)
            openedCardAnimator.PlayReveal();
    }

    public void ToggleOpenedCardSide()
    {
        if (currentOpenedCard == null)
            return;

        if (cardFlipAnimator != null)
        {
            cardFlipAnimator.PlayFlip(() =>
            {
                showingBackSide = !showingBackSide;

                if (showingBackSide)
                    ShowOpenedCardBack();
                else
                    ShowOpenedCardFront();
            });

            return;
        }

        showingBackSide = !showingBackSide;

        if (showingBackSide)
            ShowOpenedCardBack();
        else
            ShowOpenedCardFront();
    }

    private void ShowOpenedCardFront()
    {
        showingBackSide = false;
        RefreshOpenedCardSideUI();
    }

    private void ShowOpenedCardBack()
    {
        showingBackSide = true;
        RefreshOpenedCardSideUI();
    }

    private void RefreshOpenedCardSideUI()
    {
        if (currentOpenedCard == null)
        {
            HideBackBoneIcons();
            return;
        }

        if (openedCardImage != null)
        {
            Sprite sideSprite = showingBackSide && currentOpenedCard.openedCardBackSprite != null
                ? currentOpenedCard.openedCardBackSprite
                : currentOpenedCard.openedCardSprite;

            openedCardImage.sprite = sideSprite;
            openedCardImage.enabled = sideSprite != null;
            openedCardImage.preserveAspect = true;
            openedCardImage.color = Color.white;
        }

        if (openedFrontDescriptionText != null)
        {
            openedFrontDescriptionText.gameObject.SetActive(!showingBackSide);
            openedFrontDescriptionText.text = GetCardFrontDescription(currentOpenedCard);
        }

        if (openedBackDescriptionText != null)
        {
            openedBackDescriptionText.gameObject.SetActive(showingBackSide);
            openedBackDescriptionText.text = GetCardBackDescription(currentOpenedCard);
        }

        if (showingBackSide)
            ShowBackContentForCard(currentOpenedCard);
        else
            HideBackBoneIcons();

        if (astragalosBadgeGroup != null)
            astragalosBadgeGroup.SetActive(!showingBackSide);

        if (openedCardTitleText != null)
            openedCardTitleText.gameObject.SetActive(!showingBackSide);

        if (openedPageSideText != null)
        {
            openedPageSideText.text = showingBackSide
                ? GetLocalizedText("BACK_PAGE", "BACK PAGE")
                : GetLocalizedText("FRONT_PAGE", "FRONT PAGE");
        }

        if (flipCardButtonText != null)
        {
            flipCardButtonText.text = GetLocalizedText("FLIP", "FLIP");
        }

        RefreshOpenedRankText();
    }

    private void HideBackBoneIcons()
    {
        if (backBoneIconsGroup != null)
            backBoneIconsGroup.SetActive(false);

        if (backBoneIconImages != null)
        {
            foreach (Image iconImage in backBoneIconImages)
            {
                if (iconImage == null)
                    continue;

                iconImage.sprite = null;
                iconImage.enabled = false;
                iconImage.gameObject.SetActive(false);
            }
        }

        HideBackRowTexts();
    }

    private void HideBackRowTexts()
    {
        if (backRowTexts == null)
            return;

        foreach (TMP_Text rowText in backRowTexts)
        {
            if (rowText == null)
                continue;

            rowText.text = "";
            rowText.gameObject.SetActive(false);
        }
    }

    private void ShowBackContentForCard(DiscoveryCard card)
    {
        if (card == null)
        {
            HideBackBoneIcons();
            return;
        }

        if (backBoneIconsGroup != null)
            backBoneIconsGroup.SetActive(true);

        int iconCount = backBoneIconImages != null ? backBoneIconImages.Length : 0;
        int rowCount = backRowTexts != null ? backRowTexts.Length : 0;
        int maxRows = Mathf.Max(iconCount, rowCount);

        for (int i = 0; i < maxRows; i++)
        {
            RefreshBackIconAtIndex(card, i);
            RefreshBackRowTextAtIndex(card, i);
        }
    }

    private void RefreshBackIconAtIndex(DiscoveryCard card, int index)
    {
        if (backBoneIconImages == null || index < 0 || index >= backBoneIconImages.Length)
            return;

        Image iconImage = backBoneIconImages[index];

        if (iconImage == null)
            return;

        bool hasSprite =
            card.backBoneIconSprites != null &&
            index < card.backBoneIconSprites.Length &&
            card.backBoneIconSprites[index] != null;

        iconImage.sprite = hasSprite ? card.backBoneIconSprites[index] : null;
        iconImage.enabled = hasSprite;
        iconImage.gameObject.SetActive(hasSprite);
        iconImage.preserveAspect = true;
        iconImage.color = Color.white;
    }

    private void RefreshBackRowTextAtIndex(DiscoveryCard card, int index)
    {
        if (backRowTexts == null || index < 0 || index >= backRowTexts.Length)
            return;

        TMP_Text rowText = backRowTexts[index];

        if (rowText == null)
            return;

        bool hasKey =
            card.backRowTextKeys != null &&
            index < card.backRowTextKeys.Length &&
            !string.IsNullOrEmpty(card.backRowTextKeys[index]);

        rowText.text = hasKey
            ? GetLocalizedText(card.backRowTextKeys[index], "")
            : "";

        rowText.gameObject.SetActive(hasKey);
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

        string rankKey = currentOpenedCardUsesLatestRunStats
            ? card.unlockPlayerPrefsKey + "_LastRank"
            : card.unlockPlayerPrefsKey + "_Rank";

        if (!PlayerPrefs.HasKey(rankKey))
            return card.rank;

        return (CardRank)PlayerPrefs.GetInt(rankKey, (int)card.rank);
    }

    private bool HasEarnedAstragalosBadge(DiscoveryCard card)
    {
        if (card == null || string.IsNullOrEmpty(card.unlockPlayerPrefsKey))
            return false;

        string badgeKey = currentOpenedCardUsesLatestRunStats
            ? card.unlockPlayerPrefsKey + "_LastAstragalosBadge"
            : card.unlockPlayerPrefsKey + "_AstragalosBadge";

        return PlayerPrefs.GetInt(badgeKey, 0) == 1;
    }

    private void RefreshOpenedRankText()
    {
        if (openedRankText == null || currentOpenedCard == null)
            return;

        CardRank rank = GetSavedRank(currentOpenedCard);

        string rankKey = GetRankLocalizationKey(rank);
        string localizedRank = GetLocalizedText(rankKey, rank.ToString().ToUpper());
        string rankLabel = GetLocalizedText("CARD_RANK", "CARD RANK");

        openedRankText.text = $"{rankLabel}: {localizedRank}";
    }

    private void ApplyOpenedCardPerformance(DiscoveryCard card, CardRank rank, bool earnedAstragalos)
    {
        if (rankTintOverlay != null)
        {
            rankTintOverlay.raycastTarget = false;
            rankTintOverlay.preserveAspect = false;
            rankTintOverlay.color = Color.white;

            switch (rank)
            {
                case CardRank.Gold:
                    rankTintOverlay.sprite = goldRankOverlaySprite;
                    break;

                case CardRank.Silver:
                    rankTintOverlay.sprite = silverRankOverlaySprite;
                    break;

                case CardRank.Bronze:
                    rankTintOverlay.sprite = bronzeRankOverlaySprite;
                    break;
            }

            rankTintOverlay.enabled = rankTintOverlay.sprite != null;
        }

        RefreshOpenedRankText();

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

    private string GetRankLocalizationKey(CardRank rank)
    {
        switch (rank)
        {
            case CardRank.Gold:
                return "GOLD";

            case CardRank.Silver:
                return "SILVER";

            case CardRank.Bronze:
                return "BRONZE";

            default:
                return "BRONZE";
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
            totalDiscoveryCardsText.text = $"{GetLocalizedText("TOTAL_DISCOVERY_CARDS", "Total Discovery Cards")}: {unlockedCards}/{TotalPossibleDiscoveryCards}";

        if (goldCardsText != null)
            goldCardsText.text = $"{GetLocalizedText("GOLD", "Gold")}: {gold}";

        if (silverCardsText != null)
            silverCardsText.text = $"{GetLocalizedText("SILVER", "Silver")}: {silver}";

        if (bronzeCardsText != null)
            bronzeCardsText.text = $"{GetLocalizedText("BRONZE", "Bronze")}: {bronze}";

        if (averageMistakesText != null)
            averageMistakesText.text = $"{GetLocalizedText("AVERAGE_MISTAKES_PER_LEVEL", "Average number of mistakes per level")}: {GetAverageMistakesText()}";

        if (averageCompletionTimeText != null)
            averageCompletionTimeText.text = $"{GetLocalizedText("AVERAGE_COMPLETION_TIME", "Average Level Completion Time")}: {GetAverageCompletionTimeText()}";

        if (sideCharactersUnlockedText != null)
            sideCharactersUnlockedText.text = $"{GetLocalizedText("SIDE_CHARACTERS_UNLOCKED", "Side Characters unlocked")}: {sideCharacters}/{TotalPossibleSideCharacters}";

        if (astragalosBadgesText != null)
            astragalosBadgesText.text = $"{GetLocalizedText("ASTRAGALOS_BADGE", "Astragalos Badge")}: {astragalosBadges}/{TotalPossibleAstragalosBadges}";
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

    private string GetCardTitle(DiscoveryCard card)
    {
        if (card == null)
            return "";

        return GetLocalizedText(card.openedCardTitleKey, card.openedCardTitle);
    }

    private string GetCardFrontDescription(DiscoveryCard card)
    {
        if (card == null)
            return "";

        string baseDescription = GetLocalizedText(
            card.openedCardFrontDescriptionKey,
            card.openedCardFrontDescription
        );

        string performanceText = GetCardPerformanceText(card);

        if (string.IsNullOrEmpty(performanceText))
            return baseDescription;

        return $"{baseDescription}\n\n{performanceText}";
    }

    private string GetCardPerformanceText(DiscoveryCard card)
    {
        if (card == null || string.IsNullOrEmpty(card.unlockPlayerPrefsKey))
            return "";

        string mistakesKey = currentOpenedCardUsesLatestRunStats
            ? card.unlockPlayerPrefsKey + "_LastMistakes"
            : card.unlockPlayerPrefsKey + "_Mistakes";

        string timeKey = currentOpenedCardUsesLatestRunStats
            ? card.unlockPlayerPrefsKey + "_LastCompletionTime"
            : card.unlockPlayerPrefsKey + "_CompletionTime";

        bool hasMistakes = PlayerPrefs.HasKey(mistakesKey);
        bool hasTime = PlayerPrefs.HasKey(timeKey);

        if (!hasMistakes && !hasTime)
            return "";

        int mistakes = PlayerPrefs.GetInt(mistakesKey, 0);
        float completionTime = PlayerPrefs.GetFloat(timeKey, 0f);

        string completedLabel = currentOpenedCardUsesLatestRunStats
            ? GetLocalizedText("LEVEL_COMPLETED", "Level completed")
            : GetLocalizedText("BEST_COMPLETION_TIME", "Best completion time");

        string mistakesLabel = currentOpenedCardUsesLatestRunStats
            ? GetLocalizedText("NUMBER_OF_MISTAKES", "Number of mistakes")
            : GetLocalizedText("FEWEST_MISTAKES", "Fewest mistakes");

        return $"<b>{completedLabel}:</b> {FormatTimeLongLocalized(completionTime)}\n<b>{mistakesLabel}:</b> {mistakes}";
    }

    private string FormatTimeLongLocalized(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60f);
        int remainingSeconds = Mathf.FloorToInt(seconds % 60f);

        string minLabel = GetLocalizedText("MIN", "min");
        string secLabel = GetLocalizedText("SEC", "sec");

        if (minutes <= 0)
            return $"{remainingSeconds} {secLabel}";

        return $"{minutes} {minLabel} {remainingSeconds} {secLabel}";
    }

    private string GetCardBackDescription(DiscoveryCard card)
    {
        if (card == null)
            return "";

        return GetLocalizedText(card.openedCardBackDescriptionKey, card.openedCardBackDescription);
    }

    private string GetLocalizedText(string key, string fallback)
    {
        if (!string.IsNullOrEmpty(key) && LanguageManager.Instance != null)
            return LanguageManager.Instance.GetText(key);

        return fallback;
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

    public void RequestResetRewards()
    {
        ShowConfirmationOverlay(ConfirmationAction.ResetRewards);
    }

    private void RefreshEvaluationControlsLocalization()
    {
        if (resetRewardsButtonText != null)
            resetRewardsButtonText.text = GetLocalizedText("RESET_REWARDS", "Reset rewards");

        if (downloadCsvButtonText != null)
            downloadCsvButtonText.text = GetLocalizedText("DOWNLOAD_CSV", "Download CSV");

        if (confirmationYesButtonText != null)
            confirmationYesButtonText.text = GetLocalizedText("YES", "Yes");

        if (confirmationNoButtonText != null)
            confirmationNoButtonText.text = GetLocalizedText("NO", "No");
    }

    public void RequestDownloadCsv()
    {
        ShowConfirmationOverlay(ConfirmationAction.DownloadCsv);
    }

    private void ShowConfirmationOverlay(ConfirmationAction action)
    {
        pendingConfirmationAction = action;

        if (confirmationOverlay != null)
        {
            confirmationOverlay.SetActive(true);
            confirmationOverlay.transform.SetAsLastSibling();
        }

        RefreshEvaluationControlsLocalization();
        RefreshConfirmationMessage();
    }

    private void RefreshConfirmationMessage()
    {
        if (confirmationMessageText == null)
            return;

        confirmationMessageText.text = GetConfirmationMessage(pendingConfirmationAction);
        confirmationMessageText.ForceMeshUpdate();
    }

    private string GetConfirmationMessage(ConfirmationAction action)
    {
        switch (action)
        {
            case ConfirmationAction.ResetRewards:
                return GetLocalizedText(
                    "CONFIRM_RESET_REWARDS",
                    "Are you sure you want to reset your rewards and progress?"
                );

            case ConfirmationAction.DownloadCsv:
                return GetLocalizedText(
                    "CONFIRM_DOWNLOAD_CSV",
                    "Do you want to download the CSV log file?"
                );

            default:
                return "";
        }
    }

    private void CloseConfirmationOverlay()
    {
        pendingConfirmationAction = ConfirmationAction.None;

        if (confirmationOverlay != null)
            confirmationOverlay.SetActive(false);
    }

    private void ConfirmPendingAction()
    {
        ConfirmationAction actionToRun = pendingConfirmationAction;

        CloseConfirmationOverlay();

        switch (actionToRun)
        {
            case ConfirmationAction.ResetRewards:
                ResetRewardsAndProgressFromPanel();
                break;

            case ConfirmationAction.DownloadCsv:
                SessionCSVLogger.DownloadCsvFile();
                break;
        }
    }

    public void ResetRewardsAndProgressFromPanel()
    {
        SessionCSVLogger.LogEvent(
            "REWARDS_RESET_CONFIRMED",
            extra: "Rewards/progress reset from Rewards panel."
        );

        DeleteRewardKeys("Reward_Level1_Nibbles");
        DeleteRewardKeys("Reward_Level5_Yeti");
        DeleteRewardKeys("Reward_Level9_Achilles");

        PlayerPrefs.DeleteKey("CompletedPlayableLevels");
        PlayerPrefs.DeleteKey("TotalMistakes");
        PlayerPrefs.DeleteKey("TotalCompletionTime");

        PlayerPrefs.Save();

        CloseOpenedCard();
        RefreshAll();

        Debug.Log("[Rewards] Rewards and progress reset from Rewards panel.");
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
        ResetRewardsAndProgressFromPanel();
    }

    private void DeleteRewardKeys(string rewardKey)
    {
        PlayerPrefs.DeleteKey(rewardKey);
        PlayerPrefs.DeleteKey(rewardKey + "_Rank");
        PlayerPrefs.DeleteKey(rewardKey + "_AstragalosBadge");
        PlayerPrefs.DeleteKey(rewardKey + "_Mistakes");
        PlayerPrefs.DeleteKey(rewardKey + "_CompletionTime");
        PlayerPrefs.DeleteKey(rewardKey + "_LastMistakes");
        PlayerPrefs.DeleteKey(rewardKey + "_LastCompletionTime");
        PlayerPrefs.DeleteKey(rewardKey + "_LastRank");
        PlayerPrefs.DeleteKey(rewardKey + "_LastAstragalosBadge");
    }
}