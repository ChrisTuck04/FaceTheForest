using UnityEngine;
using UnityEngine.AI;

public class MazeGate : MonoBehaviour
{
    [Header("References")]
    public Transform visualModel; // Assign the Child (Tree Mesh) here

    [Header("Settings")]
    public float moveSpeed = 3f; // Slightly faster for dramatic effect
    public float hiddenHeight = -5f; // Y pos when path is OPEN
    public float visibleHeight = 0f; // Y pos when path is BLOCKED

    private Vector3 targetLocalPos;
    private NavMeshObstacle obstacle;

    void Awake()
    {
        obstacle = GetComponent<NavMeshObstacle>();
        obstacle.carving = true;
        
        // Initialize visuals based on whether the obstacle is currently enabled
        float startY = obstacle.enabled ? visibleHeight : hiddenHeight;
        targetLocalPos = new Vector3(visualModel.localPosition.x, startY, visualModel.localPosition.z);
        visualModel.localPosition = targetLocalPos;
    }

    void Update()
    {
        // Smoothly move the visual child
        visualModel.localPosition = Vector3.Lerp(visualModel.localPosition, targetLocalPos, Time.deltaTime * moveSpeed);
    }

    public void SetGateState(bool shouldBlock)
    {
        if (obstacle == null) 
        {
            obstacle = GetComponent<NavMeshObstacle>();
        }
        // -----------------------------

        obstacle.enabled = shouldBlock;

        float targetY = shouldBlock ? visibleHeight : hiddenHeight;
        targetLocalPos = new Vector3(visualModel.localPosition.x, targetY, visualModel.localPosition.z);
        
        // If we are in the Editor (not playing), we must force the transform update immediately
        // because Update() isn't running to do the smooth Lerp.
        if (!Application.isPlaying)
        {
            visualModel.localPosition = targetLocalPos;
        }
    }
}