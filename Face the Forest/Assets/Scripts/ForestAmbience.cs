using UnityEngine;

public class ForestAmbience : MonoBehaviour
{
    [Header("Ambience Layers")]
    [SerializeField] private AudioClip[] ambienceClips;
    [SerializeField] private float[] clipVolumes = { 1f, 0.3f }; // Night loop at 1.0, backdrop at 0.3
    
    [Header("Settings")]
    [SerializeField] private float minDelay = 2f;
    [SerializeField] private float maxDelay = 8f;
    
    private AudioSource[] audioSources;
    private float masterVolume = 1f;
    
    private void Start()
    {
        LoadAmbienceClips();
        SetupAudioSources();
        
        // Load saved volume
        masterVolume = PlayerPrefs.GetFloat("master_volume", 1.0f);
        ApplyVolume();
        
        PlayAllLayers();
    }
    
    private void LoadAmbienceClips()
    {
        ambienceClips = new AudioClip[] 
        {
            Resources.Load<AudioClip>("Audio/Ambience/Environment/Nature/Ambiance_Night_Loop_Stereo"),
            Resources.Load<AudioClip>("Audio/Ambience/Backdrop/backdrop")
        };
        Debug.Log($"Loaded {ambienceClips.Length} ambience clips");
    }
    
    private void SetupAudioSources()
    {
        audioSources = new AudioSource[ambienceClips.Length];
        
        for (int i = 0; i < ambienceClips.Length; i++)
        {
            audioSources[i] = gameObject.AddComponent<AudioSource>();
            audioSources[i].clip = ambienceClips[i];
            audioSources[i].loop = true;
            audioSources[i].playOnAwake = false;
            audioSources[i].spatialBlend = 0f;
            audioSources[i].volume = clipVolumes[i] * masterVolume;
        }
    }
    
    private void PlayAllLayers()
    {
        for (int i = 0; i < audioSources.Length; i++)
        {
            float delay = Random.Range(minDelay, maxDelay);
            audioSources[i].PlayDelayed(delay);
        }
    }
    
    // Called by SettingsManager when volume changes
    public void SetVolume(float volume)
    {
        masterVolume = volume;
        ApplyVolume();
    }
    
    private void ApplyVolume()
    {
        if (audioSources == null) return;
        
        for (int i = 0; i < audioSources.Length; i++)
        {
            if (audioSources[i] != null)
            {
                audioSources[i].volume = clipVolumes[i] * masterVolume;
            }
        }
    }
}