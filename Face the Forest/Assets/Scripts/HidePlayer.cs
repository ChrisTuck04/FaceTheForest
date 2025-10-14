using UnityEngine;

public class HidePlayer : MonoBehaviour
{
    //public GameObject hidingText;
    //public enemyAI monsterScript;

    GameManager gameManager;

    public PlayerMovement pmScript;

    private void Start()
    {
        gameManager = GameManager.instance;
        if (pmScript == null)
        {
            pmScript = FindFirstObjectByType<PlayerMovement>();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.PlayerEnterHiding();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.PlayerExitHiding();
        }
    }
}
