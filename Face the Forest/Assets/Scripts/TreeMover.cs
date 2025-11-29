using UnityEngine;
using UnityEngine.AI;

public class TreeMover : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 2f;
    public float hiddenHeight = -50f;
    public float visibleHeight = 0f;

    [Header("Visibility Settings")]
    public float maxViewDistance = 12f;

    [Header("State")]
    public bool isBlocked = false;

    private NavMeshObstacle obstacle;
    private Vector3 targetPos;

    void Awake()
    {
        obstacle = GetComponent<NavMeshObstacle>();

        obstacle.carving = true; 
        

        targetPos = transform.position;
        UpdateState(isBlocked, true);
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * moveSpeed);
    }

    public void ToggleGate(bool blockPath)
    {
        UpdateState(blockPath, false);
    }

    void UpdateState(bool block, bool instant)
    {
        isBlocked = block;
        
        float y = isBlocked ? visibleHeight : hiddenHeight;
        targetPos = new Vector3(transform.position.x, y, transform.position.z);

        if (instant) transform.position = targetPos;

        obstacle.enabled = isBlocked; 
    }

    public bool IsVisibleToPlayer(Camera playerCam)
    {
        if (playerCam == null) return false;

        float dist = Vector3.Distance(transform.position, playerCam.transform.position);
        if (dist > maxViewDistance) return false;

        Vector3 viewPos = playerCam.WorldToViewportPoint(transform.position);
        if (viewPos.x < 0 || viewPos.x > 1 || viewPos.y < 0 || viewPos.y > 1 || viewPos.z <= 0)
            return false;

        RaycastHit hit;
        Vector3 dir = transform.position - playerCam.transform.position;
         
        if (Physics.Raycast(playerCam.transform.position, dir, out hit, dir.magnitude))
        {
            if (hit.transform != this.transform && !hit.transform.IsChildOf(this.transform)) 
            {
                return false; 
            }
        }

        return true;
    }
}