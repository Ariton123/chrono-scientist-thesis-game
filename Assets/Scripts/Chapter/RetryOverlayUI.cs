using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RetryOverlayUI : MonoBehaviour
{
    [Header("Overlay")]
    [SerializeField] private GameObject retryOverlay;

    [Header("Texts")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text retryButtonText;
    [SerializeField] private TMP_Text mainMenuButtonText;

    [Header("Localization Keys")]
    [SerializeField] private string titleKey = "TIME_RAN_OUT";
    [SerializeField] private string descriptionKey = "FAIL_DESC_NIBBLES";
    [SerializeField] private string retryButtonKey = "RETRY";
    [SerializeField] private string mainMenuButtonKey = "MAIN_MENU";

    [Header("Character")]
    [SerializeField] private Image sadCharacterImage;
    [SerializeField] private Sprite sadCharacterSprite;

    private void OnEnable()
    {
        if (LanguageManager.Instance != null)
            LanguageManager.Instance.OnLanguageChanged += HandleLanguageChanged;

        RefreshUI();
    }

    private void OnDisable()
    {
        if (LanguageManager.Instance != null)
            LanguageManager.Instance.OnLanguageChanged -= HandleLanguageChanged;
    }

    private void Start()
    {
        RefreshUI();
    }

    public void Show()
    {
        if (retryOverlay != null)
        {
            retryOverlay.SetActive(true);
            retryOverlay.transform.SetAsLastSibling();
        }
        else
        {
            gameObject.SetActive(true);
            transform.SetAsLastSibling();
        }

        RefreshUI();

        Debug.Log("[RetryOverlayUI] Retry overlay shown.");
    }

    public void Hide()
    {
        if (retryOverlay != null)
            retryOverlay.SetActive(false);
        else
            gameObject.SetActive(false);

        Debug.Log("[RetryOverlayUI] Retry overlay hidden.");
    }

    private void HandleLanguageChanged(GameLanguage language)
    {
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (LanguageManager.Instance != null)
        {
            if (titleText != null)
                titleText.text = LanguageManager.Instance.GetText(titleKey);

            if (descriptionText != null)
                descriptionText.text = LanguageManager.Instance.GetText(descriptionKey);

            if (retryButtonText != null)
                retryButtonText.text = LanguageManager.Instance.GetText(retryButtonKey);

            if (mainMenuButtonText != null)
                mainMenuButtonText.text = LanguageManager.Instance.GetText(mainMenuButtonKey);
        }
        else
        {
            if (titleText != null)
                titleText.text = "TIME RAN OUT!";

            if (descriptionText != null)
                descriptionText.text = "Time ran out. Try again and complete the challenge!";

            if (retryButtonText != null)
                retryButtonText.text = "RETRY";

            if (mainMenuButtonText != null)
                mainMenuButtonText.text = "MAIN MENU";
        }

        if (sadCharacterImage != null)
        {
            sadCharacterImage.sprite = sadCharacterSprite;
            sadCharacterImage.enabled = sadCharacterSprite != null;
            sadCharacterImage.preserveAspect = true;
            sadCharacterImage.color = Color.white;
        }
    }
}