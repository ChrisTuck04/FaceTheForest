using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player; // Reference to the player's transform

    // Update is called once per frame
    void Update()
    {
        
        transform.position = player.position; // Update camera position to follow the player with the specified offset
    }
}
