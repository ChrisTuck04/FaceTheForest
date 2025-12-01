using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    private InputSystem_Actions controls;

    public GameObject pausePanel;
    public GameObject settingsPanel;
    public bool IsPaused { get; private set; }

    private void Start()
    {
        Resume();
    }

    private void Awake()
    {
        controls = new InputSystem_Actions();
        controls.Enable();

    }

    private void OnEnable()
    {
        controls.UI.Pause.started += OnPause;
        controls.UI.Enable();
    }

    private void OnDisable()
    {
        controls.UI.Pause.started -= OnPause;
        controls.UI.Disable();
    }

    private void OnPause(InputAction.CallbackContext ctx)
    {
        TogglePause();
    }

    public void TogglePause()
    {
        if (IsPaused) Resume();
        else Pause();
    }

    public void Pause()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        IsPaused = true;
    }

    public void Resume()
    {
        CloseSettings();
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        IsPaused = false;
    }

    public void OpenSettings()
    {
        pausePanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        pausePanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

}
