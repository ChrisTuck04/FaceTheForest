using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    private ReadableItem itemInRange;

    private void Update()
    {
        if (itemInRange != null && Input.GetKeyDown(KeyCode.E))
        {
            itemInRange.Interact();
            itemInRange = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Player entered an item!");
        if (other.TryGetComponent<ReadableItem>(out var item))
            itemInRange = item;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<ReadableItem>(out var item) && item == itemInRange)
            itemInRange = null;
    }
}
