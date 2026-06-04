using System.Collections;
using UnityEngine;
using TMPro;

public class Stage1WorldIntroController : MonoBehaviour
{
    [Header("Core References")]
    public GameObject continueButton;
    public TMP_Text hintText;
    public Stage1GameManager gameManager;

    [Header("Continue Button Safety")]
    [SerializeField] private CanvasGroup continueButtonCanvasGroup;

    [Header("Typewriter")]
    [SerializeField] private TypewriterTextUI hintTypewriter;

    [Header("UI Fade Before Mission Panel")]
    [Tooltip("Use this for UI objects only: hint scroll, hint text, continue button. Do NOT add the background.")]
    [SerializeField] private CanvasGroup[] fadeOutCanvasGroups;

    [Header("Character Fade Before Mission Panel")]
    [Tooltip("Use this for SpriteRenderer characters, for example DarwinGroup and Apprentice sprite renderers.")]
    [SerializeField] private SpriteRenderer[] fadeOutSpriteRenderers;

    [Header("Disable After Fade")]
    [Tooltip("Objects disabled after fade finishes. Add hint scroll, hint text, button, and character parent objects if needed.")]
    [SerializeField] private GameObject[] disableAfterFade;

    [Header("Timing")]
    [SerializeField] private float fadeOutDuration = 0.65f;

    [Tooltip("Pause after everything fades out, before the mission panel opens.")]
    [SerializeField] private float delayBeforeMissionPanel = 0.9f;

    [SerializeField] private bool useUnscaledTime = true;

    private bool cluePlacedCorrectly = false;
    private bool isTransitioning = false;

    private void Awake()
    {
        if (hintTypewriter == null && hintText != null)
            hintTypewriter = hintText.GetComponent<TypewriterTextUI>();

        if (continueButtonCanvasGroup == null && continueButton != null)
            continueButtonCanvasGroup = continueButton.GetComponent<CanvasGroup>();

        if (continueButtonCanvasGroup == null && continueButton != null)
            continueButtonCanvasGroup = continueButton.AddComponent<CanvasGroup>();

        SetContinueButtonVisible(false);
    }

    private void OnEnable()
    {
        if (LanguageManager.Instance != null)
            LanguageManager.Instance.OnLanguageChanged += HandleLanguageChanged;

        if (!cluePlacedCorrectly && !isTransitioning)
            SetContinueButtonVisible(false);
    }

    private void OnDisable()
    {
        if (LanguageManager.Instance != null)
            LanguageManager.Instance.OnLanguageChanged -= HandleLanguageChanged;

        if (hintTypewriter != null)
            hintTypewriter.StopTyping();
    }

    private void Start()
    {
        cluePlacedCorrectly = false;
        isTransitioning = false;

        SetContinueButtonVisible(false);

        if (hintText != null)
            hintText.gameObject.SetActive(true);

        ResetFadeObjects();

        SetContinueButtonVisible(false);

        RefreshHintText();
    }

    public void OnCluePlacedCorrectly()
    {
        if (isTransitioning)
            return;

        cluePlacedCorrectly = true;
        RefreshHintText();

        SetContinueButtonVisible(true);
    }

    public void OnContinueClicked()
    {
        if (isTransitioning)
            return;

        if (!cluePlacedCorrectly)
        {
            Debug.Log("[Stage1WorldIntro] Continue clicked before clue was placed. Ignored.");
            SetContinueButtonVisible(false);
            return;
        }

        SetContinueButtonVisible(false);
        StartCoroutine(FadeOutThenOpenMissionPanel());
    }

