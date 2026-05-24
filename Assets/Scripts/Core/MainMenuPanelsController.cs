using UnityEngine;

public class MainMenuPanelsController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject instructionsPanel;
    [SerializeField] private GameObject settingsPanel;

    public void OpenInstructions()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (instructionsPanel != null)
            instructionsPanel.SetActive(true);
    }

    public void CloseInstructions()
    {
        if (instructionsPanel != null)
            instructionsPanel.SetActive(false);
    }

    public void OpenSettings()
    {
        if (instructionsPanel != null)
            instructionsPanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    public void CloseAllPanels()
    {
        if (instructionsPanel != null)
            instructionsPanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }
}