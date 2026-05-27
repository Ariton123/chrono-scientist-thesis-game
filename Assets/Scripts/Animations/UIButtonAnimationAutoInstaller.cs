using UnityEngine;
using UnityEngine.UI;

public class UIButtonAnimationAutoInstaller : MonoBehaviour
{
    [Header("Auto Install Settings")]
    [SerializeField] private bool includeInactiveButtons = true;
    [SerializeField] private bool installOnStart = true;

    private void Start()
    {
        if (installOnStart)
            InstallAnimations();
    }

    [ContextMenu("Install Button Animations Now")]
    public void InstallAnimations()
    {
        Button[] buttons = FindObjectsByType<Button>(
            includeInactiveButtons ? FindObjectsInactive.Include : FindObjectsInactive.Exclude,
            FindObjectsSortMode.None
        );

        int addedCount = 0;
        int alreadyHadCount = 0;

        foreach (Button button in buttons)
        {
            if (button.GetComponent<UIButtonAnimator>() == null)
            {
                button.gameObject.AddComponent<UIButtonAnimator>();
                addedCount++;
            }
            else
            {
                alreadyHadCount++;
            }
        }

        Debug.Log($"UIButtonAnimationAutoInstaller: Added UIButtonAnimator to {addedCount} buttons. Already had it: {alreadyHadCount}.");
    }
}