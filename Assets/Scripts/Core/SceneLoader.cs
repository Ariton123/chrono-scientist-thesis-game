using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Load scene by build index
    public void LoadSceneByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    // Load scene by name (optional alternative)
    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // Quit the application
    public void QuitGame()
    {
        ResetPrototypeProgressOnly();

        Debug.Log("Quit Game");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void ResetPrototypeProgressOnly()
    {
        DeleteRewardKeys("Reward_Level1_Nibbles");
        DeleteRewardKeys("Reward_Level5_Yeti");
        DeleteRewardKeys("Reward_Level9_Achilles");

        PlayerPrefs.DeleteKey("CompletedPlayableLevels");
        PlayerPrefs.DeleteKey("TotalMistakes");
        PlayerPrefs.DeleteKey("TotalCompletionTime");

        PlayerPrefs.Save();

        Debug.Log("[SceneLoader] Prototype progress reset before quit.");
    }

    private void DeleteRewardKeys(string rewardKey)
    {
        // Discovery card unlock state
        PlayerPrefs.DeleteKey(rewardKey);

        // Best achievement stats
        PlayerPrefs.DeleteKey(rewardKey + "_Rank");
        PlayerPrefs.DeleteKey(rewardKey + "_AstragalosBadge");
        PlayerPrefs.DeleteKey(rewardKey + "_Mistakes");
        PlayerPrefs.DeleteKey(rewardKey + "_CompletionTime");

        // Latest run stats
        PlayerPrefs.DeleteKey(rewardKey + "_LastRank");
        PlayerPrefs.DeleteKey(rewardKey + "_LastAstragalosBadge");
        PlayerPrefs.DeleteKey(rewardKey + "_LastMistakes");
        PlayerPrefs.DeleteKey(rewardKey + "_LastCompletionTime");
    }
}