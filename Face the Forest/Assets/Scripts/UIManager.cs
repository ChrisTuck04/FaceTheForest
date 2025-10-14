using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public GameObject deathScreenUI;
    public GameObject tutorialUI;

    private void Start()
    {
        StartCoroutine(ShowTutorialCoroutine());
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

    IEnumerator ShowTutorialCoroutine()
    {
        if (tutorialUI != null)
        {
            tutorialUI.SetActive(true);

            yield return new WaitForSeconds(15f);

            tutorialUI.SetActive(false);
        }
    }
}