using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private const string AudioMutedPrefKey = "AudioMuted";
    private const string SFXVolumePrefKey = "SFXVolume";
    private const string MusicVolumePrefKey = "MusicVolume";

    [Header("Default Audio Clips")]
    [SerializeField] private AudioClip defaultBackgroundMusicClip;
    [SerializeField] private AudioClip buttonClickClip;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Music Settings")]
    [SerializeField] private bool playDefaultMusicOnStart = true;
    [SerializeField] private bool loopMusic = true;

    public bool IsMuted { get; private set; }
    public float SFXVolume { get; private set; } = 1f;
    public float MusicVolume { get; private set; } = 1f;

    public event System.Action<bool> OnAudioMuteChanged;
    public event System.Action<float> OnSFXVolumeChanged;
    public event System.Action<float> OnMusicVolumeChanged;

    private AudioClip currentMusicClip;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        EnsureAudioSources();

        LoadAudioState();
        ApplyMuteState();
        ApplySourceVolumes();

        if (playDefaultMusicOnStart && defaultBackgroundMusicClip != null)
            PlayMusic(defaultBackgroundMusicClip);
    }

    private void EnsureAudioSources()
    {
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.playOnAwake = false;
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
        }

        musicSource.loop = loopMusic;
        sfxSource.loop = false;
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

        ApplySourceVolumes();
        OnSFXVolumeChanged?.Invoke(SFXVolume);
    }

    public void SetMusicVolume(float value)
    {
        MusicVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(MusicVolumePrefKey, MusicVolume);
        PlayerPrefs.Save();

        ApplySourceVolumes();
        OnMusicVolumeChanged?.Invoke(MusicVolume);
    }

    public void PlayButtonClick()
    {
        PlaySFX(buttonClickClip);
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null)
            return;

        sfxSource.PlayOneShot(clip, SFXVolume);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null || musicSource == null)
            return;

        // Do not restart the same music if it is already playing.
        if (currentMusicClip == clip && musicSource.isPlaying)
            return;

        currentMusicClip = clip;
        musicSource.clip = clip;
        musicSource.loop = loopMusic;
        musicSource.volume = MusicVolume;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource == null)
            return;

        musicSource.Stop();
        currentMusicClip = null;
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

    private void ApplySourceVolumes()
    {
        if (sfxSource != null)
            sfxSource.volume = SFXVolume;

        if (musicSource != null)
            musicSource.volume = MusicVolume;
    }
}