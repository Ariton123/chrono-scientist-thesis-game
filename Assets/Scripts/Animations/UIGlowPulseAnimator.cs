using UnityEngine;
using UnityEngine.UI;

public class UIGlowPulseAnimator : MonoBehaviour
{
    [Header("Optional References")]
    [SerializeField] private Image targetImage;

    [Header("Glow Pulse")]
    [SerializeField] private bool animateAlpha = true;
    [SerializeField] private float minAlpha = 0.45f;
    [SerializeField] private float maxAlpha = 1f;
    [SerializeField] private float alphaSpeed = 1.4f;

    [Header("Scale Pulse")]
    [SerializeField] private bool animateScale = true;
    [SerializeField] private float minScaleMultiplier = 0.92f;
    [SerializeField] private float maxScaleMultiplier = 1.08f;
    [SerializeField] private float scaleSpeed = 1.1f;

    [Header("Timing")]
    [SerializeField] private bool useUnscaledTime = true;
    [SerializeField] private float phaseOffset = 0f;

    private RectTransform rectTransform;
    private Vector3 startScale;
    private Color startColor;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        if (targetImage == null)
            targetImage = GetComponent<Image>();

        if (rectTransform != null)
            startScale = rectTransform.localScale;

        if (targetImage != null)
            startColor = targetImage.color;
    }

    private void Update()
    {
        float time = useUnscaledTime ? Time.unscaledTime : Time.time;
        time += phaseOffset;

        if (targetImage != null && animateAlpha)
        {
            float alphaT = (Mathf.Sin(time * alphaSpeed) + 1f) * 0.5f;
            float alpha = Mathf.Lerp(minAlpha, maxAlpha, alphaT);

            Color c = startColor;
            c.a = alpha;
            targetImage.color = c;
        }

        if (rectTransform != null && animateScale)
        {
            float scaleT = (Mathf.Sin(time * scaleSpeed) + 1f) * 0.5f;
            float scale = Mathf.Lerp(minScaleMultiplier, maxScaleMultiplier, scaleT);

            rectTransform.localScale = startScale * scale;
        }
    }
}