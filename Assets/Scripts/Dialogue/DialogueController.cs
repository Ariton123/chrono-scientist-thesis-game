using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    [Serializable]
    public class Line
    {
        [Header("Speaker Position")]
        public bool speakerOnLeft = true;

        [Header("Normal Character Sprites")]
        public Sprite leftSprite;
        public Sprite rightSprite;

        [Header("Selected Player Character")]
        public bool leftUsesSelectedPlayer = false;
        public bool rightUsesSelectedPlayer = false;
        public PlayerSpriteExpression selectedPlayerExpression = PlayerSpriteExpression.Normal;

        [Header("Localization Key")]
        public string dialogueKey;

        [Header("Mascot")]
        public bool showMusowl = false;
        public Sprite musowlSprite;

        [Header("Special Objects")]
        public bool showChronoEngine = false;
    }

    [Header("UI")]
    public TMP_Text dialogueText;
    public TMP_Text nextButtonText;
    public TMP_Text skipButtonText;
    public Button skipButton;

    [Header("Typewriter")]
    [SerializeField] private TypewriterTextUI dialogueTypewriter;

    [Header("Character Images")]
    public Image leftCharacter;
    public Image rightCharacter;
    public Image musowl;

    [Header("Special Objects")]
    [SerializeField] private GameObject chronoEngineObject;

    [Header("Lines")]
    public Line[] lines;

    [Header("After Dialogue")]
    public int sceneToLoadAfterDialogue = 2;

    [Header("Reward Unlock After Dialogue")]
    [SerializeField] private bool unlockRewardAfterDialogue = false;
    [SerializeField] private string rewardUnlockKey;

    [Header("Return Flow")]
    [SerializeField] private bool openPlayPanelAfterDialogue = false;

    [Header("Skip Settings")]
    [SerializeField] private bool allowSkipping = true;
    [SerializeField] private bool hideSkipButtonOnLastLine = true;

    private int index = 0;
    private bool dialogueEnding = false;

    private const string OpenPlayPanelPrefKey = "OpenPlayPanelOnLoad";

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

    private void Start()
    {
        index = 0;
        dialogueEnding = false;

        if (dialogueTypewriter == null && dialogueText != null)
            dialogueTypewriter = dialogueText.GetComponent<TypewriterTextUI>();

        if (chronoEngineObject != null)
            chronoEngineObject.SetActive(false);

        if (skipButton != null)
        {
            skipButton.interactable = allowSkipping;
            skipButton.gameObject.SetActive(allowSkipping);
        }

        ShowLine();
    }

    public void Next()
    {
        if (dialogueEnding)
            return;

        if (dialogueTypewriter != null)
            dialogueTypewriter.StopTyping();

        Debug.Log($"[Dialogue] Next clicked. Current index before increment: {index}");

        index++;

        Debug.Log($"[Dialogue] Index after increment: {index}");

        if (lines == null || index >= lines.Length)
        {
            EndDialogue();
            return;
        }

        ShowLine();
    }

    public void SkipDialogue()
    {
        if (dialogueEnding)
            return;

        if (!allowSkipping)
            return;

        if (dialogueTypewriter != null)
            dialogueTypewriter.StopTyping();

        Debug.Log("[Dialogue] Skip Dialogue clicked. Ending dialogue immediately.");

        EndDialogue();
    }

    private void EndDialogue()
    {
        if (dialogueEnding)
            return;

        dialogueEnding = true;

        Debug.Log("[Dialogue] Reached end. Processing after-dialogue flow.");

        if (chronoEngineObject != null)
            chronoEngineObject.SetActive(false);

        if (skipButton != null)
            skipButton.interactable = false;

        if (unlockRewardAfterDialogue && !string.IsNullOrEmpty(rewardUnlockKey))
        {
            RewardsPanelController.UnlockCard(rewardUnlockKey);
            Debug.Log($"[Dialogue] Reward unlocked: {rewardUnlockKey}");
        }

        if (openPlayPanelAfterDialogue)
        {
            PlayerPrefs.SetInt(OpenPlayPanelPrefKey, 1);
            PlayerPrefs.Save();

            Debug.Log("[Dialogue] Main Menu will reopen PlayPanel.");
        }

        SceneManager.LoadScene(sceneToLoadAfterDialogue);
    }

    private void HandleLanguageChanged(GameLanguage language)
    {
        ShowLine();
    }

    private void ShowLine()
    {
        if (lines == null || lines.Length == 0)
        {
            if (dialogueTypewriter != null)
                dialogueTypewriter.StopTyping();

            if (dialogueText != null)
                dialogueText.text = "(NO LINES SET)";

            if (chronoEngineObject != null)
                chronoEngineObject.SetActive(false);

            UpdateNextButtonText();
            UpdateSkipButtonText();
            UpdateSkipButtonVisibility();

            return;
        }

        if (index < 0 || index >= lines.Length)
            return;

        Line line = lines[index];

        Debug.Log($"[Dialogue] Showing line {index} with key: '{line.dialogueKey}'");

        if (dialogueText != null)
        {
            string resolvedText = LanguageManager.Instance != null
                ? LanguageManager.Instance.GetText(line.dialogueKey)
                : line.dialogueKey;

            Debug.Log($"[Dialogue] Resolved text: '{resolvedText}'");

            if (dialogueTypewriter != null)
                dialogueTypewriter.Play(resolvedText);
            else
                dialogueText.text = resolvedText;
        }

        ApplyCharacterSprite(
            leftCharacter,
            line.leftSprite,
            line.leftUsesSelectedPlayer,
            line.selectedPlayerExpression
        );

        ApplyCharacterSprite(
            rightCharacter,
            line.rightSprite,
            line.rightUsesSelectedPlayer,
            line.selectedPlayerExpression
        );

        if (musowl != null)
        {
            musowl.gameObject.SetActive(line.showMusowl);

            if (line.showMusowl && line.musowlSprite != null)
                musowl.sprite = line.musowlSprite;
        }

        if (chronoEngineObject != null)
            chronoEngineObject.SetActive(line.showChronoEngine);

        UpdateNextButtonText();
        UpdateSkipButtonText();
        UpdateSkipButtonVisibility();
    }

    private void ApplyCharacterSprite(
        Image characterImage,
        Sprite normalSprite,
        bool usesSelectedPlayer,
        PlayerSpriteExpression selectedPlayerExpression)
    {
        if (characterImage == null)
            return;

        CharacterSpriteSwitcher spriteSwitcher = characterImage.GetComponent<CharacterSpriteSwitcher>();

        if (usesSelectedPlayer)
        {
            characterImage.gameObject.SetActive(true);

            if (spriteSwitcher != null)
            {
                spriteSwitcher.enabled = true;
                spriteSwitcher.RefreshSprite(selectedPlayerExpression);
            }
            else
            {
                Debug.LogWarning($"[Dialogue] {characterImage.name} is marked as selected player, but no CharacterSpriteSwitcher is attached.");
            }

            characterImage.color = Color.white;
            return;
        }

        if (spriteSwitcher != null)
            spriteSwitcher.enabled = false;

        if (normalSprite != null)
        {
            characterImage.gameObject.SetActive(true);
            characterImage.sprite = normalSprite;
            characterImage.color = Color.white;
        }
        else
        {
            characterImage.gameObject.SetActive(false);
        }
    }

    private void UpdateNextButtonText()
    {
        if (nextButtonText == null)
            return;

        bool isLastLine = lines != null && lines.Length > 0 && index >= lines.Length - 1;

        if (LanguageManager.Instance == null)
        {
            nextButtonText.text = isLastLine ? "READY" : "NEXT";
            return;
        }

        string key = isLastLine ? "READY" : "NEXT";
        nextButtonText.text = LanguageManager.Instance.GetText(key);
    }

    private void UpdateSkipButtonText()
    {
        if (skipButtonText == null)
            return;

        if (LanguageManager.Instance == null)
        {
            skipButtonText.text = "SKIP DIALOGUE";
            return;
        }

        skipButtonText.text = LanguageManager.Instance.GetText("SKIP_DIALOGUE");
    }

    private void UpdateSkipButtonVisibility()
    {
        if (skipButton == null)
            return;

        if (!allowSkipping)
        {
            skipButton.gameObject.SetActive(false);
            return;
        }

        bool isLastLine = lines != null && lines.Length > 0 && index >= lines.Length - 1;

        skipButton.gameObject.SetActive(!(hideSkipButtonOnLastLine && isLastLine));
    }
}