using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Scale Settings")]
    [SerializeField] private float hoverScale = 1.06f;
    [SerializeField] private float pressedScale = 0.95f;
    [SerializeField] private float animationSpeed = 12f;

    [Header("Options")]
    [SerializeField] private bool useUnscaledTime = true;

    private RectTransform rectTransform;
    private Vector3 originalScale;
    private Coroutine scaleRoutine;

    private bool isHovering;
    private bool isPressed;
    private Button button;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        button = GetComponent<Button>();

        // Stores the real original scale.
        // So even if a button is not exactly 1,1,1, it still returns correctly.
        originalScale = rectTransform.localScale;
    }

    private void OnEnable()
    {
        isHovering = false;
        isPressed = false;

        if (rectTransform != null)
            rectTransform.localScale = originalScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!CanAnimate()) return;

        isHovering = true;

        if (!isPressed)
            AnimateTo(originalScale * hoverScale);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!CanAnimate()) return;

        isHovering = false;

        if (!isPressed)
            AnimateTo(originalScale);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!CanAnimate()) return;

        isPressed = true;
        AnimateTo(originalScale * pressedScale);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!CanAnimate()) return;

        isPressed = false;

        // If mouse/finger is still over the button, return to hover scale.
        // Otherwise return to normal scale.
        if (isHovering)
            AnimateTo(originalScale * hoverScale);
        else
            AnimateTo(originalScale);
    }

    private bool CanAnimate()
    {
        return button != null && button.interactable && gameObject.activeInHierarchy;
    }

    private void AnimateTo(Vector3 targetScale)
    {
        if (scaleRoutine != null)
            StopCoroutine(scaleRoutine);

        scaleRoutine = StartCoroutine(ScaleRoutine(targetScale));
    }

    private IEnumerator ScaleRoutine(Vector3 targetScale)
    {
        while (Vector3.Distance(rectTransform.localScale, targetScale) > 0.001f)
        {
            float deltaTime = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

            rectTransform.localScale = Vector3.Lerp(
                rectTransform.localScale,
                targetScale,
                animationSpeed * deltaTime
            );

            yield return null;
        }

        rectTransform.localScale = targetScale;
        scaleRoutine = null;
    }
}