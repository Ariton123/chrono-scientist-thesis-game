using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class LocalizedLevelText : MonoBehaviour
{
    [SerializeField] private int levelNumber = 1;

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

        string levelWord = "LEVEL";

        if (LanguageManager.Instance != null)
            levelWord = LanguageManager.Instance.GetText("LEVEL");

        textComponent.text = $"{levelWord} {levelNumber}";
    }
}