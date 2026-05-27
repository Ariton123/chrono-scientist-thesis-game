using UnityEngine;
using System.Collections;

public class Stage1GameManager : MonoBehaviour
{
    public GameObject environment;
    public GameObject characters;
    public GameObject missionPanel;
    public GameObject assemblyPanel;
    public GameObject completionPanel;
    public GameObject failPanel;

    public Stage1AssemblyController assemblyController;

    void Start()
    {
        if (environment != null)
            environment.SetActive(true);

        if (characters != null)
            characters.SetActive(true);

        HidePanelInstant(missionPanel);
        HidePanelInstant(assemblyPanel);
        HidePanelInstant(completionPanel);

        if (failPanel != null)
            failPanel.SetActive(false);
    }

    public void OpenMissionPanel()
    {
        HidePanelInstant(assemblyPanel);
        HidePanelInstant(completionPanel);

        if (failPanel != null)
            failPanel.SetActive(false);

        ShowPanelAnimated(missionPanel);
    }

    public void StartAssemblyPhase()
    {
        missionPanel.SetActive(false);
        assemblyPanel.SetActive(true);

        if (completionPanel != null)
            completionPanel.SetActive(false);

        if (failPanel != null)
            failPanel.SetActive(false);

        if (assemblyController != null)
            assemblyController.BeginAssembly();
    }

    public void ShowCompletionPanel()
    {
        HidePanelInstant(assemblyPanel);

        if (failPanel != null)
            failPanel.SetActive(false);

        ShowPanelAnimated(completionPanel);
    }

    public void ShowFailPanel()
    {
        // Keep assembly panel active because RetryOverlay lives inside it.
        if (assemblyPanel != null)
            assemblyPanel.SetActive(true);

        HidePanelInstant(completionPanel);

        if (failPanel != null)
        {
            failPanel.SetActive(true);
            failPanel.transform.SetAsLastSibling();
        }
        else
        {
            Debug.LogWarning("[StageGameManager] FailPanel is missing.");
        }
    }

    public void CompleteStage()
    {
        Debug.Log("Stage Complete!");
        ShowCompletionPanel();
    }

    private void ShowPanelAnimated(GameObject panel)
    {
        if (panel == null)
            return;

        UIPanelSlideAnimator animator = panel.GetComponent<UIPanelSlideAnimator>();

        if (animator != null)
            animator.PlayIn();
        else
            panel.SetActive(true);
    }

    private void HidePanelInstant(GameObject panel)
    {
        if (panel == null)
            return;

        UIPanelSlideAnimator animator = panel.GetComponent<UIPanelSlideAnimator>();

        if (animator != null)
            animator.HideInstant();
        else
            panel.SetActive(false);
    }
}