using UnityEngine;

public class PlayPanelController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject playPanel;

    [Header("Scene Loader")]
    [SerializeField] private SceneLoader sceneLoader;

    [Header("Pre-Level Dialogue Scene Indexes")]
    [SerializeField] private int level1PreDialogueSceneIndex = 1;
    [SerializeField] private int level5PreDialogueSceneIndex = 4;
    [SerializeField] private int level9PreDialogueSceneIndex = 7;

    private const string OpenPlayPanelPrefKey = "OpenPlayPanelOnLoad";

    private void Start()
    {
        if (PlayerPrefs.GetInt(OpenPlayPanelPrefKey, 0) == 1)
        {
            PlayerPrefs.SetInt(OpenPlayPanelPrefKey, 0);
            PlayerPrefs.Save();

            OpenPlayPanel();
        }
    }

    public void OpenPlayPanel()
    {
        if (playPanel != null)
            playPanel.SetActive(true);
    }

    public void ClosePlayPanel()
    {
        if (playPanel != null)
            playPanel.SetActive(false);
    }

    public void StartLevel1()
    {
        LoadPreDialogue(level1PreDialogueSceneIndex);
    }

    public void StartLevel5()
    {
        LoadPreDialogue(level5PreDialogueSceneIndex);
    }

    public void StartLevel9()
    {
        LoadPreDialogue(level9PreDialogueSceneIndex);
    }

    private void LoadPreDialogue(int sceneIndex)
    {
        if (sceneLoader == null)
        {
            Debug.LogWarning("[PlayPanelController] SceneLoader reference is missing.");
            return;
        }

        Debug.Log($"[PlayPanelController] Loading pre-level dialogue scene index: {sceneIndex}");
        sceneLoader.LoadSceneByIndex(sceneIndex);
    }
}