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

    private UIManager uiManager;
    private UIManagerMain uiManagerMain;
    void Start()
    {
        uiManager = FindFirstObjectByType<UIManager>();
        uiManagerMain = FindFirstObjectByType<UIManagerMain>();

        anim = GetComponentInChildren<Animator>(); //NEW
        agent = GetComponent<NavMeshAgent>();       //NEW

        gameManagerScript = GameManager.instance;

        Debug.Log("4. ENEMY SCRIPT STARTING. TimeScale on load is: " + Time.timeScale);

        counter = 1;
        randDest = Random.Range(0, destinations.Count);
        currentDest = destinations[randDest];

        dest = currentDest.position;
        agent.destination = dest;
        agent.speed = walkSpeed;
        walking = true;
    }

    void Update()
    {
        float speed = agent.velocity.magnitude; //NEW
        anim.SetFloat("Speed", speed); //NEW

        float distance = Vector3.Distance(player.position, agent.transform.position);
        // Player caught, death
        if (distance <= catchDistance)
        {
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
}