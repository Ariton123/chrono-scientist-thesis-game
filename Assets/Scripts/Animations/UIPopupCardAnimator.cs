using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupCardAnimator : MonoBehaviour
{
    [Header("Main Card")]
    [SerializeField] private RectTransform cardRect;

    [Header("Optional Effects")]
    [SerializeField] private CanvasGroup cardCanvasGroup;
    [SerializeField] private Image glowImage;
    [SerializeField] private RectTransform astragalosBadgeRect;

    [Header("Reveal Timing")]
    [SerializeField] private float startScale = 0.15f;
    [SerializeField] private float overshootScale = 1.08f;
    [SerializeField] private float finalScale = 1f;

    [SerializeField] private float growDuration = 0.22f;
    [SerializeField] private float settleDuration = 0.12f;

    [Header("Glow")]
    [SerializeField] private float glowStartAlpha = 0f;
    [SerializeField] private float glowPeakAlpha = 0.55f;
    [SerializeField] private float glowFinalAlpha = 0.25f;

    [Header("Astragalos Badge Pop")]
    [SerializeField] private bool animateAstragalosBadge = true;
    [SerializeField] private float badgeDelay = 0.22f;
    [SerializeField] private float badgePopScale = 1.25f;
    [SerializeField] private float badgePopDuration = 0.16f;

    [Header("Timing")]
    [SerializeField] private bool useUnscaledTime = true;

    private Vector3 originalScale;
    private Vector3 badgeOriginalScale;
    private Coroutine revealCoroutine;

    private void Awake()
    {
        if (cardRect == null)
            cardRect = GetComponent<RectTransform>();

        if (cardCanvasGroup == null)
            cardCanvasGroup = GetComponent<CanvasGroup>();

        if (cardCanvasGroup == null)
            cardCanvasGroup = gameObject.AddComponent<CanvasGroup>();

        if (cardRect != null)
            originalScale = cardRect.localScale;

        if (astragalosBadgeRect != null)
            badgeOriginalScale = astragalosBadgeRect.localScale;
    }

    public void PlayReveal()
    {
        if (revealCoroutine != null)
            StopCoroutine(revealCoroutine);

        revealCoroutine = StartCoroutine(RevealRoutine());
    }

    private IEnumerator RevealRoutine()
    {
        if (cardRect == null)
            yield break;

        cardRect.localScale = originalScale * startScale;

        if (cardCanvasGroup != null)
            cardCanvasGroup.alpha = 0f;

        SetGlowAlpha(glowStartAlpha);

        if (astragalosBadgeRect != null && animateAstragalosBadge)
            astragalosBadgeRect.localScale = Vector3.zero;

        float elapsed = 0f;

        while (elapsed < growDuration)
        {
            elapsed += DeltaTime();
            float t = Mathf.Clamp01(elapsed / growDuration);
            float eased = EaseOutBack(t);

            cardRect.localScale = Vector3.LerpUnclamped(originalScale * startScale, originalScale * overshootScale, eased);

            if (cardCanvasGroup != null)
                cardCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t);

            SetGlowAlpha(Mathf.Lerp(glowStartAlpha, glowPeakAlpha, t));

            yield return null;
        }

        elapsed = 0f;

        while (elapsed < settleDuration)
        {
            elapsed += DeltaTime();
            float t = Mathf.Clamp01(elapsed / settleDuration);
            float eased = Mathf.SmoothStep(0f, 1f, t);

            cardRect.localScale = Vector3.Lerp(originalScale * overshootScale, originalScale * finalScale, eased);
            SetGlowAlpha(Mathf.Lerp(glowPeakAlpha, glowFinalAlpha, t));

            yield return null;
        }

        cardRect.localScale = originalScale * finalScale;
        SetGlowAlpha(glowFinalAlpha);

        if (astragalosBadgeRect != null && animateAstragalosBadge)
        {
            yield return Wait(badgeDelay);
            yield return StartCoroutine(PopBadge());
        }

        revealCoroutine = null;
    }

    private IEnumerator PopBadge()
    {
        if (astragalosBadgeRect == null)
            yield break;

        Vector3 normal = badgeOriginalScale == Vector3.zero ? Vector3.one : badgeOriginalScale;
        Vector3 big = normal * badgePopScale;

        float half = badgePopDuration * 0.5f;
        float elapsed = 0f;

        while (elapsed < half)
        {
            elapsed += DeltaTime();
            float t = Mathf.Clamp01(elapsed / half);
            astragalosBadgeRect.localScale = Vector3.Lerp(Vector3.zero, big, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }

        elapsed = 0f;

        while (elapsed < half)
        {
            elapsed += DeltaTime();
            float t = Mathf.Clamp01(elapsed / half);
            astragalosBadgeRect.localScale = Vector3.Lerp(big, normal, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }

        astragalosBadgeRect.localScale = normal;
    }

    private void SetGlowAlpha(float alpha)
    {
        if (glowImage == null)
            return;

        Color c = glowImage.color;
        c.a = alpha;
        glowImage.color = c;
    }

    private float DeltaTime()
    {
        return useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
    }

    private WaitForSeconds Wait(float seconds)
    {
        return new WaitForSeconds(seconds);
    }

    private float EaseOutBack(float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;

        return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
    }
}