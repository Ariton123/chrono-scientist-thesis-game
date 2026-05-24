using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsCharacterUI : MonoBehaviour
{
    [SerializeField] private TMP_Text genderButtonText;

    [Header("Character Preview")]
    [SerializeField] private Image characterPreviewImage;
    [SerializeField] private Sprite malePreviewSprite;
    [SerializeField] private Sprite femalePreviewSprite;

    private void OnEnable()
    {
        if (CharacterSelectionManager.Instance != null)
            CharacterSelectionManager.Instance.OnGenderChanged += HandleGenderChanged;

        if (LanguageManager.Instance != null)
            LanguageManager.Instance.OnLanguageChanged += HandleLanguageChanged;

        RefreshUI();
    }

    private void OnDisable()
    {
        if (CharacterSelectionManager.Instance != null)
            CharacterSelectionManager.Instance.OnGenderChanged -= HandleGenderChanged;

        if (LanguageManager.Instance != null)
            LanguageManager.Instance.OnLanguageChanged -= HandleLanguageChanged;
    }

    private void HandleLanguageChanged(GameLanguage language)
    {
        RefreshUI();
    }

    public void ToggleGender()
    {
        if (CharacterSelectionManager.Instance == null)
            return;

        PlayerGender nextGender =
            CharacterSelectionManager.Instance.CurrentGender == PlayerGender.Male
                ? PlayerGender.Female
                : PlayerGender.Male;

        CharacterSelectionManager.Instance.SetGender(nextGender);
    }

    private void HandleGenderChanged(PlayerGender gender)
    {
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (CharacterSelectionManager.Instance == null)
            return;

        PlayerGender gender = CharacterSelectionManager.Instance.CurrentGender;

        if (genderButtonText != null)
        {
            string key = gender == PlayerGender.Male ? "SETTINGS_MALE" : "SETTINGS_FEMALE";

            if (LanguageManager.Instance != null)
                genderButtonText.text = LanguageManager.Instance.GetText(key);
            else
                genderButtonText.text = gender == PlayerGender.Male ? "MALE" : "FEMALE";
        }

        if (characterPreviewImage != null)
            characterPreviewImage.sprite = gender == PlayerGender.Male ? malePreviewSprite : femalePreviewSprite;
    }
}