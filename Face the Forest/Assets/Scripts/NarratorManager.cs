using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class NarratorManager : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public EnemyAI enemyScript;
    public Transform exitGoal;
    public AudioSource audioSource;

    [Header("Settings")]
    public float talkInterval = 12f;
    public float naturalInterval = 1f;

    [Range(0f, 1f)]
    public float calmSpeakChance = 0.4f;
    [Range(0f, 1f)]
    public float warningChance = 0.5f;
    public float delayAfterWarning = 1.5f;

    [Header("Audio Clips - Calm Guidance")]
    public AudioClip[] calmGoLeft;
    public AudioClip[] calmGoRight;
    public AudioClip[] calmGoForward;
    public AudioClip[] calmGeneral;

    [Header("Audio Clips - Terrified Guidance")]
    public AudioClip[] panicGoLeft;
    public AudioClip[] panicGoRight;
    public AudioClip[] panicGoForward;
    public AudioClip[] panicGeneral;

    [Header("Audio Clips - Tutorial Event")]
    public AudioClip[] tutorialIntros;       // "Watch out! The trees are moving!"
    public AudioClip[] tutorialTrapReaction; // "Wait! The path... it moved!", "We are trapped."
    public AudioClip[] tutorialPathClear;    // "Okay, it's clear now. Move quickly."

    [Header("Audio Clips - Other")]
    public AudioClip[] mazeChange; // "The maze is changing..."

    UnityEngine.AI.NavMeshPath path;
    float calmTimer;
    float naturalTimer;
    bool fearSpoken = false;

    void Start()
    {
        path = new UnityEngine.AI.NavMeshPath();
        calmTimer = talkInterval;
        naturalTimer = naturalInterval;

        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0;

        PlayClip(tutorialIntros);
    }

    void Update()
    {
        calmTimer -= Time.deltaTime;
        naturalTimer -= Time.deltaTime;

        //resets chasing spoken flag, chase has ended, will speak again
        if (!enemyScript.chasing && fearSpoken)
        {
            Debug.Log("Resetting fear spoken flag.");
            fearSpoken = false;
        }

        //enemy has started chasing the player
        if (enemyScript.chasing && !fearSpoken && !audioSource.isPlaying)
        {
            Debug.Log("Enemy chasing player, starting chase sequence.");
            fearSpoken = true;
            StartCoroutine(ChaseSequence());
        }

        else if (calmTimer <= 0 && !audioSource.isPlaying)
        {
            if (!enemyScript.chasing) DecideWhatToSayCalm();
            calmTimer = talkInterval + Random.Range(-2f, 2f);
        }
    }

    IEnumerator ChaseSequence()
    {
        
        if (Random.value < warningChance)
        {
            PlayClip(panicGeneral);

            float waitTime = delayAfterWarning;
            if (audioSource.clip != null)
            {
                waitTime += audioSource.clip.length;
            }

            yield return new WaitForSeconds(waitTime);
        }
        
        PlayDirectionalGuidance(isPanic: true);
    }

    void DecideWhatToSayCalm()
    {
        //random normal guidance
        if (Random.value > calmSpeakChance)
        {
            Debug.Log("Deciding not to speak calmly.");
            return;
        }
        Debug.Log("Deciding calm guidance.");
        //direction guidance, calm
        PlayDirectionalGuidance(isPanic: false);
    }

    public void PlayClip(AudioClip[] clipArray)
    {
        if (clipArray == null || clipArray.Length == 0) return;
        int index = Random.Range(0, clipArray.Length);

        audioSource.clip = clipArray[index];
        audioSource.Play();
    }

    void PlayDirectionalGuidance(bool isPanic)
    {
        Debug.Log("Deciding guidance direction. Panic: " + isPanic);

        // 1. SANITIZE THE POINTS
        // Find the closest point on the NavMesh to the player (within 2.0f units)
        NavMeshHit playerHit;
        if (!NavMesh.SamplePosition(player.position, out playerHit, 2.0f, NavMesh.AllAreas))
        {
            Debug.LogError("Narrator Error: Player is not on the NavMesh!");
            return;
        }

        // Find the closest point on the NavMesh to the Exit (within 2.0f units)
        NavMeshHit exitHit;
        if (!NavMesh.SamplePosition(exitGoal.position, out exitHit, 5.0f, NavMesh.AllAreas))
        {
            Debug.LogError("Narrator Error: Exit Goal is too far from the NavMesh!");
            return;
        }

        // 2. CALCULATE PATH USING THE SNAPPED POINTS
        NavMesh.CalculatePath(playerHit.position, exitHit.position, NavMesh.AllAreas, path);

        // Debug Status
        Debug.Log($"Path Status: {path.status}, Corners: {path.corners.Length}"); 

        if (path.corners.Length < 2) return;

        // 3. DETERMINE DIRECTION
        Vector3 desiredDirection = (path.corners[1] - player.position).normalized;
        float angle = Vector3.SignedAngle(player.forward, desiredDirection, Vector3.up);

        // ... (Rest of your audio selection logic remains the same) ...
        
        if (Random.value > 0.5f && !isPanic)
        {
            PlayClip(calmGeneral);
            return;
        }

        if (angle < -25) PlayClip(isPanic ? panicGoLeft : calmGoLeft);
        else if (angle > 25) PlayClip(isPanic ? panicGoRight : calmGoRight);
        else PlayClip(isPanic ? panicGoForward : calmGoForward);
    }

    public void ReactToMapChange()
    {
        if (enemyScript.chasing || audioSource.isPlaying) return;

        PlayClip(mazeChange);
        calmTimer = talkInterval + Random.Range(-2f, 2f);
    }

    public void SilenceNarrator()
    {
        // 1. Stop the logic loop so no new lines are generated
        StopAllCoroutines();

        // 2. Immediately cut off whatever is currently playing
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        // 3. Optional: Disable this script so Update() stops running
        this.enabled = false;
    }

    public void PlayTutorialTrap()
    {
        // Interrupt any current casual chatter immediately
        SilenceNarrator();
        this.enabled = true; // Re-enable if Silence disabled it
        PlayClip(tutorialTrapReaction);
        calmTimer = talkInterval + Random.Range(-2f, 2f);
    }

    // CALLED BY: TutorialMazeTrigger.cs (Phase 2: Trees Lower)
    public void PlayTutorialClear()
    {
        if (enemyScript.chasing) return; // Priority check
        PlayClip(tutorialPathClear);
    }
}
