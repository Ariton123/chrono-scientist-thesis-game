using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsAudioUI : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider musicSlider;

    [Header("Value Texts")]
    [SerializeField] private TMP_Text sfxValueText;
    [SerializeField] private TMP_Text musicValueText;

    private bool listenersAdded = false;

    private void Awake()
    {
        AddListeners();
    }

    private void OnEnable()
    {
        RefreshUIFromAudioManager();
    }

    private void Start()
    {
        RefreshUIFromAudioManager();
    }

    private void OnDestroy()
    {
        RemoveListeners();
    }

    private void AddListeners()
    {
        if (listenersAdded)
            return;

        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(OnSFXSliderChanged);

        if (musicSlider != null)
            musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);

        listenersAdded = true;
    }

    private void RemoveListeners()
    {
        if (!listenersAdded)
            return;

        if (sfxSlider != null)
            sfxSlider.onValueChanged.RemoveListener(OnSFXSliderChanged);

        if (musicSlider != null)
            musicSlider.onValueChanged.RemoveListener(OnMusicSliderChanged);

        listenersAdded = false;
    }

    public void RefreshUIFromAudioManager()
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("[SettingsAudioUI] AudioManager.Instance is null.");
            return;
        }

        if (sfxSlider != null)
            sfxSlider.SetValueWithoutNotify(AudioManager.Instance.SFXVolume);

        if (musicSlider != null)
            musicSlider.SetValueWithoutNotify(AudioManager.Instance.MusicVolume);

        RefreshSFXValue(AudioManager.Instance.SFXVolume);
        RefreshMusicValue(AudioManager.Instance.MusicVolume);

        Debug.Log($"[SettingsAudioUI] Refreshed values. SFX: {AudioManager.Instance.SFXVolume}, Music: {AudioManager.Instance.MusicVolume}");
    }

    private void OnSFXSliderChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetSFXVolume(value);

        RefreshSFXValue(value);
    }

    private void OnMusicSliderChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMusicVolume(value);

        RefreshMusicValue(value);
    }

    private void RefreshSFXValue(float value)
    {
        if (sfxValueText != null)
            sfxValueText.text = Mathf.RoundToInt(value * 100f).ToString();
    }

    private void RefreshMusicValue(float value)
    {
        if (musicValueText != null)
            musicValueText.text = Mathf.RoundToInt(value * 100f).ToString();
    }
}