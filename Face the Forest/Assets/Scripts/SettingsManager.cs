using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [Header("Sensitivity")]
    public Slider sensitivitySlider;

    [Header("Volume")]
    public Slider volumeSlider;

    public PlayerCam playerCam;

    private const string SensitivityKey = "mouse_sensitivity";
    private const string VolumeKey = "master_volume";

    private void Start()
    {
        LoadSettings();

        sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    private void OnSensitivityChanged(float value)
    {
        PlayerPrefs.SetFloat(SensitivityKey, value);
        PlayerPrefs.Save();

        playerCam.sensX = value;
        playerCam.sensY = value;
    }

    private void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat(VolumeKey, value);
        PlayerPrefs.Save();
    }

    public void LoadSettings()
    {
        float sensitivity = PlayerPrefs.GetFloat(SensitivityKey, 100f);
        float volume = PlayerPrefs.GetFloat(VolumeKey, 1.0f);

        sensitivitySlider.value = sensitivity;
        volumeSlider.value = volume;

        sensitivitySlider.value = sensitivity;
        volumeSlider.value = volume;

        playerCam.sensX = sensitivity;
        playerCam.sensY = sensitivity;

        AudioListener.volume = volume;
    }
}
