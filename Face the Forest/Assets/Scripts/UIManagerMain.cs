using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
public class UIManagerMain : MonoBehaviour
{
    public GameObject deathScreenUI;
    NarratorManager narrator;

    private void Start()
    {
        narrator = FindFirstObjectByType<NarratorManager>();
    }
    public void ShowDeathScreen()
    {
        if (deathScreenUI != null)
        {
            deathScreenUI.SetActive(true);

            if (narrator != null) narrator.SilenceNarrator();

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
