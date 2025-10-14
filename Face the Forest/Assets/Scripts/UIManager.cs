using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public GameObject deathScreenUI;
    public GameObject tutorialUIOne;
    public GameObject tutorialUITwo;
    public GameObject tutorialUIThree;

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
        if (tutorialUIOne != null && tutorialUITwo != null)
        {
            tutorialUIOne.SetActive(true);

            yield return new WaitForSeconds(5f);

            tutorialUIOne.SetActive(false);
            tutorialUITwo.SetActive(true);

            yield return new WaitForSeconds(7f);

            tutorialUITwo.SetActive(false);
            tutorialUIThree.SetActive(true);

            yield return new WaitForSeconds(5f);

            tutorialUIThree.SetActive(false);
        }
    }
}