using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private const string AudioMutedPrefKey = "AudioMuted";
    private const string SFXVolumePrefKey = "SFXVolume";
    private const string MusicVolumePrefKey = "MusicVolume";

    public bool IsMuted { get; private set; }
    public float SFXVolume { get; private set; } = 1f;
    public float MusicVolume { get; private set; } = 1f;

    public event System.Action<bool> OnAudioMuteChanged;
    public event System.Action<float> OnSFXVolumeChanged;
    public event System.Action<float> OnMusicVolumeChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadAudioState();
        ApplyMuteState();
    }

    public void ToggleMute()
    {
        SetMuted(!IsMuted);
    }

    public void SetMuted(bool muted)
    {
        IsMuted = muted;
        PlayerPrefs.SetInt(AudioMutedPrefKey, IsMuted ? 1 : 0);
        PlayerPrefs.Save();

        ApplyMuteState();
        OnAudioMuteChanged?.Invoke(IsMuted);
    }

    public void SetSFXVolume(float value)
    {
        SFXVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(SFXVolumePrefKey, SFXVolume);
        PlayerPrefs.Save();

        OnSFXVolumeChanged?.Invoke(SFXVolume);
    }

    public void SetMusicVolume(float value)
    {
        MusicVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(MusicVolumePrefKey, MusicVolume);
        PlayerPrefs.Save();

        OnMusicVolumeChanged?.Invoke(MusicVolume);
    }

    private void LoadAudioState()
    {
        IsMuted = PlayerPrefs.GetInt(AudioMutedPrefKey, 0) == 1;
        SFXVolume = PlayerPrefs.GetFloat(SFXVolumePrefKey, 1f);
        MusicVolume = PlayerPrefs.GetFloat(MusicVolumePrefKey, 1f);
    }

    private void ApplyMuteState()
    {
        AudioListener.volume = IsMuted ? 0f : 1f;
    }
}