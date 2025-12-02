using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyAI : MonoBehaviour
{
    public GameManager gameManagerScript;
    
    public Transform target;
    public List<Transform> destinations;
    //public Animator anim;
    public float walkSpeed, chaseSpeed, minIdleTime, maxIdleTime;
    float idleTime, distance;
    public bool walking, chasing, hunting, pathing;
    public int hintCount, catchDistance, deathAnimTime;
    public Transform player;
    Transform currentDest;
    Vector3 dest;
    int randDest, counter;
    public NavMeshAgent agent;
    Animator anim; //NEW

    [Header("AI Vision")]
    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;
    public LayerMask playerMask;
    public LayerMask obstacleMask;

    [Header("Chase Music")]
    public float chaseMusicStartDistance = 20f;
    public float chaseMusicStopDistance = 30f;
    private AudioSource chaseAudioSource;
    private bool chaseMusicPlaying = false;

    [Header("Monster Audio")]
    public float footstepInterval = 0.5f; // Time between footstep sounds
    public float voiceInterval = 5f; // Time between voice sounds
    public float voiceMinInterval = 3f; // Minimum time between voices
    public float voiceMaxInterval = 8f; // Maximum time between voices
    
    private AudioSource footstepAudioSource;
    private AudioSource voiceAudioSource;
    private AudioSource screamAudioSource;
    private float footstepTimer = 0f;
    private float voiceTimer = 0f;
    private int currentVoiceIndex = 0;
    private AudioClip[] voiceClips;
    private AudioClip footstepClip;
    private AudioClip screamClip;
    private bool isDead = false;
    [Header("Attack Settings")]
    public float attackDistance = 3.5f;
    public float attackCooldown = 2f;
    float lastAttackTime = 0f;


    private UIManager uiManager;
    private UIManagerMain uiManagerMain;
    
    void Start()
    {
        uiManager = FindFirstObjectByType<UIManager>();
        uiManagerMain = FindFirstObjectByType<UIManagerMain>();

        anim = GetComponentInChildren<Animator>(); 
        agent = GetComponent<NavMeshAgent>();      

        gameManagerScript = GameManager.instance;

        Debug.Log("4. ENEMY SCRIPT STARTING. TimeScale on load is: " + Time.timeScale);

        counter = 1;
        randDest = Random.Range(0, destinations.Count);
        currentDest = destinations[randDest];

        dest = currentDest.position;
        agent.destination = dest;
        agent.speed = walkSpeed;
        walking = true;
        isDead = false;

        // Setup all audio
        SetupChaseMusic();
        SetupMonsterAudio();
        
        // Set initial random voice interval
        voiceTimer = Random.Range(voiceMinInterval, voiceMaxInterval);
    }

    void SetupChaseMusic()
    {
        GameObject audioObject = new GameObject("ChaseMusic");
        audioObject.transform.SetParent(transform);
        audioObject.transform.localPosition = Vector3.zero;
        
        chaseAudioSource = audioObject.AddComponent<AudioSource>();
        
        AudioClip chaseClip = Resources.Load<AudioClip>("Audio/Music/Chase/chase");
        
        if (chaseClip != null)
        {
            chaseAudioSource.clip = chaseClip;
            chaseAudioSource.loop = true;
            chaseAudioSource.playOnAwake = false;
            chaseAudioSource.volume = 0.7f;
            Debug.Log("Chase music loaded successfully!");
        }
        else
        {
            Debug.LogError("Chase music not found! Make sure chase.mp3 is in Resources/Audio/Music/Chase/");
        }
    }

    void SetupMonsterAudio()
    {
        // Setup footstep audio
        GameObject footstepObject = new GameObject("FootstepAudio");
        footstepObject.transform.SetParent(transform);
        footstepObject.transform.localPosition = Vector3.zero;
        footstepAudioSource = footstepObject.AddComponent<AudioSource>();
        footstepAudioSource.playOnAwake = false;
        footstepAudioSource.volume = 0.6f;
        footstepAudioSource.spatialBlend = 1f; // 3D sound
        footstepAudioSource.maxDistance = 30f;
        
        footstepClip = Resources.Load<AudioClip>("Audio/Monster/Enemies/Monster/Steps/Monster_Steps_1");
        if (footstepClip != null)
        {
            Debug.Log("Monster footstep audio loaded successfully!");
        }
        else
        {
            Debug.LogError("Monster footstep audio not found!");
        }

        // Setup voice audio
        GameObject voiceObject = new GameObject("VoiceAudio");
        voiceObject.transform.SetParent(transform);
        voiceObject.transform.localPosition = Vector3.zero;
        voiceAudioSource = voiceObject.AddComponent<AudioSource>();
        voiceAudioSource.playOnAwake = false;
        voiceAudioSource.volume = 0.8f;
        voiceAudioSource.spatialBlend = 1f; // 3D sound
        voiceAudioSource.maxDistance = 40f;
        
        // Load all 5 voice clips
        voiceClips = new AudioClip[5];
        for (int i = 0; i < 5; i++)
        {
            voiceClips[i] = Resources.Load<AudioClip>($"Audio/Monster/Enemies/Monster/Voice/Monster_Attack_{i + 1}");
            if (voiceClips[i] != null)
            {
                Debug.Log($"Monster voice {i + 1} loaded successfully!");
            }
            else
            {
                Debug.LogError($"Monster voice {i + 1} not found!");
            }
        }

        // Setup scream audio
        GameObject screamObject = new GameObject("ScreamAudio");
        screamObject.transform.SetParent(transform);
        screamObject.transform.localPosition = Vector3.zero;
        screamAudioSource = screamObject.AddComponent<AudioSource>();
        screamAudioSource.playOnAwake = false;
        screamAudioSource.volume = 1f;
        screamAudioSource.spatialBlend = 0.5f; // Mix of 2D and 3D
        
        screamClip = Resources.Load<AudioClip>("Audio/Monster/Enemies/Monster/Voice/Monster_LongScream");
        if (screamClip != null)
        {
            Debug.Log("Monster scream audio loaded successfully!");
        }
        else
        {
            Debug.LogError("Monster scream audio not found!");
        }
    }

    void Update()
    {
        if (isDead) return; // Don't do anything if player is dead
        
        float distance = Vector3.Distance(player.position, agent.transform.position);
        
        // Handle chase music based on distance and chasing state
        HandleChaseMusic(distance);
        
        // Handle monster audio
        HandleMonsterAudio();
        
        float speed = agent.velocity.magnitude; 
        anim.SetFloat("Speed", speed); 

        anim.SetBool("isChasing", chasing);

        if (chasing && distance <= attackDistance && Time.time > lastAttackTime + attackCooldown)
        {
            agent.isStopped = true;
            agent.updateRotation = false;
            anim.SetTrigger("Attack");
            lastAttackTime = Time.time;

            StartCoroutine(ResumeAfterAttack());
        }

        // Player caught, death
        if (distance <= catchDistance)
        {
            isDead = true;
            
            // Stop all ongoing audio immediately
            StopAllAudio();
            
            // Play scream sound
            if (screamAudioSource != null && screamClip != null)
            {
                screamAudioSource.PlayOneShot(screamClip);
            }
            
            if (uiManager != null)
            {
                uiManager.ShowDeathScreen();
            }

            if (uiManagerMain != null)
            {
                uiManagerMain.ShowDeathScreen();
            }
            //anim.ResetTrigger("walk");
            //anim.ResetTrigger("idle");
            //anim.ResetTrigger("sprint");
            //anim.SetTrigger("jumpscare");
            PlayerMovement playerScript = player.GetComponent<PlayerMovement>();
            playerScript?.Die();
        
            StartCoroutine(DeathRoutine());
            return; // Stop processing update
        }

        if (!chasing)
        {
            if (CanSeePlayer() && !gameManagerScript.hiding)
            {
                chasing = true;
                walking = false;
                pathing = false;
                hunting = false;
                Debug.Log("Player spotted, starting chase!");
            }
        }

        if (chasing)
        {
            dest = player.position;
            agent.destination = dest;
            agent.speed = chaseSpeed;
            //anim.ResetTrigger("walk");
            //anim.ResetTrigger("idle");
            //anim.SetTrigger("sprint");
            if (gameManagerScript != null && gameManagerScript.hiding && distance <= 8)
            {
                chasing = false;
                pathing = true;
                Debug.Log("Player is hiding, stopping chase.");
            }
        }
        // Hint was given, head near player
        else if (hunting)
        {
            hunting = false;
            dest = player.position;
            Debug.Log("Hunting near: " + dest);

            NavMeshPath path = new NavMeshPath();
            Transform closestDestination = null;
            float shortestPathLength = Mathf.Infinity;

            // Loop through all patrol points to find the one closest to the player's last position.
            foreach (Transform destinationPoint in destinations)
            {
                if (destinationPoint == null) continue;
                if (NavMesh.CalculatePath(dest, destinationPoint.position, NavMesh.AllAreas, path))
                {
                    // If a path is successfully found, calculate its total length.
                    float pathLength = 0f;

                    // A path is a series of corner points. We sum the distance between each corner.
                    for (int i = 0; i < path.corners.Length - 1; i++)
                    {
                        pathLength += Vector3.Distance(path.corners[i], path.corners[i + 1]);
                    }
                    // If this path is the shortest one we've found so far, save it.
                    if (pathLength < shortestPathLength)
                    {
                        shortestPathLength = pathLength;
                        closestDestination = destinationPoint;
                    }
                }
            }
            Debug.Log("New Hunting Point: " + closestDestination.name);
            currentDest = closestDestination;
            dest = currentDest.position;
            agent.destination = dest;
            agent.speed = walkSpeed;
            walking = true;
            //anim.ResetTrigger("walk");
            //anim.ResetTrigger("idle");
            //anim.SetTrigger("sprint");
        }

        else if (pathing)
        {
            pathing = false;
            randDest = Random.Range(0, destinations.Count);
            Debug.Log("New Destination Index: " + randDest);
            currentDest = destinations[randDest];
            dest = currentDest.position;
            agent.destination = dest;
            agent.speed = walkSpeed;
            walking = true;
        }
        else if (walking)
        {
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                Debug.Log("--- AI ARRIVED, Starting StayIdle Coroutine ---");
                walking = false;
                StartCoroutine(StayIdle());
            }
        }

        
    }

    void HandleMonsterAudio()
    {
        if (isDead) return; // Don't play audio if player is dead
        
        // Play footsteps when moving
        if (agent.velocity.magnitude > 0.1f)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0f)
            {
                if (footstepAudioSource != null && footstepClip != null)
                {
                    footstepAudioSource.PlayOneShot(footstepClip);
                }
                footstepTimer = footstepInterval;
            }
        }
        
        // Play voice sounds periodically
        voiceTimer -= Time.deltaTime;
        if (voiceTimer <= 0f)
        {
            if (voiceAudioSource != null && voiceClips != null && voiceClips.Length > 0)
            {
                // Play current voice clip
                if (voiceClips[currentVoiceIndex] != null)
                {
                    voiceAudioSource.PlayOneShot(voiceClips[currentVoiceIndex]);
                }
                
                // Move to next voice clip (rotate through 1-5)
                currentVoiceIndex = (currentVoiceIndex + 1) % 5;
            }
            
            // Set next random interval
            voiceTimer = Random.Range(voiceMinInterval, voiceMaxInterval);
        }
    }

    void HandleChaseMusic(float distance)
    {
        if (chaseAudioSource == null || isDead) return;

        // Only play chase music when actually chasing
        if (chasing && distance <= chaseMusicStartDistance && !chaseMusicPlaying)
        {
            chaseAudioSource.Play();
            chaseMusicPlaying = true;
            Debug.Log("Chase music started! Distance: " + distance);
        }
        // Stop music when not chasing or too far away
        else if ((!chasing || distance > chaseMusicStopDistance) && chaseMusicPlaying)
        {
            chaseAudioSource.Stop();
            chaseMusicPlaying = false;
            Debug.Log("Chase music stopped! Distance: " + distance);
        }
    }

    void StopAllAudio()
    {
        // Stop chase music
        if (chaseAudioSource != null && chaseAudioSource.isPlaying)
        {
            chaseAudioSource.Stop();
            chaseMusicPlaying = false;
        }
        
        // Stop footsteps
        if (footstepAudioSource != null && footstepAudioSource.isPlaying)
        {
            footstepAudioSource.Stop();
        }
        
        // Stop voice
        if (voiceAudioSource != null && voiceAudioSource.isPlaying)
        {
            voiceAudioSource.Stop();
        }
    }

    IEnumerator StayIdle()
    {
        Debug.Log("StayIdle: Coroutine has started.");
        idleTime = Random.Range(minIdleTime, maxIdleTime);
        Debug.Log("StayIdle: Calculated idleTime is: " + idleTime + " seconds. Waiting now...");

        yield return new WaitForSeconds(idleTime);
        Debug.Log("StayIdle: Checking condition... (Counter is " + counter + ", HintCount is " + hintCount + ")");
        if (counter < hintCount)
        {
            Debug.Log("StayIdle: RESULT = PATROLLING");
            counter++;
            pathing = true;
        }
        else
        {
            Debug.Log("StayIdle: RESULT = HUNTING");
            counter = 0;
            Debug.Log("Hunting player! Counter: " + counter);
            hunting = true;
        }
    }
    
    IEnumerator DeathRoutine()
    {
        Debug.Log("Player caught! Starting death sequence...");

        yield return new WaitForSeconds(deathAnimTime);
        //SceneManager.LoadScene(deathScene);
    }

    void OnDisable()
    {
        // Stop all audio when the script is disabled
        StopAllAudio();
        if (screamAudioSource != null && screamAudioSource.isPlaying)
        {
            screamAudioSource.Stop();
        }
    }

    void OnDestroy()
    {
        // Stop all audio when the object is destroyed
        StopAllAudio();
        if (screamAudioSource != null && screamAudioSource.isPlaying)
        {
            screamAudioSource.Stop();
        }
    }
    
    private bool CanSeePlayer()
    {
        // Check if the player is even within the view radius.
        if (Vector3.Distance(transform.position, player.position) > viewRadius)
        {
            return false;
        }

        // Get the direction vector from the enemy to the player.
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        // Check if this direction is within the enemy's view angle.
        if (Vector3.Angle(transform.forward, directionToPlayer) < viewAngle / 2)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // Check if there are any obstacles blocking the line of sight to the player.
            if (!Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleMask))
            {
                return true; // Line of sight is clear.
            }
        }

        return false;
    }

    IEnumerator ResumeAfterAttack()
    {
        yield return new WaitForSeconds(1.0f);

        agent.isStopped = false;
        agent.updateRotation = true;
    } 


}