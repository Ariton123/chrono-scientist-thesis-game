using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIBoneDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string boneID;

    [Header("Placement Tuning")]
    [Tooltip("1 = exact slot size. Smaller/larger values can fine-tune specific bones if needed.")]
    public float placedSizeMultiplier = 1f;

    [Tooltip("Extra rotation after snapping into the slot.")]
    public float placedRotationZ = 0f;

    [Tooltip("Small manual offset after snapping, normally keep this at (0,0).")]
    public Vector2 placedPositionOffset = Vector2.zero;

    [Header("Correct Placement Highlight")]
    [SerializeField] private Color placedColor = new Color(1f, 0.2f, 0.2f, 1f);

    [Header("Correct Placement Pop")]
    [SerializeField] private float popScale = 1.15f;
    [SerializeField] private float popDuration = 0.1f;

    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Image image;

    private Vector2 startAnchoredPosition;
    private Vector2 startSizeDelta;
    private Vector2 startAnchorMin;
    private Vector2 startAnchorMax;
    private Vector2 startPivot;
    private Vector3 startLocalScale;
    private Quaternion startRotation;
    private Transform startParent;
    private Color startColor;

    private bool placedCorrectly = false;
    private bool wasDroppedOnSlot = false;

    private Coroutine popCoroutine;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        image = GetComponent<Image>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    private void Start()
    {
        CaptureStartState();
    }

    private void CaptureStartState()
    {
        startParent = transform.parent;

        startAnchoredPosition = rectTransform.anchoredPosition;
        startSizeDelta = rectTransform.sizeDelta;
        startAnchorMin = rectTransform.anchorMin;
        startAnchorMax = rectTransform.anchorMax;
        startPivot = rectTransform.pivot;
        startLocalScale = rectTransform.localScale;
        startRotation = rectTransform.localRotation;

        if (image != null)
        {
            image.preserveAspect = false;
            startColor = image.color;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (placedCorrectly) return;

        wasDroppedOnSlot = false;
        canvasGroup.blocksRaycasts = false;

        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (placedCorrectly) return;
        if (canvas == null) return;

        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (placedCorrectly) return;

        canvasGroup.blocksRaycasts = true;

        if (!wasDroppedOnSlot)
            ResetToStart();
    }

    public void SnapToSlot(RectTransform slotTransform)
    {
        if (slotTransform == null) return;

        placedCorrectly = true;
        wasDroppedOnSlot = true;

        transform.SetParent(slotTransform, false);
        rectTransform.SetAsLastSibling();

        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        rectTransform.anchoredPosition = placedPositionOffset;
        rectTransform.sizeDelta = Vector2.zero;

        rectTransform.localRotation = Quaternion.Euler(0f, 0f, placedRotationZ);
        rectTransform.localScale = Vector3.one * placedSizeMultiplier;

        if (image != null)
        {
            image.preserveAspect = false;
            image.color = placedColor;
        }

        canvasGroup.blocksRaycasts = true;

        PlayPlacementPop();
    }

    private void PlayPlacementPop()
    {
        if (popCoroutine != null)
            StopCoroutine(popCoroutine);

        popCoroutine = StartCoroutine(PopBack());
    }

    private IEnumerator PopBack()
    {
        Vector3 normalScale = Vector3.one * placedSizeMultiplier;
        Vector3 biggerScale = normalScale * popScale;

        rectTransform.localScale = biggerScale;

        yield return new WaitForSeconds(popDuration);

        rectTransform.localScale = normalScale;
        popCoroutine = null;
    }

    public void MarkDroppedOnSlot()
    {
        wasDroppedOnSlot = true;
    }

    public void ResetToStart()
    {
        if (placedCorrectly) return;

        ResetVisualStateToStart();
    }

    public void ForceResetToStart()
    {
        placedCorrectly = false;
        wasDroppedOnSlot = false;

        ResetVisualStateToStart();
    }

    private void ResetVisualStateToStart()
    {
        if (popCoroutine != null)
        {
            StopCoroutine(popCoroutine);
            popCoroutine = null;
        }

        transform.SetParent(startParent, false);

        rectTransform.anchorMin = startAnchorMin;
        rectTransform.anchorMax = startAnchorMax;
        rectTransform.pivot = startPivot;
        rectTransform.anchoredPosition = startAnchoredPosition;
        rectTransform.sizeDelta = startSizeDelta;
        rectTransform.localScale = startLocalScale;
        rectTransform.localRotation = startRotation;

        if (image != null)
        {
            image.preserveAspect = false;
            image.color = startColor;
        }

        canvasGroup.blocksRaycasts = true;
        wasDroppedOnSlot = false;
    }

    public bool IsPlacedCorrectly()
    {
        return placedCorrectly;
    }
}