using UnityEngine;

public class Stage1MissionPanel : MonoBehaviour
{
    public Stage1GameManager gameManager;

    public void OnReadyClicked()
    {
        gameManager.StartAssemblyPhase();
    }
}