using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIManagerMain : MonoBehaviour
{
    public GameObject deathScreenUI;
    
    [Header("Death Screen Audio")]
    private AudioSource deathMusicSource;
    private AudioClip deathMusicClip;
    private AudioSource buttonAudioSource;
    private AudioClip buttonClickClip;

    private void Start()
    {
        SetupDeathAudio();
    }

    private void SetupDeathAudio()
    {
        // Setup death music audio source
        GameObject deathMusicObject = new GameObject("DeathMusic");
        deathMusicObject.transform.SetParent(transform);
        deathMusicSource = deathMusicObject.AddComponent<AudioSource>();
        deathMusicSource.playOnAwake = false;
        deathMusicSource.loop = true;
        deathMusicSource.volume = 0.6f;

        // Load death music
        deathMusicClip = Resources.Load<AudioClip>("Audio/Monster/MX/Ambience/CoF");
        
        if (deathMusicClip != null)
        {
            Debug.Log("Death music loaded successfully!");
        }
        else
        {
            Debug.LogError("Death music not found! Update the path in SetupDeathAudio()");
        }

        // Setup button click audio source
        buttonAudioSource = gameObject.AddComponent<AudioSource>();
        buttonAudioSource.playOnAwake = false;
        buttonAudioSource.volume = 0.7f;

        buttonClickClip = Resources.Load<AudioClip>("Audio/Monster/Menu/Menu_Buttons_1");
        
        if (buttonClickClip == null)
        {
            Debug.LogError("Button click sound not found!");
        }
    }

    public void ShowDeathScreen()
    {
        if (deathScreenUI != null)
        {
            deathScreenUI.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Play death music
            if (deathMusicSource != null && deathMusicClip != null)
            {
                deathMusicSource.Play();
                Debug.Log("Death music started!");
            }
        }
    }

    public void RestartLevel()
    {
        // Play button click sound
        if (buttonAudioSource != null && buttonClickClip != null)
        {
            buttonAudioSource.PlayOneShot(buttonClickClip);
        }

        // Stop death music
        if (deathMusicSource != null && deathMusicSource.isPlaying)
        {
            deathMusicSource.Stop();
        }

        deathScreenUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnDestroy()
    {
        // Stop death music when destroyed
        if (deathMusicSource != null && deathMusicSource.isPlaying)
        {
            deathMusicSource.Stop();
        }
    }
}