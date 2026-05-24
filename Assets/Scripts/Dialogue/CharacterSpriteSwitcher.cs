using UnityEngine;
using UnityEngine.UI;

public enum PlayerSpriteExpression
{
    Normal,
    Smiling
}

public class CharacterSpriteSwitcher : MonoBehaviour
{
    [Header("Male Sprites")]
    [SerializeField] private Sprite maleNormalSprite;
    [SerializeField] private Sprite maleSmilingSprite;

    [Header("Female Sprites")]
    [SerializeField] private Sprite femaleNormalSprite;
    [SerializeField] private Sprite femaleSmilingSprite;

    [Header("Default Expression")]
    [SerializeField] private PlayerSpriteExpression defaultExpression = PlayerSpriteExpression.Normal;

    private Image uiImage;
    private SpriteRenderer spriteRenderer;

    private const string GenderPrefKey = "PlayerGender";

    private void Awake()
    {
        uiImage = GetComponent<Image>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        if (CharacterSelectionManager.Instance != null)
            CharacterSelectionManager.Instance.OnGenderChanged += HandleGenderChanged;

        RefreshSprite();
    }

    private void Start()
    {
        RefreshSprite();
    }

    private void OnDisable()
    {
        if (CharacterSelectionManager.Instance != null)
            CharacterSelectionManager.Instance.OnGenderChanged -= HandleGenderChanged;
    }

    private void HandleGenderChanged(PlayerGender gender)
    {
        RefreshSprite();
    }

    public void RefreshSprite()
    {
        RefreshSprite(defaultExpression);
    }

    public void RefreshSprite(PlayerSpriteExpression expression)
    {
        PlayerGender selectedGender = PlayerGender.Male;

        if (CharacterSelectionManager.Instance != null)
        {
            selectedGender = CharacterSelectionManager.Instance.CurrentGender;
        }
        else
        {
            selectedGender = (PlayerGender)PlayerPrefs.GetInt(GenderPrefKey, (int)PlayerGender.Male);
        }

        Sprite selectedSprite = GetSprite(selectedGender, expression);

        if (selectedSprite == null)
            return;

        if (uiImage != null)
            uiImage.sprite = selectedSprite;

        if (spriteRenderer != null)
            spriteRenderer.sprite = selectedSprite;
    }

    private Sprite GetSprite(PlayerGender gender, PlayerSpriteExpression expression)
    {
        if (gender == PlayerGender.Male)
        {
            if (expression == PlayerSpriteExpression.Smiling && maleSmilingSprite != null)
                return maleSmilingSprite;

            return maleNormalSprite;
        }

        if (expression == PlayerSpriteExpression.Smiling && femaleSmilingSprite != null)
            return femaleSmilingSprite;

        return femaleNormalSprite;
    }
}