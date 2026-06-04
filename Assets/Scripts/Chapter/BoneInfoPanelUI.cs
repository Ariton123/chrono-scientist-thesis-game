using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoneInfoPanelUI : MonoBehaviour
{
    [Header("UI References")]
    public Image bonePreviewImage;
    public TMP_Text boneNameText;
    public TMP_Text boneDescriptionText;

    [Header("Default State")]
    public Sprite defaultSprite;

    [Header("Preview Settings")]
    [SerializeField] private bool preservePreviewAspect = true;

    private const string DefaultNameKey = "BONEINFO_DEFAULT_NAME";
    private const string DefaultDescriptionKey = "BONEINFO_DEFAULT_DESC";

    private string currentNameKey = DefaultNameKey;
    private string currentDescriptionKey = DefaultDescriptionKey;
    private Sprite currentSprite;
    private float currentPreviewRotationZ = 0f;

    private bool isShowingDefault = true;

    private void OnEnable()
    {
        if (LanguageManager.Instance != null)
            LanguageManager.Instance.OnLanguageChanged += HandleLanguageChanged;
    }

    private void OnDisable()
    {
        if (LanguageManager.Instance != null)
            LanguageManager.Instance.OnLanguageChanged -= HandleLanguageChanged;
    }

    private void Start()
    {
        ClearInfo();
    }

    public void ShowBoneInfo(Sprite boneSprite, string nameKey, string descriptionKey)
    {
        ShowBoneInfo(boneSprite, nameKey, descriptionKey, 0f);
    }

    public void ShowBoneInfo(Sprite boneSprite, string nameKey, string descriptionKey, float previewRotationZ)
    {
        isShowingDefault = false;

        currentSprite = boneSprite != null ? boneSprite : defaultSprite;

        currentNameKey = string.IsNullOrEmpty(nameKey) ? DefaultNameKey : nameKey;
        currentDescriptionKey = string.IsNullOrEmpty(descriptionKey) ? DefaultDescriptionKey : descriptionKey;
        currentPreviewRotationZ = previewRotationZ;

        RefreshUI();
    }

    public void ClearInfo()
    {
        isShowingDefault = true;

        currentSprite = null;
        currentNameKey = DefaultNameKey;
        currentDescriptionKey = DefaultDescriptionKey;
        currentPreviewRotationZ = 0f;

        RefreshUI();
    }

    private void HandleLanguageChanged(GameLanguage language)
    {
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (bonePreviewImage != null)
        {
            bonePreviewImage.sprite = currentSprite;
            bonePreviewImage.enabled = currentSprite != null;
            bonePreviewImage.preserveAspect = preservePreviewAspect;
            bonePreviewImage.color = Color.white;
            bonePreviewImage.rectTransform.localRotation = Quaternion.Euler(0f, 0f, currentPreviewRotationZ);
        }

        if (LanguageManager.Instance == null)
            return;

        if (boneNameText != null)
            boneNameText.text = LanguageManager.Instance.GetText(currentNameKey);

        if (boneDescriptionText != null)
        {
            boneDescriptionText.text = LanguageManager.Instance.GetText(currentDescriptionKey);

            if (isShowingDefault)
                boneDescriptionText.alignment = TextAlignmentOptions.Center;
            else
                boneDescriptionText.alignment = TextAlignmentOptions.Left;
        }
    }
}