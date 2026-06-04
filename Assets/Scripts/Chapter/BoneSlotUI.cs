using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoneSlotUI : MonoBehaviour, IDropHandler
{
    public string requiredBoneID;
    public Stage1AssemblyController assemblyController;

    [Header("Snap Target")]
    [Tooltip("Optional. If assigned, the dragged bone snaps to this RectTransform instead of the slot root.")]
    [SerializeField] private RectTransform snapTarget;

    [Header("Correct Slot Visual")]
    [SerializeField] private float placedSlotAlpha = 0.25f;
    [SerializeField] private bool fadeOnCorrect = false;

    private Image slotImage;
    private bool alreadyFilled = false;
    private Color startColor;

    private void Awake()
    {
        slotImage = GetComponent<Image>();

        if (slotImage != null)
            startColor = slotImage.color;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (alreadyFilled) return;
        if (eventData.pointerDrag == null) return;

        UIBoneDrag draggedBone = eventData.pointerDrag.GetComponent<UIBoneDrag>();
        if (draggedBone == null) return;

        draggedBone.MarkDroppedOnSlot();

        if (draggedBone.boneID == requiredBoneID)
        {
            alreadyFilled = true;

            RectTransform targetRect = snapTarget != null
                ? snapTarget
                : GetComponent<RectTransform>();

            draggedBone.SnapToSlot(targetRect);

            FadeSlotVisual();

            if (assemblyController != null)
                assemblyController.RegisterCorrectPlacement();
        }
        else
        {
            if (assemblyController != null)
                assemblyController.RegisterMistake();

            draggedBone.ResetToStart();
        }
    }

    public void ResetSlot()
    {
        alreadyFilled = false;

        if (slotImage != null)
            slotImage.color = startColor;
    }

    private void FadeSlotVisual()
    {
        if (!fadeOnCorrect) return;
        if (slotImage == null) return;

        Color color = slotImage.color;
        color.a = placedSlotAlpha;
        slotImage.color = color;
    }
}