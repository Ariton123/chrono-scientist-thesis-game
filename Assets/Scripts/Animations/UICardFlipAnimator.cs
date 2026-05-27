using System;
using System.Collections;
using UnityEngine;

public class UICardFlipAnimator : MonoBehaviour
{
    [SerializeField] private RectTransform cardRect;
    [SerializeField] private float halfFlipDuration = 0.16f;
    [SerializeField] private bool useUnscaledTime = true;

    private bool isFlipping = false;

    private void Awake()
    {
        if (cardRect == null)
            cardRect = GetComponent<RectTransform>();
    }

    public void PlayFlip(Action onMiddleFlip)
    {
        if (isFlipping || cardRect == null)
            return;

        StartCoroutine(FlipRoutine(onMiddleFlip));
    }

    private IEnumerator FlipRoutine(Action onMiddleFlip)
    {
        isFlipping = true;

        Vector3 originalScale = cardRect.localScale;

        float elapsed = 0f;

        while (elapsed < halfFlipDuration)
        {
            elapsed += DeltaTime();
            float t = Mathf.Clamp01(elapsed / halfFlipDuration);

            float x = Mathf.Lerp(originalScale.x, 0f, Mathf.SmoothStep(0f, 1f, t));
            cardRect.localScale = new Vector3(x, originalScale.y, originalScale.z);

            yield return null;
        }

        onMiddleFlip?.Invoke();

        elapsed = 0f;

        while (elapsed < halfFlipDuration)
        {
            elapsed += DeltaTime();
            float t = Mathf.Clamp01(elapsed / halfFlipDuration);

            float x = Mathf.Lerp(0f, originalScale.x, Mathf.SmoothStep(0f, 1f, t));
            cardRect.localScale = new Vector3(x, originalScale.y, originalScale.z);

            yield return null;
        }

        cardRect.localScale = originalScale;
        isFlipping = false;
    }

    private float DeltaTime()
    {
        return useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
    }
}