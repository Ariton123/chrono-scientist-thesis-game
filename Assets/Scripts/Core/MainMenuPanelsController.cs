using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuPanelsController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject instructionsPanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("Instructions Title")]
    [SerializeField] private TMP_Text instructionsTitleText;
    [SerializeField] private string instructionsTitleKey = "INSTRUCTIONS";

    [Header("Instructions Pages")]
    [SerializeField] private GameObject page1Content;
    [SerializeField] private GameObject page2Content;

    [Header("Instructions Page 1 Texts")]
    [SerializeField] private TMP_Text chapter1Text;
    [SerializeField] private TMP_Text chapter2Text;
    [SerializeField] private TMP_Text chapter3Text;

    [Header("Instructions Page 2 Texts")]
    [SerializeField] private TMP_Text storyText;
    [SerializeField] private TMP_Text timerMistakesText;
    [SerializeField] private TMP_Text discoveryCardsRanksText;
    [SerializeField] private TMP_Text astragalosText;

    [Header("Instructions Navigation")]
    [SerializeField] private Button previousPageButton;
    [SerializeField] private Button nextPageButton;

    [Header("Instructions Localization Keys - Page 1")]
    [SerializeField] private string chapter1Key = "INSTRUCTIONS_CHAPTER1_MECHANIC";
    [SerializeField] private string chapter2Key = "INSTRUCTIONS_CHAPTER2_MECHANIC";
    [SerializeField] private string chapter3Key = "INSTRUCTIONS_CHAPTER3_MECHANIC";

    [Header("Instructions Localization Keys - Page 2")]
    [SerializeField] private string storyKey = "INSTRUCTIONS_STORY";
    [SerializeField] private string timerMistakesKey = "INSTRUCTIONS_REWARDS_TIMER_MISTAKES";
    [SerializeField] private string discoveryCardsRanksKey = "INSTRUCTIONS_REWARDS_CARDS_RANKS";
    [SerializeField] private string astragalosKey = "INSTRUCTIONS_REWARDS_ASTRAGALOS";

    private int currentInstructionsPage = 1;
    private const int TotalInstructionsPages = 2;

    private void Awake()
    {
        WireInstructionButtons();
    }

    private void OnEnable()
    {
        if (LanguageManager.Instance != null)
            LanguageManager.Instance.OnLanguageChanged += HandleLanguageChanged;
    }

    private void OnDisable()
    {
        if (LanguageManager.Instance != null)
            LanguageManager.Instance.OnLanguageChanged -= HandleLanguageChanged;
    }

    private void WireInstructionButtons()
    {
        if (previousPageButton != null)
        {
            previousPageButton.onClick.RemoveListener(ShowPreviousInstructionsPage);
            previousPageButton.onClick.AddListener(ShowPreviousInstructionsPage);
        }

        if (nextPageButton != null)
        {
            nextPageButton.onClick.RemoveListener(ShowNextInstructionsPage);
            nextPageButton.onClick.AddListener(ShowNextInstructionsPage);
        }
    }

    public void OpenInstructions()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (instructionsPanel != null)
            instructionsPanel.SetActive(true);

        ShowInstructionsPage(1);
    }

    public void CloseInstructions()
    {
        if (instructionsPanel != null)
            instructionsPanel.SetActive(false);
    }

    public void OpenSettings()
    {
        if (instructionsPanel != null)
            instructionsPanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    public void CloseAllPanels()
    {
        if (instructionsPanel != null)
            instructionsPanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    public void ShowNextInstructionsPage()
    {
        ShowInstructionsPage(Mathf.Min(currentInstructionsPage + 1, TotalInstructionsPages));
    }

    public void ShowPreviousInstructionsPage()
    {
        ShowInstructionsPage(Mathf.Max(currentInstructionsPage - 1, 1));
    }

    private void ShowInstructionsPage(int page)
    {
        currentInstructionsPage = Mathf.Clamp(page, 1, TotalInstructionsPages);

        if (page1Content != null)
            page1Content.SetActive(currentInstructionsPage == 1);

        if (page2Content != null)
            page2Content.SetActive(currentInstructionsPage == 2);

        if (previousPageButton != null)
            previousPageButton.gameObject.SetActive(currentInstructionsPage > 1);

        if (nextPageButton != null)
            nextPageButton.gameObject.SetActive(currentInstructionsPage < TotalInstructionsPages);

        RefreshInstructionsTexts();
    }

    private void HandleLanguageChanged(GameLanguage language)
    {
        RefreshInstructionsTexts();
    }

    private void RefreshInstructionsTexts()
    {
        RefreshInstructionsTitle();
        RefreshPage1Texts();
        RefreshPage2Texts();
    }

    private void RefreshInstructionsTitle()
    {
        if (instructionsTitleText == null)
            return;

        string title = GetLocalizedText(instructionsTitleKey, "INSTRUCTIONS");
        instructionsTitleText.text = $"{title} {currentInstructionsPage}/{TotalInstructionsPages}";
    }

    private void RefreshPage1Texts()
    {
        if (chapter1Text != null)
        {
            chapter1Text.text = GetLocalizedText(
                chapter1Key,
                "Drag each bone to its correct place in the skeleton.\n\nTap on a bone to learn its name and what it does."
            );
        }

        if (chapter2Text != null)
        {
            chapter2Text.text = GetLocalizedText(
                chapter2Key,
                "Drag each bone to the correct function.\n\nMatch bones with what they do in the body and complete all matches to finish the level."
            );
        }

        if (chapter3Text != null)
        {
            chapter3Text.text = GetLocalizedText(
                chapter3Key,
                "Drag each amulet to the correct meaning.\n\nLook at the amulet clues, match them with what they represent, and complete all matches to finish the level."
            );
        }
    }

    private void RefreshPage2Texts()
    {
        if (storyText != null)
        {
            storyText.text = GetLocalizedText(
                storyKey,
                "You are a young apprentice in Darwin’s lab. With the Chrono Engine, you travel through time to discover how bones connect anatomy, movement, history, and culture."
            );
        }

        if (timerMistakesText != null)
        {
            timerMistakesText.text = GetLocalizedText(
                timerMistakesKey,
                "The timer shows how much time is left. The mistake counter shows how many wrong attempts you made."
            );
        }

        if (discoveryCardsRanksText != null)
        {
            discoveryCardsRanksText.text = GetLocalizedText(
                discoveryCardsRanksKey,
                "After each mission, you unlock a Discovery Card. Cards help you review what you learned and can be Bronze, Silver, or Gold depending on your performance."
            );
        }

        if (astragalosText != null)
        {
            astragalosText.text = GetLocalizedText(
                astragalosKey,
                "Earn a Gold card with 1 mistake or less to receive the special Golden Astragalos badge."
            );
        }
    }

    private string GetLocalizedText(string key, string fallback)
    {
        if (!string.IsNullOrEmpty(key) && LanguageManager.Instance != null)
            return LanguageManager.Instance.GetText(key);

        return fallback;
    }
}