    private IEnumerator FadeOutThenOpenMissionPanel()
    {
        isTransitioning = true;

        if (hintTypewriter != null)
            hintTypewriter.StopTyping();

        float elapsed = 0f;

        while (elapsed < fadeOutDuration)
        {
            elapsed += DeltaTime();

            float t = Mathf.Clamp01(elapsed / fadeOutDuration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            float alpha = Mathf.Lerp(1f, 0f, smoothT);

            SetCanvasGroupsAlpha(alpha);
            SetSpriteRenderersAlpha(alpha);

            yield return null;
        }

        SetCanvasGroupsAlpha(0f);
        SetSpriteRenderersAlpha(0f);

        DisableFadeObjects();

        if (delayBeforeMissionPanel > 0f)
        {
            if (useUnscaledTime)
                yield return new WaitForSecondsRealtime(delayBeforeMissionPanel);
            else
                yield return new WaitForSeconds(delayBeforeMissionPanel);
        }

        if (gameManager != null)
            gameManager.OpenMissionPanel();

        isTransitioning = false;
    }

    private void SetContinueButtonVisible(bool visible)
    {
        if (continueButton != null)
            continueButton.SetActive(visible);

        if (continueButtonCanvasGroup != null)
        {
            continueButtonCanvasGroup.alpha = visible ? 1f : 0f;
            continueButtonCanvasGroup.interactable = visible;
            continueButtonCanvasGroup.blocksRaycasts = visible;
        }
    }

    private void ResetFadeObjects()
    {
        if (disableAfterFade != null)
        {
            foreach (GameObject obj in disableAfterFade)
            {
                if (obj != null)
                    obj.SetActive(true);
            }
        }

        if (fadeOutCanvasGroups != null)
        {
            foreach (CanvasGroup group in fadeOutCanvasGroups)
            {
                if (group == null)
                    continue;

                group.alpha = 1f;
                group.interactable = true;
                group.blocksRaycasts = true;
            }
        }

        if (fadeOutSpriteRenderers != null)
        {
            foreach (SpriteRenderer spriteRenderer in fadeOutSpriteRenderers)
            {
                if (spriteRenderer == null)
                    continue;

                Color color = spriteRenderer.color;
                color.a = 1f;
                spriteRenderer.color = color;
            }
        }

        if (!cluePlacedCorrectly)
            SetContinueButtonVisible(false);
    }

    private void SetCanvasGroupsAlpha(float alpha)
    {
        if (fadeOutCanvasGroups == null)
            return;

        foreach (CanvasGroup group in fadeOutCanvasGroups)
        {
            if (group == null)
                continue;

            group.alpha = alpha;
        }

        if (!cluePlacedCorrectly && continueButtonCanvasGroup != null)
        {
            continueButtonCanvasGroup.alpha = 0f;
            continueButtonCanvasGroup.interactable = false;
            continueButtonCanvasGroup.blocksRaycasts = false;
        }
    }

    private void SetSpriteRenderersAlpha(float alpha)
    {
        if (fadeOutSpriteRenderers == null)
            return;

        foreach (SpriteRenderer spriteRenderer in fadeOutSpriteRenderers)
        {
            if (spriteRenderer == null)
                continue;

            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
        }
    }

    private void DisableFadeObjects()
    {
        if (fadeOutCanvasGroups != null)
        {
            foreach (CanvasGroup group in fadeOutCanvasGroups)
            {
                if (group == null)
                    continue;

                group.interactable = false;
                group.blocksRaycasts = false;
            }
        }

        if (disableAfterFade != null)
        {
            foreach (GameObject obj in disableAfterFade)
            {
                if (obj != null)
                    obj.SetActive(false);
            }
        }
    }

    private float DeltaTime()
    {
        return useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
    }

    private void HandleLanguageChanged(GameLanguage language)
    {
        if (!isTransitioning)
            RefreshHintText();

        if (!cluePlacedCorrectly)
            SetContinueButtonVisible(false);
    }

    private void RefreshHintText()
    {
        if (hintText == null || LanguageManager.Instance == null)
            return;

        string key = cluePlacedCorrectly
            ? "STAGE1_INTRO_002"
            : "STAGE1_INTRO_001";

        string resolvedText = LanguageManager.Instance.GetText(key);

        if (hintTypewriter != null)
            hintTypewriter.Play(resolvedText);
        else
            hintText.text = resolvedText;
    }
}