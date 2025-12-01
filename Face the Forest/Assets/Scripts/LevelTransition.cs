using UnityEngine;
using UnityEngine.SceneManagement; // If loading a new scene
using System.Collections;

public class LevelTransition : MonoBehaviour
{
    [Header("Settings")]
    public string nextSceneName = "Scene2";
    public float transitionDuration = 3.0f;
    
    // Reference to your Narrator to say the final line
    public NarratorManager narrator; 
    public AudioClip transitionLine; // "Close your eyes..."

    private bool hasTriggered = false;
    private float initialFogDensity;

    void Start()
    {
        // Remember what the fog was so we don't break it later
        if (RenderSettings.fog) initialFogDensity = RenderSettings.fogDensity;
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasTriggered || !other.CompareTag("Player")) return;
        hasTriggered = true;

        StartCoroutine(TransitionRoutine());
    }

    IEnumerator TransitionRoutine()
    {
        // 1. Play Voice Line
        /* if (narrator != null && transitionLine != null) {
            narrator.SilenceNarrator(); // Stop other chatter
            narrator.audioSource.PlayOneShot(transitionLine);
        }
        */

        // 2. Ramp Fog to Maximum (Blind the player)
        float timer = 0f;
        while (timer < transitionDuration)
        {
            timer += Time.deltaTime;
            // Lerp fog from Current to 1.0 (Solid Wall of Fog)
            RenderSettings.fogDensity = Mathf.Lerp(initialFogDensity, 1.0f, timer / transitionDuration);
            yield return null;
        }

        // 3. Load the Next Scene
        // The screen is now fully gray/foggy, so the pop-in won't be seen.
        SceneManager.LoadScene(nextSceneName);
    }
}