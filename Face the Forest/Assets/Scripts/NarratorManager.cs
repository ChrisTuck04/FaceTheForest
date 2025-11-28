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
    public AudioClip[] panicRun;
    public AudioClip[] monsterBehind;

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
    }

    void Update()
    {
        calmTimer -= Time.deltaTime;
        naturalTimer -= Time.deltaTime;

        //resets chasing spoken flag, chase has ended, will speak again
        if (!enemyScript.chasing && fearSpoken)
        {
            fearSpoken = false;
        }

        //enemy has started chasing the player
        if (enemyScript.chasing && !fearSpoken && !audioSource.isPlaying)
        {
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
        bool isBehind = CheckIfMonsterBehind();

        //behind player warning chance
        if (isBehind && Random.value < warningChance)
        {
            PlayClip(monsterBehind);

            float waitTime = delayAfterWarning;
            if (audioSource.clip != null)
            {
                waitTime += audioSource.clip.length;
            }

            yield return new WaitForSeconds(waitTime);
        }
        else if (!isBehind && Random.value < warningChance)
        {
            PlayClip(panicRun);

            float waitTime = delayAfterWarning;
            if (audioSource.clip != null)
            {
                waitTime += audioSource.clip.length;
            }

            yield return new WaitForSeconds(waitTime);
        }

        //direction guidance, panic
        PlayDirectionalGuidance(isPanic: true);
    }

    void DecideWhatToSayCalm()
    {
        //random normal guidance
        if (Random.value > calmSpeakChance)
        {
            return;
        }

        //direction guidance, calm
        PlayDirectionalGuidance(isPanic: false);
    }

    bool CheckIfMonsterBehind()
    {
        if (enemyScript == null) return false;
        Vector3 relativePos = player.InverseTransformPoint(enemyScript.transform.position);
        return relativePos.z < 0;
    }

    void PlayClip(AudioClip[] clipArray)
    {
        if (clipArray == null || clipArray.Length == 0) return;
        int index = Random.Range(0, clipArray.Length);

        audioSource.clip = clipArray[index];
        audioSource.Play();
    }

    void PlayDirectionalGuidance(bool isPanic)
    {
        NavMesh.CalculatePath(player.position, exitGoal.position, NavMesh.AllAreas, path);
        if (path.corners.Length < 2) return;

        //find direction to go
        Vector3 desiredDirection = (path.corners[1] - player.position).normalized;
        float angle = Vector3.SignedAngle(player.forward, desiredDirection, Vector3.up);

        if (Random.value > 0.5f && !isPanic)
        {
            PlayClip(calmGeneral);
            return;
        }

        if (angle < -25) //left
        {
            PlayClip(isPanic ? panicGoLeft : calmGoLeft);
        }
        else if (angle > 25) //right
        {
            PlayClip(isPanic ? panicGoRight : calmGoRight);
        }
        else //forward
        {
            PlayClip(isPanic ? panicGoForward : calmGoForward);
        }
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
}
