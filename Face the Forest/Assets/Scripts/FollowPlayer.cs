using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform cameraPosition; // Reference to the player's transform

    // Update is called once per frame
    void Update()
    {
        
        transform.position = cameraPosition.position; // Update camera position to follow the player with the specified offset
    }
}
