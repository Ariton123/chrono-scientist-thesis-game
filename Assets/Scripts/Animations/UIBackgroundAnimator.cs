using UnityEngine;

public class UIBackgroundAnimator : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private bool animatePosition = true;
    [SerializeField] private float moveAmountX = 12f;
    [SerializeField] private float moveAmountY = 8f;
    [SerializeField] private float moveSpeed = 0.25f;

    [Header("Zoom")]
    [SerializeField] private bool animateScale = true;
    [SerializeField] private float scaleAmount = 0.025f;
    [SerializeField] private float scaleSpeed = 0.18f;

    [Header("Timing")]
    [SerializeField] private bool useUnscaledTime = true;

    private RectTransform rectTransform;
    private Vector2 startAnchoredPosition;
    private Vector3 startScale;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        if (rectTransform != null)
        {
            startAnchoredPosition = rectTransform.anchoredPosition;
            startScale = rectTransform.localScale;
        }
    }

    private void Update()
    {
        if (rectTransform == null)
            return;

        float time = useUnscaledTime ? Time.unscaledTime : Time.time;

        if (animatePosition)
        {
            float x = Mathf.Sin(time * moveSpeed) * moveAmountX;
            float y = Mathf.Cos(time * moveSpeed * 0.8f) * moveAmountY;

            rectTransform.anchoredPosition = startAnchoredPosition + new Vector2(x, y);
        }

        if (animateScale)
        {
            float scaleOffset = Mathf.Sin(time * scaleSpeed) * scaleAmount;
            float finalScale = 1f + scaleOffset;

            rectTransform.localScale = startScale * finalScale;
        }
    }

    public void ResetAnimation()
    {
        if (rectTransform == null)
            return;

        rectTransform.anchoredPosition = startAnchoredPosition;
        rectTransform.localScale = startScale;
    }
}