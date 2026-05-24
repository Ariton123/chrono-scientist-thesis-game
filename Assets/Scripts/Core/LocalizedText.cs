using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizedText : MonoBehaviour
{
    [Header("Localization")]
    [SerializeField] private string localizationKey;

    private TextMeshProUGUI textComponent;

    private void Awake()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        RefreshText();

        if (LanguageManager.Instance != null)
            LanguageManager.Instance.OnLanguageChanged += HandleLanguageChanged;
    }

    private void OnDestroy()
    {
        if (LanguageManager.Instance != null)
            LanguageManager.Instance.OnLanguageChanged -= HandleLanguageChanged;
    }

    private void HandleLanguageChanged(GameLanguage language)
    {
        RefreshText();
    }

    public void RefreshText()
    {
        if (textComponent == null)
            textComponent = GetComponent<TextMeshProUGUI>();

        if (LanguageManager.Instance == null)
        {
            Debug.LogWarning($"LocalizedText on '{gameObject.name}' could not refresh because LanguageManager.Instance is null.");
            return;
        }

        textComponent.text = LanguageManager.Instance.GetText(localizationKey);
    }

    public void SetKey(string newKey)
    {
        localizationKey = newKey;
        RefreshText();
    }
}