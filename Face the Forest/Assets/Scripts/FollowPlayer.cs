using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform cameraPosition; // Reference to the player's transform

    // Update is called once per frame
    void Update()
    {
        Vector3 offset = new Vector3(0, 0, 0);
        if (isCrouching)
        {
            offset.y -= 0.5f; // Adjust the offset value as needed
        }

        transform.position = cameraPosition.position + offset; // Update camera position to follow the player with the specified offset
    }

    public void SetCrouching()
    {
        isCrouching = !isCrouching;
    }
}
