using UnityEngine;
using UnityEngine.UI;

public class AudioToggleUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image buttonImage;

    [Header("Sprites")]
    [SerializeField] private Sprite audioOnSprite;
    [SerializeField] private Sprite audioOffSprite;

    private void OnEnable()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.OnAudioMuteChanged += HandleAudioMuteChanged;

        RefreshVisual();
    }

    private void OnDisable()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.OnAudioMuteChanged -= HandleAudioMuteChanged;
    }

    public void ToggleAudio()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.ToggleMute();

        RefreshVisual();
    }

    private void HandleAudioMuteChanged(bool isMuted)
    {
        RefreshVisual();
    }

    private void RefreshVisual()
    {
        if (buttonImage == null || AudioManager.Instance == null)
            return;

        buttonImage.sprite = AudioManager.Instance.IsMuted ? audioOffSprite : audioOnSprite;
        buttonImage.SetAllDirty();
    }
}