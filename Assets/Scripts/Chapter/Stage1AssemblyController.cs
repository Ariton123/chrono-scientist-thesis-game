using UnityEngine;
using TMPro;

public class Stage1AssemblyController : MonoBehaviour
{
    public Stage1GameManager gameManager;
    public SceneLoader sceneLoader;

    [Header("Performance")]
    [SerializeField] private LevelPerformanceController performanceController;

    [Header("Retry Overlay")]
    [SerializeField] private RetryOverlayUI retryOverlayUI;

    [Header("Reset Targets")]
    [SerializeField] private UIBoneDrag[] draggablePieces;
    [SerializeField] private BoneSlotUI[] boneSlots;
    [SerializeField] private BoneInfoPanelUI boneInfoPanel;

    [Header("Next Scene")]
    public int nextSceneIndex = 3;

    public int totalPieces = 6;
    public int placedPieces = 0;

    [Header("Assembly Panel")]
    public GameObject completeButton;
    public TMP_Text assemblyStatusText;

    private enum AssemblyState
    {
        NotStarted,
        InProgress,
        Complete,
        Failed
    }

    private AssemblyState currentState = AssemblyState.NotStarted;

    private void OnEnable()
    {
        if (LanguageManager.Instance != null)
            LanguageManager.Instance.OnLanguageChanged += HandleLanguageChanged;
    }

    private void OnDisable()
    {
        if (LanguageManager.Instance != null)
            LanguageManager.Instance.OnLanguageChanged -= HandleLanguageChanged;
    }

    public void BeginAssembly()
    {
        placedPieces = 0;
        currentState = AssemblyState.InProgress;

        if (completeButton != null)
            completeButton.SetActive(false);

        if (retryOverlayUI != null)
            retryOverlayUI.Hide();

        if (performanceController != null)
            performanceController.BeginLevel();

        RefreshAssemblyText();
    }

    public void RestartAssembly()
    {
        Debug.Log("[Assembly] Restarting assembly in-place.");

        ResetDraggablePieces();
        ResetSlots();

        if (boneInfoPanel != null)
            boneInfoPanel.ClearInfo();

        BeginAssembly();
    }

    public void FailAssembly()
    {
        if (currentState == AssemblyState.Complete)
            return;

        currentState = AssemblyState.Failed;

        if (completeButton != null)
            completeButton.SetActive(false);

        RefreshAssemblyText();

        if (gameManager != null)
            gameManager.ShowFailPanel();

        if (retryOverlayUI != null)
            retryOverlayUI.Show();
        else
            Debug.LogWarning("[Assembly] RetryOverlayUI reference is missing.");
    }

    public void RegisterCorrectPlacement()
    {
        if (currentState != AssemblyState.InProgress)
            return;

        if (performanceController != null && performanceController.HasFailed)
            return;

        placedPieces++;
        RefreshAssemblyText();

        if (placedPieces >= totalPieces)
        {
            CompleteAssembly();
        }
    }

    public void RegisterMistake()
    {
        if (currentState != AssemblyState.InProgress)
            return;

        if (performanceController != null)
            performanceController.RegisterMistake();
    }

    public void CompleteAssembly()
    {
        if (currentState != AssemblyState.InProgress)
            return;

        if (performanceController != null && performanceController.HasFailed)
            return;

        currentState = AssemblyState.Complete;

        if (performanceController != null)
            performanceController.CompleteLevel();

        RefreshAssemblyText();

        if (completeButton != null)
            completeButton.SetActive(true);

        Debug.Log("Assembly complete!");
    }

    public void OnContinueAfterAssembly()
    {
        if (currentState != AssemblyState.Complete)
            return;

        if (gameManager != null)
            gameManager.CompleteStage();
    }

    public void OnContinueToLabHub()
    {
        if (sceneLoader != null)
            sceneLoader.LoadSceneByIndex(nextSceneIndex);
    }

    public void OnRetryClicked()
    {
        RestartAssembly();
    }

    public void OnMainMenuClicked()
    {
        if (sceneLoader != null)
            sceneLoader.LoadSceneByIndex(0);
    }

    private void ResetDraggablePieces()
    {
        if (draggablePieces == null)
            return;

        foreach (UIBoneDrag draggable in draggablePieces)
        {
            if (draggable != null)
                draggable.ForceResetToStart();
        }
    }

    private void ResetSlots()
    {
        if (boneSlots == null)
            return;

        foreach (BoneSlotUI slot in boneSlots)
        {
            if (slot != null)
                slot.ResetSlot();
        }
    }

    private void HandleLanguageChanged(GameLanguage language)
    {
        RefreshAssemblyText();
    }

    private void RefreshAssemblyText()
    {
        if (assemblyStatusText == null || LanguageManager.Instance == null)
            return;

        switch (currentState)
        {
            case AssemblyState.InProgress:
                if (placedPieces == 0)
                {
                    assemblyStatusText.text = LanguageManager.Instance.GetText("STAGE1_ASSEMBLY_001");
                }
                else if (placedPieces < totalPieces)
                {
                    assemblyStatusText.text = LanguageManager.Instance.GetText("STAGE1_ASSEMBLY_002")
                        .Replace("{current}", placedPieces.ToString())
                        .Replace("{total}", totalPieces.ToString());
                }
                else
                {
                    assemblyStatusText.text = LanguageManager.Instance.GetText("STAGE1_ASSEMBLY_003");
                }
                break;

            case AssemblyState.Complete:
                assemblyStatusText.text = LanguageManager.Instance.GetText("STAGE1_ASSEMBLY_003");
                break;

            case AssemblyState.Failed:
                assemblyStatusText.text = "";
                break;

            default:
                assemblyStatusText.text = "";
                break;
        }
    }
}