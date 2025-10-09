using UnityEngine;
using UnityEngine.AI; // Make sure this is included

public class EnemyAI : MonoBehaviour
{
    public Transform target;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (target != null)
        {
            // Create a NavMeshHit object to store the result of our search
            NavMeshHit hit;

            // Search for the closest point on the NavMesh to the target's position
            // It will search within a 1.0f radius (you can increase this if needed)
            if (NavMesh.SamplePosition(target.position, out hit, 1.0f, NavMesh.AllAreas))
            {
                // If a valid point was found, set that as the new destination
                agent.SetDestination(hit.position);
            }
        }
    }
}