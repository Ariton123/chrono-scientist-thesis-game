using UnityEngine;

public class SceneMusicController : MonoBehaviour
{
    [Header("Scene Music")]
    [SerializeField] private AudioClip sceneMusicClip;

    [Header("Options")]
    [SerializeField] private bool playOnStart = true;
    [SerializeField] private bool stopMusicIfNoClip = false;

    private void Start()
    {
        if (!playOnStart)
            return;

        ApplySceneMusic();
    }

    [ContextMenu("Apply Scene Music Now")]
    public void ApplySceneMusic()
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("[SceneMusicController] AudioManager.Instance is null.");
            return;
        }

        if (sceneMusicClip != null)
        {
            AudioManager.Instance.PlayMusic(sceneMusicClip);
        }
        else if (stopMusicIfNoClip)
        {
            AudioManager.Instance.StopMusic();
        }
    }
}