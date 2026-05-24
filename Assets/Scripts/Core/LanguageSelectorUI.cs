using UnityEngine;
using UnityEngine.UI;

public class LanguageSelectorUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject languagePanel;
    [SerializeField] private Image languagePanelImage;

    [Header("Main Button Image")]
    [SerializeField] private Image currentLanguageButtonImage;

    [Header("Main Button Sprites")]
    [SerializeField] private Sprite englishButtonSprite;
    [SerializeField] private Sprite deutschButtonSprite;
    [SerializeField] private Sprite francaisButtonSprite;
    [SerializeField] private Sprite italianoButtonSprite;

    [Header("Panel Sprites")]
    [SerializeField] private Sprite englishPanelSprite;
    [SerializeField] private Sprite deutschPanelSprite;
    [SerializeField] private Sprite francaisPanelSprite;
    [SerializeField] private Sprite italianoPanelSprite;

    private void OnEnable()
    {
        if (LanguageManager.Instance != null)
            LanguageManager.Instance.OnLanguageChanged += HandleLanguageChanged;

        RefreshVisuals();
    }

    private void OnDisable()
    {
        if (LanguageManager.Instance != null)
            LanguageManager.Instance.OnLanguageChanged -= HandleLanguageChanged;
    }

    private void HandleLanguageChanged(GameLanguage language)
    {
        RefreshVisuals();
    }

    public void ToggleLanguagePanel()
    {
        if (languagePanel != null)
            languagePanel.SetActive(!languagePanel.activeSelf);
    }

    public void SetEnglish()
    {
        if (LanguageManager.Instance != null)
            LanguageManager.Instance.SetLanguage(GameLanguage.English);

        RefreshVisuals();
        ClosePanel();
    }

    public void SetDeutsch()
    {
        if (LanguageManager.Instance != null)
            LanguageManager.Instance.SetLanguage(GameLanguage.Deutsch);

        RefreshVisuals();
        ClosePanel();
    }

    public void SetFrancais()
    {
        if (LanguageManager.Instance != null)
            LanguageManager.Instance.SetLanguage(GameLanguage.Francais);

        RefreshVisuals();
        ClosePanel();
    }

    public void SetItaliano()
    {
        if (LanguageManager.Instance != null)
            LanguageManager.Instance.SetLanguage(GameLanguage.Italiano);

        RefreshVisuals();
        ClosePanel();
    }

    private void ClosePanel()
    {
        if (languagePanel != null)
            languagePanel.SetActive(false);
    }

    private void RefreshVisuals()
    {
        if (LanguageManager.Instance == null)
        {
            Debug.LogWarning("LanguageSelectorUI: LanguageManager.Instance is null.");
            return;
        }

        Debug.Log($"[LanguageSelectorUI] Refreshing visuals for language: {LanguageManager.Instance.CurrentLanguage}");

        switch (LanguageManager.Instance.CurrentLanguage)
        {
            case GameLanguage.English:
                if (currentLanguageButtonImage != null)
                    currentLanguageButtonImage.sprite = englishButtonSprite;

                if (languagePanelImage != null)
                    languagePanelImage.sprite = englishPanelSprite;
                break;

            case GameLanguage.Deutsch:
                if (currentLanguageButtonImage != null)
                    currentLanguageButtonImage.sprite = deutschButtonSprite;

                if (languagePanelImage != null)
                    languagePanelImage.sprite = deutschPanelSprite;
                break;

            case GameLanguage.Francais:
                if (currentLanguageButtonImage != null)
                    currentLanguageButtonImage.sprite = francaisButtonSprite;

                if (languagePanelImage != null)
                    languagePanelImage.sprite = francaisPanelSprite;
                break;

            case GameLanguage.Italiano:
                if (currentLanguageButtonImage != null)
                    currentLanguageButtonImage.sprite = italianoButtonSprite;

                if (languagePanelImage != null)
                    languagePanelImage.sprite = italianoPanelSprite;
                break;
        }

        if (currentLanguageButtonImage != null)
            currentLanguageButtonImage.SetAllDirty();

        if (languagePanelImage != null)
            languagePanelImage.SetAllDirty();
    }
}