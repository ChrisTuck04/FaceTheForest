using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int hinderance = 2; // Amount to reduce player speed when in hiding spot
    public bool hiding = false;
    bool playerInHidingSpot = false;
    bool isCrouching = false;
    PlayerMovement pmScript;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        pmScript = FindFirstObjectByType<PlayerMovement>();
    }

    public void PlayerEnterHiding()
    {
        playerInHidingSpot = true;
        pmScript.moveSpeed = hinderance;
        CheckHiding();
    }

    public void PlayerExitHiding()
    {
        playerInHidingSpot = false;
        pmScript.moveSpeed += hinderance;
        CheckHiding();
    }
    
    public void SetCrouching()
    {
        isCrouching = !isCrouching;

        CheckHiding(); // Check if the player is in a hiding spot when crouching state changes
    }

    private void CheckHiding()
    {
        if (isCrouching && playerInHidingSpot)
        {
            hiding = true;
            //hidingText.SetActive(true);
            Debug.Log("Player is now hiding");
        }
        else if (hiding == true)
        {
            hiding = false;
            //hidingText.SetActive(false);
            Debug.Log("Player is no longer hiding");
        }
    }
}