using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseOverlayUI : MonoBehaviour
{
    [Header("Overlay")]
    [Tooltip("Assign the full PauseOverlay object here. It can be the same object this script is attached to.")]
    [SerializeField] private GameObject pauseOverlay;

    [Tooltip("CanvasGroup on PauseOverlay. If missing, the script creates one automatically.")]
    [SerializeField] private CanvasGroup overlayCanvasGroup;

    [Header("Gameplay Phase Check")]
    [Tooltip("Assign the AssemblyPanel here. Pause will be blocked while this panel is active.")]
    [SerializeField] private GameObject assemblyPanel;

    [Tooltip("If true, ESC/P pause is disabled while the AssemblyPanel is active.")]
    [SerializeField] private bool blockPauseDuringAssembly = true;

    [Header("Texts")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text resumeButtonText;
    [SerializeField] private TMP_Text retryButtonText;
    [SerializeField] private TMP_Text mainMenuButtonText;

    [Header("Localization Keys")]
    [SerializeField] private string titleKey = "GAME_IS_PAUSED";
    [SerializeField] private string resumeButtonKey = "RESUME";
    [SerializeField] private string retryButtonKey = "RETRY";
    [SerializeField] private string mainMenuButtonKey = "MAIN_MENU";

    [Header("Scene Loading")]
    [SerializeField] private int mainMenuSceneIndex = 0;

    [Header("Input")]
    [SerializeField] private bool allowEscapeKey = true;

    private bool isPaused = false;
    private float previousTimeScale = 1f;

    public bool IsPaused => isPaused;

    private void Awake()
    {
        if (pauseOverlay == null)
            pauseOverlay = gameObject;

        if (overlayCanvasGroup == null && pauseOverlay != null)
            overlayCanvasGroup = pauseOverlay.GetComponent<CanvasGroup>();

        if (overlayCanvasGroup == null && pauseOverlay != null)
            overlayCanvasGroup = pauseOverlay.AddComponent<CanvasGroup>();

        // IMPORTANT:
        // Keep the object active so this script can still detect Escape/P.
        if (pauseOverlay != null)
            pauseOverlay.SetActive(true);

        HideInstant();
    }

    private void OnEnable()
    {
        if (LanguageManager.Instance != null)
            LanguageManager.Instance.OnLanguageChanged += HandleLanguageChanged;

        RefreshUI();
    }

    private void OnDisable()
    {
        if (LanguageManager.Instance != null)
            LanguageManager.Instance.OnLanguageChanged -= HandleLanguageChanged;

        // Safety: if scene/object disables while paused, never leave the game frozen.
        if (isPaused)
            Time.timeScale = 1f;
    }

    private void Update()
    {
        if (!allowEscapeKey)
            return;

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            TogglePauseFromKeyboard();
        }
    }

    private void TogglePauseFromKeyboard()
    {
        if (isPaused)
        {
            ResumeGame();
            return;
        }

        if (ShouldBlockPauseNow())
        {
            Debug.Log("[PauseOverlayUI] Pause ignored because AssemblyPanel is active.");
            return;
        }

        PauseGame();
    }

    private bool ShouldBlockPauseNow()
    {
        if (!blockPauseDuringAssembly)
            return false;

        if (assemblyPanel == null)
            return false;

        return assemblyPanel.activeInHierarchy;
    }

    public void PauseGame()
    {
        if (isPaused)
            return;

        if (ShouldBlockPauseNow())
        {
            Debug.Log("[PauseOverlayUI] Pause ignored because AssemblyPanel is active.");
            return;
        }

        isPaused = true;

        previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        ShowOverlay();
        RefreshUI();

        Debug.Log("[PauseOverlayUI] Game paused.");
    }

    public void ResumeGame()
    {
        if (!isPaused)
            return;

        isPaused = false;

        Time.timeScale = previousTimeScale <= 0f ? 1f : previousTimeScale;

        HideInstant();

        Debug.Log("[PauseOverlayUI] Game resumed.");
    }

    public void OnResumeClicked()
    {
        ResumeGame();
    }

    public void OnRetryClicked()
    {
        Time.timeScale = 1f;
        isPaused = false;

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    public void OnMainMenuClicked()
    {
        Time.timeScale = 1f;
        isPaused = false;

        SceneManager.LoadScene(mainMenuSceneIndex);
    }

    private void ShowOverlay()
    {
        if (pauseOverlay != null)
        {
            pauseOverlay.SetActive(true);
            pauseOverlay.transform.SetAsLastSibling();
        }

        if (overlayCanvasGroup != null)
        {
            overlayCanvasGroup.alpha = 1f;
            overlayCanvasGroup.interactable = true;
            overlayCanvasGroup.blocksRaycasts = true;
        }
    }

    private void HideInstant()
    {
        if (pauseOverlay != null)
            pauseOverlay.SetActive(true);

        if (overlayCanvasGroup != null)
        {
            overlayCanvasGroup.alpha = 0f;
            overlayCanvasGroup.interactable = false;
            overlayCanvasGroup.blocksRaycasts = false;
        }
    }

    private void HandleLanguageChanged(GameLanguage language)
    {
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (LanguageManager.Instance != null)
        {
            if (titleText != null)
                titleText.text = LanguageManager.Instance.GetText(titleKey);

            if (resumeButtonText != null)
                resumeButtonText.text = LanguageManager.Instance.GetText(resumeButtonKey);

            if (retryButtonText != null)
                retryButtonText.text = LanguageManager.Instance.GetText(retryButtonKey);

            if (mainMenuButtonText != null)
                mainMenuButtonText.text = LanguageManager.Instance.GetText(mainMenuButtonKey);
        }
        else
        {
            if (titleText != null)
                titleText.text = "Game is paused";

            if (resumeButtonText != null)
                resumeButtonText.text = "Resume";

            if (retryButtonText != null)
                retryButtonText.text = "Retry";

            if (mainMenuButtonText != null)
                mainMenuButtonText.text = "Main Menu";
        }
    }
}