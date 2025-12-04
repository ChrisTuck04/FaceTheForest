using UnityEngine;

public class WinGameTrigger : MonoBehaviour
{
    [Header("UI Reference")]
    [Tooltip("Drag your WinCanvas here. If empty, it looks for an object named 'WinCanvas'.")]
    public GameObject winCanvas;

    private bool hasTriggered = false;

    void Start()
    {
        // AUTOMATION: If you forgot to drag it in, or if this is a prefab 
        // that can't hold scene references, try to find it by name.
        if (winCanvas == null)
        {
            winCanvas = GameObject.Find("WinCanvas"); 
            if (winCanvas == null) Debug.LogError("WinGameTrigger: Could not find 'WinCanvas' in the scene!");
        }
        
        // Ensure it starts hidden, just in case
        if (winCanvas != null) winCanvas.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        // Check for Player
        if (other.CompareTag("Player")) 
        {
            hasTriggered = true;
            ActivateWinScreen();
        }
    }

    void ActivateWinScreen()
    {
        if (winCanvas != null)
        {
            // 1. Show the UI
            winCanvas.SetActive(true);

            // 2. Unlock the Mouse (Crucial so they can click Restart)
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}