using System;
using UnityEngine;

public enum GameLanguage
{
    English,
    Deutsch,
    Francais,
    Italiano
}

public class LanguageManager : MonoBehaviour
{
    public static LanguageManager Instance;

    [Header("Localization Database")]
    [SerializeField] private LocalizationDatabase localizationDatabase;

    public GameLanguage CurrentLanguage { get; private set; } = GameLanguage.English;

    public event Action<GameLanguage> OnLanguageChanged;

    private const string LanguagePrefKey = "SelectedLanguage";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadLanguage();
    }

    private void Start()
    {
        NotifyLanguageChanged();
    }

    public void SetLanguage(GameLanguage newLanguage)
    {
        if (CurrentLanguage == newLanguage)
            return;

        CurrentLanguage = newLanguage;
        PlayerPrefs.SetInt(LanguagePrefKey, (int)CurrentLanguage);
        PlayerPrefs.Save();

        NotifyLanguageChanged();
    }

    public string GetText(string key)
    {
        if (localizationDatabase == null)
        {
            Debug.LogWarning("LocalizationDatabase is not assigned on LanguageManager.");
            return key;
        }

        string value = localizationDatabase.GetText(key, CurrentLanguage);
        Debug.Log($"[Localization] Language={CurrentLanguage} | Key='{key}' | Value='{value}'");
        return value;
    }

    private void LoadLanguage()
    {
        if (PlayerPrefs.HasKey(LanguagePrefKey))
            CurrentLanguage = (GameLanguage)PlayerPrefs.GetInt(LanguagePrefKey);
        else
            CurrentLanguage = GameLanguage.English;
    }

    private void NotifyLanguageChanged()
    {
        OnLanguageChanged?.Invoke(CurrentLanguage);
    }
}