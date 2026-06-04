using UnityEngine;
using UnityEngine.SceneManagement;

public class OutroSceneController : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}