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

    private const string DefaultNameKey = "BONEINFO_DEFAULT_NAME";
    private const string DefaultDescriptionKey = "BONEINFO_DEFAULT_DESC";

    private string currentNameKey = DefaultNameKey;
    private string currentDescriptionKey = DefaultDescriptionKey;
    private Sprite currentSprite;

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
        currentSprite = boneSprite != null ? boneSprite : defaultSprite;

        currentNameKey = string.IsNullOrEmpty(nameKey) ? DefaultNameKey : nameKey;
        currentDescriptionKey = string.IsNullOrEmpty(descriptionKey) ? DefaultDescriptionKey : descriptionKey;

        RefreshUI();
    }

    public void ClearInfo()
    {
        currentSprite = null; // IMPORTANT
        currentNameKey = DefaultNameKey;
        currentDescriptionKey = DefaultDescriptionKey;

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

            // Hide image if nothing selected
            bonePreviewImage.enabled = currentSprite != null;

            bonePreviewImage.preserveAspect = false;
            bonePreviewImage.color = Color.white;
        }

        if (LanguageManager.Instance == null)
            return;

        if (boneNameText != null)
            boneNameText.text = LanguageManager.Instance.GetText(currentNameKey);

        if (boneDescriptionText != null)
            boneDescriptionText.text = LanguageManager.Instance.GetText(currentDescriptionKey);
    }
}