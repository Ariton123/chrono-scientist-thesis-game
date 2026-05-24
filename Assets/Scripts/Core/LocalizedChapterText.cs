using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class LocalizedChapterText : MonoBehaviour
{
    [SerializeField] private int chapterNumber = 1;

    private TMP_Text textComponent;

    private void Awake()
    {
        textComponent = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        if (LanguageManager.Instance != null)
            LanguageManager.Instance.OnLanguageChanged += HandleLanguageChanged;

        RefreshText();
    }

    private void OnDisable()
    {
        if (LanguageManager.Instance != null)
            LanguageManager.Instance.OnLanguageChanged -= HandleLanguageChanged;
    }

    private void HandleLanguageChanged(GameLanguage language)
    {
        RefreshText();
    }

    private void RefreshText()
    {
        if (textComponent == null)
            textComponent = GetComponent<TMP_Text>();

        string chapterWord = "CHAPTER";

        if (LanguageManager.Instance != null)
            chapterWord = LanguageManager.Instance.GetText("CHAPTER");

        textComponent.text = $"{chapterWord} {chapterNumber}";
    }
}