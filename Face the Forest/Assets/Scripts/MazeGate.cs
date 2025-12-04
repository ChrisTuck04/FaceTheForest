using UnityEngine;
using UnityEngine.AI;

public class MazeGate : MonoBehaviour
{
    [Header("References")]
    public Transform visualModel;

    [Header("Settings")]
    public float moveSpeed = 3f;
    public float hiddenHeight = -5f;
    public float visibleHeight = 0f;

    private Vector3 targetLocalPos;
    private NavMeshObstacle obstacle;

    void Awake()
    {
        InitializeObstacle();

        float startY = obstacle != null && obstacle.enabled ? visibleHeight : hiddenHeight;
        targetLocalPos = new Vector3(visualModel.localPosition.x, startY, visualModel.localPosition.z);
        visualModel.localPosition = targetLocalPos;
    }

    void Update()
    {
        // Only animate visuals, NavMesh is already updated
        visualModel.localPosition = Vector3.Lerp(visualModel.localPosition, targetLocalPos, Time.deltaTime * moveSpeed);
    }

    void InitializeObstacle()
    {
        if (obstacle == null)
        {
            obstacle = GetComponent<NavMeshObstacle>();
            if (obstacle != null)
            {
                obstacle.carving = true;
                obstacle.carveOnlyStationary = false;
            }
        }
    }

    public void SetGateState(bool shouldBlock)
    {
        InitializeObstacle();

        if (obstacle == null)
        {
            Debug.LogError($"No NavMeshObstacle on {gameObject.name}!");
            return;
        }

        // IMMEDIATELY update obstacle for pathfinding
        obstacle.enabled = shouldBlock;

        // Set visual target (animates in Update)
        float targetY = shouldBlock ? visibleHeight : hiddenHeight;
        targetLocalPos = new Vector3(visualModel.localPosition.x, targetY, visualModel.localPosition.z);
    }
}