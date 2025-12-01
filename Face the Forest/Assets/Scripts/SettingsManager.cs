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

    private const string SensitivityKey = "mouse_sensitivity";
    private const string VolumeKey = "master_volume";

    private void Start()
    {
        SetupButtonAudio();
        LoadSettings();

        // Listen for when user finishes dragging sliders (on pointer up)
        sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        
        // Add event triggers for when dragging ends
        AddSliderEndDragEvent(sensitivitySlider);
        AddSliderEndDragEvent(volumeSlider);
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
    }

    private void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat(VolumeKey, value);
        PlayerPrefs.Save();
    }

    public void LoadSettings()
    {
        float sensitivity = PlayerPrefs.GetFloat(SensitivityKey, 1.0f);
        float volume = PlayerPrefs.GetFloat(VolumeKey, 1.0f);

        sensitivitySlider.value = sensitivity;
        volumeSlider.value = volume;

        AudioListener.volume = volume;
    }

    // Call this method from Resume and Settings buttons' OnClick event in the Inspector
    public void OnButtonClick()
    {
        PlayButtonSound();
    }
}