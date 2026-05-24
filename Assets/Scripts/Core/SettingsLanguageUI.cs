using TMPro;
using UnityEngine;

public class SettingsLanguageUI : MonoBehaviour
{
    [SerializeField] private TMP_Text currentLanguageText;

    private void OnEnable()
    {
        if (LanguageManager.Instance != null)
            LanguageManager.Instance.OnLanguageChanged += HandleLanguageChanged;

        RefreshUI();
    }

    private void OnDisable()
    {
        if (LanguageManager.Instance != null)
            LanguageManager.Instance.OnLanguageChanged -= HandleLanguageChanged;
    }

    public void NextLanguage()
    {
        if (LanguageManager.Instance == null) return;

        int next = ((int)LanguageManager.Instance.CurrentLanguage + 1) %
                   System.Enum.GetValues(typeof(GameLanguage)).Length;

        LanguageManager.Instance.SetLanguage((GameLanguage)next);
    }

    public void PreviousLanguage()
    {
        if (LanguageManager.Instance == null) return;

        int count = System.Enum.GetValues(typeof(GameLanguage)).Length;
        int previous = ((int)LanguageManager.Instance.CurrentLanguage - 1 + count) % count;

        LanguageManager.Instance.SetLanguage((GameLanguage)previous);
    }

    private void HandleLanguageChanged(GameLanguage language)
    {
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (currentLanguageText == null || LanguageManager.Instance == null)
            return;

        currentLanguageText.text = GetLanguageName(LanguageManager.Instance.CurrentLanguage);
    }

    private string GetLanguageName(GameLanguage language)
    {
        switch (language)
        {
            case GameLanguage.English: return "ENGLISH";
            case GameLanguage.Deutsch: return "DEUTSCH";
            case GameLanguage.Francais: return "FRANÇAIS";
            case GameLanguage.Italiano: return "ITALIANO";
            default: return "ENGLISH";
        }
    }
}