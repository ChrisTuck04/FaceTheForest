using UnityEngine;
using TMPro;

public class ReadableUI : MonoBehaviour
{
    public static ReadableUI Instance;

    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject button;
    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject playerCam;
    [SerializeField] private TMP_Text contentText;

    private PlayerMovement playerMovement;
    private PlayerCam playerCamScript;

    private void Awake()
    {
        Instance = this;
        playerMovement = Player.GetComponent<PlayerMovement>();
        playerCamScript = playerCam.GetComponent<PlayerCam>();

        panel.SetActive(false);
        button.SetActive(false);
    }

    public void Show(string text)
    {
        contentText.text = text;
        panel.SetActive(true);
        button.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        disablePlayerActivity();


    }

    public void Hide()
    {
        panel.SetActive(false);
        button.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        enablePlayerActivity();


    }

    private void disablePlayerActivity()
    {
        playerMovement.enabled = false;
        playerCamScript.enabled = false;
    }

    private void enablePlayerActivity()
    {
        playerMovement.enabled = true;
        playerCamScript.enabled = true;
    }
}
