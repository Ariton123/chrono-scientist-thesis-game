using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoneInfoClickable : MonoBehaviour, IPointerClickHandler
{
    public BoneInfoPanelUI infoPanel;

    [Header("Bone Info")]
    public Sprite bonePreviewSprite;

    [Header("Preview Settings")]
    [Tooltip("Use this to rotate the bone inside the BoneInfoPanel preview. Example: 90 or -90 for long bones.")]
    public float previewRotationZ = 0f;

    [Header("Localization Keys")]
    public string boneNameKey;
    public string boneDescriptionKey;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (infoPanel == null)
        {
            Debug.LogWarning($"{name}: Missing BoneInfoPanelUI reference.");
            return;
        }

        if (bonePreviewSprite == null)
        {
            Image ownImage = GetComponent<Image>();

            if (ownImage != null && ownImage.sprite != null)
                bonePreviewSprite = ownImage.sprite;
            else
                Debug.LogWarning($"{name}: Missing bonePreviewSprite.");
        }

        infoPanel.ShowBoneInfo(bonePreviewSprite, boneNameKey, boneDescriptionKey, previewRotationZ);
    }
}