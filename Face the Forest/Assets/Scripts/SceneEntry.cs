using UnityEngine;
using System.Collections;

public class SceneEntry : MonoBehaviour
{
    [Header("Fog Settings")]
    public float fadeDuration = 4.0f;
    public float targetFogDensity = 0.15f;
    
    [Header("Narrator Settings")]
    public NarratorManager narrator;
    public AudioClip[] entryLine;

    void Start()
    {
        RenderSettings.fog = true;
        RenderSettings.fogDensity = 1.0f; 

        StartCoroutine(FadeInRoutine());
    }

    IEnumerator FadeInRoutine()
    {
        if (narrator != null && entryLine != null)
        {
            narrator.SilenceNarrator();
            narrator.PlayClip(entryLine);
        }

        float timer = 0f;
        float startDensity = 1.0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            
            RenderSettings.fogDensity = Mathf.Lerp(startDensity, targetFogDensity, timer / fadeDuration);
            
            yield return null;
        }

        RenderSettings.fogDensity = targetFogDensity;
    }
}