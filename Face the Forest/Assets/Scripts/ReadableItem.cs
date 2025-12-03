using UnityEngine;

public class ReadableItem : MonoBehaviour
{
    [TextArea(5, 15)]
    public string textToRead;

    public void Interact()
    {
        ReadableUI.Instance.Show(textToRead);
        gameObject.SetActive(false);
    }
}
