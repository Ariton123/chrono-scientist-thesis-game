using UnityEngine;
using TMPro;

public class Stage1WorldIntroController : MonoBehaviour
{
    public GameObject continueButton;
    public TMP_Text hintText;
    public Stage1GameManager gameManager;

    [Header("Typewriter")]
    [SerializeField] private TypewriterTextUI hintTypewriter;

    private bool cluePlacedCorrectly = false;

    private void Awake()
    {
        if (hintTypewriter == null && hintText != null)
            hintTypewriter = hintText.GetComponent<TypewriterTextUI>();
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

        if (hintTypewriter != null)
            hintTypewriter.StopTyping();
    }

    void Start()
    {
        if (continueButton != null)
            continueButton.SetActive(false);

        if (hintText != null)
            hintText.gameObject.SetActive(true);

        cluePlacedCorrectly = false;
        RefreshHintText();
    }

    public void OnCluePlacedCorrectly()
    {
        cluePlacedCorrectly = true;
        RefreshHintText();

        if (continueButton != null)
            continueButton.SetActive(true);
    }

    public void OnContinueClicked()
    {
        if (hintTypewriter != null)
            hintTypewriter.StopTyping();

        if (continueButton != null)
            continueButton.SetActive(false);

        if (hintText != null)
            hintText.gameObject.SetActive(false);

        if (gameManager != null)
            gameManager.OpenMissionPanel();
    }

    private void HandleLanguageChanged(GameLanguage language)
    {
        RefreshHintText();
    }

    private void RefreshHintText()
    {
        if (hintText == null || LanguageManager.Instance == null)
            return;

        string key = cluePlacedCorrectly
            ? "STAGE1_INTRO_002"
            : "STAGE1_INTRO_001";

        string resolvedText = LanguageManager.Instance.GetText(key);

        if (hintTypewriter != null)
            hintTypewriter.Play(resolvedText);
        else
            hintText.text = resolvedText;
    }
}