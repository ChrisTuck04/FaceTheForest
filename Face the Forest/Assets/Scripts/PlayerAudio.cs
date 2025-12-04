using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerAudio : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private float walkStepInterval = 0.5f;
    [SerializeField] private float runStepInterval = 0.3f;
    [SerializeField] private float volumeVariation = 0.1f;
    [SerializeField] private float pitchVariation = 0.1f;
    [SerializeField] private float footstepVolume = 0.7f;
    [SerializeField] private float jumpVolume = 0.6f;
    [SerializeField] private float landVolume = 0.8f;
    
    [Header("Voice Settings")]
    [SerializeField] private float basePitch = 1.5f; // Higher pitch for child voice (1.5-2.0 range)

    private AudioSource audioSource;
    private AudioClip[] walkingSounds;
    private AudioClip[] runningSounds;
    private AudioClip[] jumpSounds;
    private AudioClip[] landSounds;
    private float stepTimer;
    private bool wasGrounded;
    
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 0f;
        
        LoadAudioFiles();
    }

    private void LoadAudioFiles()
    {
        walkingSounds = Resources.LoadAll<AudioClip>("Audio/Player/Movement/Steps/Single");
        runningSounds = Resources.LoadAll<AudioClip>("Audio/Player/Movement/Steps/Single");
        jumpSounds = Resources.LoadAll<AudioClip>("Audio/Player/Movement/Jump");
        landSounds = Resources.LoadAll<AudioClip>("Audio/Player/Movement/Jump/Land");

        Debug.Log($"Loaded {walkingSounds.Length} walking sounds");
        Debug.Log($"Loaded {runningSounds.Length} running sounds");
        Debug.Log($"Loaded {jumpSounds.Length} jump sounds");
        Debug.Log($"Loaded {landSounds.Length} land sounds");
    }

    private void Start()
    {
        stepTimer = 0f;
        wasGrounded = true;
    }

    public void PlayFootsteps(bool isMoving, bool isSprinting, bool isGrounded, bool isCrouching)
    {
        if (!isGrounded || !isMoving || isCrouching)
        {
            stepTimer = 0f;
            return;
        }

        float currentInterval = isSprinting ? runStepInterval : walkStepInterval;
        stepTimer += Time.deltaTime;

        if (stepTimer >= currentInterval)
        {
            PlayRandomFootstep(isSprinting);
            stepTimer = 0f;
        }
    }

    private void PlayRandomFootstep(bool isSprinting)
    {
        AudioClip[] soundArray = isSprinting ? runningSounds : walkingSounds;
        
        if (soundArray == null || soundArray.Length == 0)
            return;

        AudioClip clip = soundArray[Random.Range(0, soundArray.Length)];
        PlaySound(clip, footstepVolume);
    }

    public void PlayJumpSound()
    {
        if (jumpSounds == null || jumpSounds.Length == 0)
            return;

        AudioClip clip = jumpSounds[Random.Range(0, jumpSounds.Length)];
        PlaySound(clip, jumpVolume);
    }

    public void CheckLanding(bool isGrounded)
    {
        if (isGrounded && !wasGrounded)
        {
            PlayLandSound();
        }
        
        wasGrounded = isGrounded;
    }

    private void PlayLandSound()
    {
        if (landSounds == null || landSounds.Length == 0)
            return;

        AudioClip clip = landSounds[Random.Range(0, landSounds.Length)];
        PlaySound(clip, landVolume);
    }

    private void PlaySound(AudioClip clip, float volume)
    {
        if (clip == null)
            return;

        float randomVolume = volume + Random.Range(-volumeVariation, volumeVariation);
        // Apply base pitch (for child voice) plus random variation
        float randomPitch = basePitch + Random.Range(-pitchVariation, pitchVariation);

        audioSource.pitch = randomPitch;
        audioSource.PlayOneShot(clip, Mathf.Clamp01(randomVolume));
    }

    public void StopAllAudio()
    {
        audioSource.Stop();
        stepTimer = 0f;
    }
}