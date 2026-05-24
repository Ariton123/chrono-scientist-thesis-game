using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LocalizationDatabase", menuName = "ChronoScientist/Localization Database")]
public class LocalizationDatabase : ScriptableObject
{
    public List<LocalizationEntry> entries = new List<LocalizationEntry>();

    private Dictionary<string, LocalizationEntry> lookup;

    private void OnEnable()
    {
        BuildLookup();
    }

    private void OnValidate()
    {
        BuildLookup();
    }

    private void BuildLookup()
    {
        lookup = new Dictionary<string, LocalizationEntry>();

        foreach (var entry in entries)
        {
            if (entry == null || string.IsNullOrWhiteSpace(entry.key))
                continue;

            if (!lookup.ContainsKey(entry.key))
                lookup.Add(entry.key, entry);
            else
                Debug.LogWarning($"Duplicate localization key found: {entry.key}", this);
        }
    }

    public string GetText(string key, GameLanguage language)
    {
        if (string.IsNullOrWhiteSpace(key))
            return string.Empty;

        if (lookup == null)
            BuildLookup();

        if (!lookup.TryGetValue(key, out var entry))
        {
            Debug.LogWarning($"Missing localization key: {key}", this);
            return key;
        }

        switch (language)
        {
            case GameLanguage.English:
                return Fallback(entry.english, key);

            case GameLanguage.Deutsch:
                return Fallback(entry.deutsch, entry.english, key);

            case GameLanguage.Francais:
                return Fallback(entry.francais, entry.english, key);

            case GameLanguage.Italiano:
                return Fallback(entry.italiano, entry.english, key);

            default:
                return Fallback(entry.english, key);
        }
    }

    private string Fallback(string primary, string fallback)
    {
        return string.IsNullOrWhiteSpace(primary) ? fallback : primary;
    }

    private string Fallback(string primary, string fallback, string finalFallback)
    {
        if (!string.IsNullOrWhiteSpace(primary))
            return primary;

        if (!string.IsNullOrWhiteSpace(fallback))
            return fallback;

        return finalFallback;
    }
}