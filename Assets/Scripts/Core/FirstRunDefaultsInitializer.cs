using UnityEngine;

public static class FirstRunDefaultsInitializer
{
    private const string LanguagePrefKey = "SelectedLanguage";
    private const string AudioMutedPrefKey = "AudioMuted";
    private const string SFXVolumePrefKey = "SFXVolume";
    private const string MusicVolumePrefKey = "MusicVolume";
    private const string GenderPrefKey = "PlayerGender";

    private const float DefaultVolume = 0.5f;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeMissingDefaults()
    {
        bool changed = false;

        if (!PlayerPrefs.HasKey(LanguagePrefKey))
        {
            PlayerPrefs.SetInt(LanguagePrefKey, (int)GameLanguage.English);
            changed = true;
        }

        if (!PlayerPrefs.HasKey(AudioMutedPrefKey))
        {
            PlayerPrefs.SetInt(AudioMutedPrefKey, 0);
            changed = true;
        }

        if (!PlayerPrefs.HasKey(SFXVolumePrefKey))
        {
            PlayerPrefs.SetFloat(SFXVolumePrefKey, DefaultVolume);
            changed = true;
        }

        if (!PlayerPrefs.HasKey(MusicVolumePrefKey))
        {
            PlayerPrefs.SetFloat(MusicVolumePrefKey, DefaultVolume);
            changed = true;
        }

        if (!PlayerPrefs.HasKey(GenderPrefKey))
        {
            PlayerPrefs.SetInt(GenderPrefKey, (int)PlayerGender.Male);
            changed = true;
        }

        if (changed)
        {
            PlayerPrefs.Save();
            Debug.Log("[FirstRunDefaults] Missing first-run defaults initialized.");
        }
    }
}