using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [Header("Sensitivity")]
    public Slider sensitivitySlider;

    [Header("Volume")]
    public Slider volumeSlider;

    [Header("Button Audio")]
    private AudioSource buttonAudioSource;
    private AudioClip buttonClickClip;
    
    [Header("Player Camera")]
    public PlayerCam playerCam;

    private const string SensitivityKey = "mouse_sensitivity";
    private const string VolumeKey = "master_volume";

    private void Start()
    {
        // Auto-find PlayerCam if not assigned
        if (playerCam == null)
        {
            playerCam = FindObjectOfType<PlayerCam>();
            if (playerCam == null)
            {
                Debug.LogError("PlayerCam script not found in scene!");
            }
            else
            {
                Debug.Log("PlayerCam automatically found and assigned!");
            }
        }

        SetupButtonAudio();
        LoadSettings();

        // Listen for when user finishes dragging sliders (on pointer up)
        if (sensitivitySlider != null)
        {
            sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
            AddSliderEndDragEvent(sensitivitySlider);
        }
        
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
            AddSliderEndDragEvent(volumeSlider);
        }
    }

    private void SetupButtonAudio()
    {
        // Create AudioSource for button sounds
        buttonAudioSource = gameObject.AddComponent<AudioSource>();
        buttonAudioSource.playOnAwake = false;
        buttonAudioSource.volume = 0.7f;

        // Load button click sound
        buttonClickClip = Resources.Load<AudioClip>("Audio/Monster/Menu/Menu_Buttons_1");
        
        if (buttonClickClip != null)
        {
            Debug.Log("Button click sound loaded successfully!");
        }
        else
        {
            Debug.LogError("Button click sound not found at Audio/Monster/Menu/Menu_Buttons_1");
        }
    }

    private void AddSliderEndDragEvent(Slider slider)
    {
        // Add EventTrigger component if it doesn't exist
        UnityEngine.EventSystems.EventTrigger trigger = slider.gameObject.GetComponent<UnityEngine.EventSystems.EventTrigger>();
        if (trigger == null)
        {
            trigger = slider.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
        }

        // Create PointerUp event (when user releases the slider)
        UnityEngine.EventSystems.EventTrigger.Entry entry = new UnityEngine.EventSystems.EventTrigger.Entry();
        entry.eventID = UnityEngine.EventSystems.EventTriggerType.PointerUp;
        entry.callback.AddListener((data) => { OnSliderDragEnd(); });
        trigger.triggers.Add(entry);
    }

    private void OnSliderDragEnd()
    {
        PlayButtonSound();
    }

    private void PlayButtonSound()
    {
        if (buttonAudioSource != null && buttonClickClip != null)
        {
            buttonAudioSource.PlayOneShot(buttonClickClip);
        }
    }

    private void OnSensitivityChanged(float value)
    {
        PlayerPrefs.SetFloat(SensitivityKey, value);
        PlayerPrefs.Save();

        if (playerCam != null)
        {
            playerCam.sensX = value;
            playerCam.sensY = value;
        }
    }

    private void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
        
        // Also update ForestAmbience if it exists
        ForestAmbience forestAmbience = FindObjectOfType<ForestAmbience>();
        if (forestAmbience != null)
        {
            forestAmbience.SetVolume(value);
        }
        
        PlayerPrefs.SetFloat(VolumeKey, value);
        PlayerPrefs.Save();
    }

    public void LoadSettings()
    {
        float sensitivity = PlayerPrefs.GetFloat(SensitivityKey, 100f);
        float volume = PlayerPrefs.GetFloat(VolumeKey, 1.0f);

        if (sensitivitySlider != null)
        {
            sensitivitySlider.value = sensitivity;
        }
        else
        {
            Debug.LogError("Sensitivity Slider is not assigned in SettingsManager!");
        }

        if (volumeSlider != null)
        {
            volumeSlider.value = volume;
        }
        else
        {
            Debug.LogError("Volume Slider is not assigned in SettingsManager!");
        }

        if (playerCam != null)
        {
            playerCam.sensX = sensitivity;
            playerCam.sensY = sensitivity;
        }
        else
        {
            Debug.LogError("PlayerCam is not assigned in SettingsManager!");
        }

        AudioListener.volume = volume;
    }

    // Call this method from Resume and Settings buttons' OnClick event in the Inspector
    public void OnButtonClick()
    {
        PlayButtonSound();
    }
}