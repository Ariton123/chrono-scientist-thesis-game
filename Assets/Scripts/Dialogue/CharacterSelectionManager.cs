using UnityEngine;

public enum PlayerGender
{
    Male,
    Female
}

public class CharacterSelectionManager : MonoBehaviour
{
    public static CharacterSelectionManager Instance;

    private const string GenderPrefKey = "PlayerGender";

    public PlayerGender CurrentGender { get; private set; } = PlayerGender.Male;

    public event System.Action<PlayerGender> OnGenderChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadGender();
    }

    public void SetGender(PlayerGender gender)
    {
        CurrentGender = gender;
        PlayerPrefs.SetInt(GenderPrefKey, (int)CurrentGender);
        PlayerPrefs.Save();

        OnGenderChanged?.Invoke(CurrentGender);
    }

    private void LoadGender()
    {
        CurrentGender = (PlayerGender)PlayerPrefs.GetInt(GenderPrefKey, (int)PlayerGender.Male);
    }
}