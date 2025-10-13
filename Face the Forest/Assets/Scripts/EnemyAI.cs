using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform target;
    public List<Transform> destinations;
    //public Animator anim;
    public float walkSpeed, chaseSpeed, minIdleTime, maxIdleTime;
    float idleTime;
    public bool walking, chasing;
    public Transform player;
    Transform currentDest;
    Vector3 dest;
    int randDest, randDecision, destRandCount;
    public int destinationAmount;
    public NavMeshAgent agent;

    void Start()
    {
        destRandCount = destinationAmount + 1;
        walking = true;
        randDest = Random.Range(0, destRandCount);
        currentDest = destinations[randDest];
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (walking == true)
        {
            dest = currentDest.position;
            agent.destination = dest;
            agent.speed = walkSpeed;
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                randDecision = Random.Range(0, 2);
                if (randDecision == 0)
                {
                    randDest = Random.Range(0, destRandCount);
                    currentDest = destinations[randDest];
                }
                if (randDecision == 1)
                {
                    //anim.ResetTrigger("walk");
                    //anim.SetTrigger("idle");
                    StopCoroutine("StayIdle");
                    StartCoroutine("StayIdle");
                    walking = false;
                }

                walking = false;
                Invoke("SetWalkingTrue", idleTime);
            }
        }
    }
    IEnumerator StayIdle()
    {
        idleTime = Random.Range(minIdleTime, maxIdleTime);
        yield return new WaitForSeconds(idleTime);
        walking = true;
        randDest = Random.Range(0, destRandCount);
        currentDest = destinations[randDest];
        //anim.ResetTrigger("idle");
        //anim.SetTrigger("walk");
    }
}