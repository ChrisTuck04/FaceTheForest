using UnityEngine;

public class HidePlayer : MonoBehaviour
{
    GameManager gameManager;
    
    // We don't need to cache pmScript in Start because we get it from the Collider now
    // This ensures we always get the LIVE player, not the dead one.

    private void Start()
    {
        gameManager = GameManager.instance;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            
            if (player != null)
            {
                player.FlipHindered();
                player.MovementHandling();
            }

            if (gameManager != null) gameManager.PlayerEnterHiding();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            
            if (player != null)
            {
                player.FlipHindered();
                player.MovementHandling(); 
            }

            if (gameManager != null) gameManager.PlayerExitHiding();
        }
    }
}