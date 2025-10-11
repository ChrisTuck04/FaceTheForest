using UnityEngine;

public class HidePlayer : MonoBehaviour
{
    //public GameObject hidingText;
    //public enemyAI monsterScript;
    public Transform monsterTransform;
    public float loseDistance;
    public int hinderance = 2; // Amount to reduce player speed when in hiding spot

    bool hiding = false;
    bool playerInHidingSpot = false;
    bool isCrouching = false;
    public PlayerMovement pmScript;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInHidingSpot = true;
            pmScript.moveSpeed -= hinderance; // Reduce player speed when in hiding spot
            CheckHiding(); // Check if the player is crouching when entering the hiding spot
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInHidingSpot = false;
            pmScript.moveSpeed += hinderance;
            CheckHiding(); // Update hiding state when leaving the hiding spot
        }
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
        else if(hiding == true)
        {
            hiding = false;
            //hidingText.SetActive(false);
            Debug.Log("Player is no longer hiding");
        }
    }
}
