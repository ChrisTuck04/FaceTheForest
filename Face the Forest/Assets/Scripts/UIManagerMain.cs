using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
public class UIManagerMain : MonoBehaviour
{
    public GameObject deathScreenUI;

    private void Start()
    {
    }
    public void ShowDeathScreen()
    {
        if (deathScreenUI != null)
        {
            deathScreenUI.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    public void RestartLevel()
    {
        deathScreenUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
