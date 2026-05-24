using System.Collections;
using UnityEngine;

public class UIPanelSlideAnimator : MonoBehaviour
{
    public enum SlideFrom
    {
        Top,
        Bottom,
        Left,
        Right
    }

    [Header("Slide Settings")]
    [SerializeField] private SlideFrom slideFrom = SlideFrom.Top;
    [SerializeField] private float slideDistance = 1200f;
    [SerializeField] private float duration = 0.75f;

    [Header("Feel")]
    [SerializeField] private bool useUnscaledTime = true;
    [SerializeField] private bool addSmallBounce = true;
    [SerializeField] private float bounceAmount = 35f;

    private RectTransform rectTransform;
    private Vector2 shownPosition;
    private Coroutine animationCoroutine;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        shownPosition = rectTransform.anchoredPosition;
    }

    public void PlayIn()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        gameObject.SetActive(true);

        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        Vector2 hiddenPosition = GetHiddenPosition();
        rectTransform.anchoredPosition = hiddenPosition;

        animationCoroutine = StartCoroutine(AnimateIn(hiddenPosition, shownPosition));
    }

    public void HideInstant()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }

        if (rectTransform != null)
            rectTransform.anchoredPosition = shownPosition;

        gameObject.SetActive(false);
    }

    private Vector2 GetHiddenPosition()
    {
        Vector2 offset = Vector2.zero;

        switch (slideFrom)
        {
            case SlideFrom.Top:
                offset = new Vector2(0f, slideDistance);
                break;

            case SlideFrom.Bottom:
                offset = new Vector2(0f, -slideDistance);
                break;

            case SlideFrom.Left:
                offset = new Vector2(-slideDistance, 0f);
                break;

            case SlideFrom.Right:
                offset = new Vector2(slideDistance, 0f);
                break;
        }

        return shownPosition + offset;
    }

    private IEnumerator AnimateIn(Vector2 from, Vector2 to)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / duration);
            float eased = EaseOutCubic(t);

            rectTransform.anchoredPosition = Vector2.LerpUnclamped(from, to, eased);

            yield return null;
        }

        rectTransform.anchoredPosition = to;

        if (addSmallBounce)
            yield return StartCoroutine(Bounce(to));

        animationCoroutine = null;
    }

    private IEnumerator Bounce(Vector2 finalPosition)
    {
        Vector2 bounceDirection = Vector2.zero;

        switch (slideFrom)
        {
            case SlideFrom.Top:
                bounceDirection = Vector2.down;
                break;

            case SlideFrom.Bottom:
                bounceDirection = Vector2.up;
                break;

            case SlideFrom.Left:
                bounceDirection = Vector2.right;
                break;

            case SlideFrom.Right:
                bounceDirection = Vector2.left;
                break;
        }

        Vector2 bouncePosition = finalPosition + bounceDirection * bounceAmount;

        float bounceDuration = 0.16f;
        float elapsed = 0f;

        while (elapsed < bounceDuration)
        {
            elapsed += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / bounceDuration);
            float eased = Mathf.SmoothStep(0f, 1f, t);

            rectTransform.anchoredPosition = Vector2.Lerp(finalPosition, bouncePosition, eased);

            yield return null;
        }

        elapsed = 0f;

        while (elapsed < bounceDuration)
        {
            elapsed += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / bounceDuration);
            float eased = Mathf.SmoothStep(0f, 1f, t);

            rectTransform.anchoredPosition = Vector2.Lerp(bouncePosition, finalPosition, eased);

            yield return null;
        }

        rectTransform.anchoredPosition = finalPosition;
    }

    private float EaseOutCubic(float t)
    {
        return 1f - Mathf.Pow(1f - t, 3f);
    }
}