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

    [Tooltip("If true, the bone keeps its original Z rotation when placed.")]
    public bool preserveStartRotationOnSnap = false;

    [Tooltip("Extra rotation after snapping into the slot. Used only if Preserve Start Rotation On Snap is false.")]
    public float placedRotationZ = 0f;

    [Tooltip("Small manual offset after snapping, normally keep this at (0,0).")]
    public Vector2 placedPositionOffset = Vector2.zero;

    [Header("Aspect Ratio")]
    [Tooltip("Recommended ON for long bones so they do not look smashed in slots.")]
    [SerializeField] private bool preserveAspectWhileDragging = true;

    [Tooltip("Recommended ON for long bones so they do not look smashed after correct placement.")]
    [SerializeField] private bool preserveAspectWhenPlaced = true;

    [Header("Bone Info While Dragging")]
    [Tooltip("Optional. If empty, the script tries to use BoneInfoClickable on this object.")]
    [SerializeField] private BoneInfoPanelUI infoPanel;

    [Tooltip("Optional. If empty, the script tries to use BoneInfoClickable on this object.")]
    [SerializeField] private BoneInfoClickable boneInfoClickable;

    [Tooltip("Optional. If empty, the script uses BoneInfoClickable sprite or this object's Image sprite.")]
    [SerializeField] private Sprite bonePreviewSprite;

    [Tooltip("Optional. If empty, the script uses BoneInfoClickable boneNameKey.")]
    [SerializeField] private string boneNameKey;

    [Tooltip("Optional. If empty, the script uses BoneInfoClickable boneDescriptionKey.")]
    [SerializeField] private string boneDescriptionKey;

    [Tooltip("Optional. If 0, the script uses BoneInfoClickable previewRotationZ.")]
    [SerializeField] private float previewRotationZ = 0f;

    [SerializeField] private bool showInfoOnBeginDrag = true;

    [Tooltip("Recommended OFF. Keeping info visible after placement helps children read/review.")]
    [SerializeField] private bool clearInfoAfterCorrectPlacement = false;

    [Tooltip("Recommended OFF. Keeping info visible after a wrong drop helps children learn.")]
    [SerializeField] private bool clearInfoAfterReset = false;

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
    private bool startPreserveAspect;

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

        if (boneInfoClickable == null)
            boneInfoClickable = GetComponent<BoneInfoClickable>();
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
            startColor = image.color;
            startPreserveAspect = image.preserveAspect;
            image.preserveAspect = preserveAspectWhileDragging;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (placedCorrectly) return;

        wasDroppedOnSlot = false;
        canvasGroup.blocksRaycasts = false;

        if (image != null)
            image.preserveAspect = preserveAspectWhileDragging;

        transform.SetAsLastSibling();

        ShowBoneInfoWhileDragging();
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

        if (preserveStartRotationOnSnap)
            rectTransform.localRotation = startRotation;
        else
            rectTransform.localRotation = Quaternion.Euler(0f, 0f, placedRotationZ);

        rectTransform.localScale = Vector3.one * placedSizeMultiplier;

        if (image != null)
        {
            image.preserveAspect = preserveAspectWhenPlaced;
            image.color = placedColor;
        }

        canvasGroup.blocksRaycasts = true;

        PlayPlacementPop();

        if (clearInfoAfterCorrectPlacement)
            ClearBoneInfoPanel();
    }

    private void ShowBoneInfoWhileDragging()
    {
        if (!showInfoOnBeginDrag)
            return;

        BoneInfoPanelUI resolvedInfoPanel = ResolveInfoPanel();

        if (resolvedInfoPanel == null)
            return;

        Sprite resolvedPreviewSprite = ResolvePreviewSprite();
        string resolvedNameKey = ResolveBoneNameKey();
        string resolvedDescriptionKey = ResolveBoneDescriptionKey();
        float resolvedPreviewRotationZ = ResolvePreviewRotationZ();

        if (string.IsNullOrEmpty(resolvedNameKey) && string.IsNullOrEmpty(resolvedDescriptionKey))
        {
            Debug.LogWarning($"{name}: No bone info localization keys assigned for drag info.");
            return;
        }

        resolvedInfoPanel.ShowBoneInfo(
            resolvedPreviewSprite,
            resolvedNameKey,
            resolvedDescriptionKey,
            resolvedPreviewRotationZ
        );
    }

    private BoneInfoPanelUI ResolveInfoPanel()
    {
        if (infoPanel != null)
            return infoPanel;

        if (boneInfoClickable != null && boneInfoClickable.infoPanel != null)
            return boneInfoClickable.infoPanel;

        return null;
    }

    private Sprite ResolvePreviewSprite()
    {
        if (bonePreviewSprite != null)
            return bonePreviewSprite;

        if (boneInfoClickable != null && boneInfoClickable.bonePreviewSprite != null)
            return boneInfoClickable.bonePreviewSprite;

        if (image != null && image.sprite != null)
            return image.sprite;

        return null;
    }

    private string ResolveBoneNameKey()
    {
        if (!string.IsNullOrEmpty(boneNameKey))
            return boneNameKey;

        if (boneInfoClickable != null)
            return boneInfoClickable.boneNameKey;

        return "";
    }

    private string ResolveBoneDescriptionKey()
    {
        if (!string.IsNullOrEmpty(boneDescriptionKey))
            return boneDescriptionKey;

        if (boneInfoClickable != null)
            return boneInfoClickable.boneDescriptionKey;

        return "";
    }

    private float ResolvePreviewRotationZ()
    {
        if (Mathf.Abs(previewRotationZ) > 0.001f)
            return previewRotationZ;

        if (boneInfoClickable != null)
            return boneInfoClickable.previewRotationZ;

        return 0f;
    }

    private void ClearBoneInfoPanel()
    {
        BoneInfoPanelUI resolvedInfoPanel = ResolveInfoPanel();

        if (resolvedInfoPanel != null)
            resolvedInfoPanel.ClearInfo();
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

        if (clearInfoAfterReset)
            ClearBoneInfoPanel();
    }

    public void ForceResetToStart()
    {
        placedCorrectly = false;
        wasDroppedOnSlot = false;

        ResetVisualStateToStart();

        if (clearInfoAfterReset)
            ClearBoneInfoPanel();
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
            image.preserveAspect = preserveAspectWhileDragging;
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