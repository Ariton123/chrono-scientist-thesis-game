using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class LocalizedTypewriterText : MonoBehaviour
{
    [Header("Localization")]
    [SerializeField] private string localizationKey;

    [Header("Typewriter")]
    [SerializeField] private TypewriterTextUI typewriterText;

    [Header("Fallback")]
    [TextArea(2, 6)]
    [SerializeField] private string fallbackText = "";

    [Header("Behaviour")]
    [SerializeField] private bool playOnEnable = true;
    [SerializeField] private bool replayOnLanguageChanged = true;

    private TMP_Text targetText;

    private void Awake()
    {
        targetText = GetComponent<TMP_Text>();

        if (typewriterText == null)
            typewriterText = GetComponent<TypewriterTextUI>();

        if (typewriterText == null)
            typewriterText = gameObject.AddComponent<TypewriterTextUI>();
    }

    private void OnEnable()
    {
        if (LanguageManager.Instance != null)
            LanguageManager.Instance.OnLanguageChanged += HandleLanguageChanged;

        if (playOnEnable)
            RefreshText();
    }

    private void OnDisable()
    {
        if (LanguageManager.Instance != null)
            LanguageManager.Instance.OnLanguageChanged -= HandleLanguageChanged;

        if (typewriterText != null)
            typewriterText.StopTyping();
    }

    private void HandleLanguageChanged(GameLanguage language)
    {
        if (replayOnLanguageChanged)
            RefreshText();
    }

    public void RefreshText()
    {
        string resolvedText = ResolveText();

        if (typewriterText != null)
            typewriterText.Play(resolvedText);
        else if (targetText != null)
            targetText.text = resolvedText;
    }

    public void SetKeyAndPlay(string newLocalizationKey)
    {
        localizationKey = newLocalizationKey;
        RefreshText();
    }

    private string ResolveText()
    {
        if (!string.IsNullOrEmpty(localizationKey) && LanguageManager.Instance != null)
            return LanguageManager.Instance.GetText(localizationKey);

        if (!string.IsNullOrEmpty(fallbackText))
            return fallbackText;

        return localizationKey;
    }
}