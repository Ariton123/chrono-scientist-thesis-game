using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TimerWarningMusowlUI : MonoBehaviour
{
    [Header("Performance Source")]
    [SerializeField] private LevelPerformanceController performanceController;

    [Header("Warning Timing")]
    [SerializeField] private float warningTimeThreshold = 20f;
    [SerializeField] private float visibleDuration = 5f;

    [Header("Animation")]
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float fadeOutDuration = 0.5f;
    [SerializeField] private float popScale = 1.05f;

    [Header("UI")]
    [SerializeField] private GameObject warningRoot;
    [SerializeField] private CanvasGroup warningCanvasGroup;
    [SerializeField] private RectTransform warningRectTransform;
    [SerializeField] private Image musowlImage;
    [SerializeField] private TMP_Text warningText;

    [Header("Localization")]
    [SerializeField] private string warningTextKey = "WATCH_THE_TIME";

    private bool warningShownThisRun = false;
    private bool wasRunningLastFrame = false;
    private Vector3 startScale = Vector3.one;
    private Coroutine warningCoroutine;

    private void Awake()
    {
        if (warningRoot == null)
            warningRoot = gameObject;

        if (warningCanvasGroup == null && warningRoot != null)
            warningCanvasGroup = warningRoot.GetComponent<CanvasGroup>();

        if (warningCanvasGroup == null && warningRoot != null)
            warningCanvasGroup = warningRoot.AddComponent<CanvasGroup>();

        if (warningRectTransform == null && warningRoot != null)
            warningRectTransform = warningRoot.GetComponent<RectTransform>();

        if (warningRectTransform != null)
            startScale = warningRectTransform.localScale;

        // IMPORTANT:
        // Keep the root active so Update() can keep checking the timer.
        if (warningRoot != null)
            warningRoot.SetActive(true);

        HideInstant();
    }

    private void OnEnable()
    {
        if (LanguageManager.Instance != null)
            LanguageManager.Instance.OnLanguageChanged += HandleLanguageChanged;

        RefreshText();
    }

    private void OnDisable()
    {
        if (LanguageManager.Instance != null)
            LanguageManager.Instance.OnLanguageChanged -= HandleLanguageChanged;
    }

    private void Update()
    {
        if (performanceController == null)
            return;

        bool isRunningNow =
            performanceController.IsRunning &&
            !performanceController.IsCompleted &&
            !performanceController.HasFailed;

        // Detect a new run, including retry.
        if (isRunningNow && !wasRunningLastFrame)
        {
            warningShownThisRun = false;
            HideInstant();
        }

        wasRunningLastFrame = isRunningNow;

        if (!isRunningNow)
            return;

        if (warningShownThisRun)
            return;

        if (performanceController.RemainingTime <= warningTimeThreshold)
            ShowWarning();
    }

    public void ShowWarning()
    {
        if (warningShownThisRun)
            return;

        warningShownThisRun = true;

        if (warningCoroutine != null)
            StopCoroutine(warningCoroutine);

        warningCoroutine = StartCoroutine(ShowWarningRoutine());
    }

    private IEnumerator ShowWarningRoutine()
    {
        RefreshText();

        if (warningRoot != null)
        {
            warningRoot.SetActive(true);
            warningRoot.transform.SetAsLastSibling();
        }

        if (warningCanvasGroup != null)
        {
            warningCanvasGroup.interactable = false;
            warningCanvasGroup.blocksRaycasts = false;
        }

        if (warningRectTransform != null)
            warningRectTransform.localScale = startScale * popScale;

        yield return FadeTo(1f, fadeInDuration);

        if (warningRectTransform != null)
            warningRectTransform.localScale = startScale;

        if (visibleDuration > 0f)
            yield return new WaitForSeconds(visibleDuration);

        yield return FadeTo(0f, fadeOutDuration);

        HideInstant();

        warningCoroutine = null;
    }

    private IEnumerator FadeTo(float targetAlpha, float duration)
    {
        if (warningCanvasGroup == null)
            yield break;

        float startAlpha = warningCanvasGroup.alpha;

        if (duration <= 0f)
        {
            warningCanvasGroup.alpha = targetAlpha;
            yield break;
        }

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / duration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            warningCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, smoothT);

            yield return null;
        }

        warningCanvasGroup.alpha = targetAlpha;
    }

    private void HideInstant()
    {
        // Do NOT disable warningRoot here.
        // The script is attached to this object, so disabling it would stop Update().
        if (warningCanvasGroup != null)
        {
            warningCanvasGroup.alpha = 0f;
            warningCanvasGroup.interactable = false;
            warningCanvasGroup.blocksRaycasts = false;
        }

        if (warningRectTransform != null)
            warningRectTransform.localScale = startScale == Vector3.zero ? Vector3.one : startScale;
    }

    private void HandleLanguageChanged(GameLanguage language)
    {
        RefreshText();
    }

    private void RefreshText()
    {
        if (warningText == null)
            return;

        if (LanguageManager.Instance != null)
            warningText.text = LanguageManager.Instance.GetText(warningTextKey);
        else
            warningText.text = "Watch the time!";
    }
}