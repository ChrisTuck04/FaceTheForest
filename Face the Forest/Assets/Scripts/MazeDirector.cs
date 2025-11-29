/*using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class MazeDirector : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform monster;
    public Transform exitGoal;
    public NarratorManager narrator;
    public List<TreeMover> allGates;

    [Header("Configuration")]
    public float changeInterval = 15f; 
    public int maxMajorChanges = 3;    
    public float majorChangeThreshold = 20f; 

    [Header("Safety Settings")]
    public float minSafeDistance = 8f;

    private float timer;
    private int majorChangesCount = 0;
    private NavMeshPath testPath;

    void Start()
    {
        timer = changeInterval;
        testPath = new NavMeshPath();
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            AttemptMapChange();
            timer = changeInterval;
        }
    }

    void AttemptMapChange()
    {
        List<TreeMover> validCandidates = new List<TreeMover>();

        foreach (TreeMover gate in allGates)
        {
            if (IsGateSafe(gate))
            {
                validCandidates.Add(gate);
            }
        }

        if (validCandidates.Count == 0) return;

        TreeMover selectedGate = validCandidates[Random.Range(0, validCandidates.Count)];


        float oldDistance = GetPathDistance();

        bool intendedState = !selectedGate.isBlocked;
        bool wasBlocked = selectedGate.isBlocked;
        
        selectedGate.ToggleGate(intendedState);
        NavMesh.CalculatePath(player.position, exitGoal.position, NavMesh.AllAreas, testPath);

        if (testPath.status != NavMeshPathStatus.PathComplete)
        {
            Debug.Log("Change aborted: Would trap player.");
            selectedGate.ToggleGate(wasBlocked);
            return;
        }

        float newDistance = GetPathLength(testPath);
        float difference = newDistance - oldDistance;

        if (difference > majorChangeThreshold)
        {
            if (majorChangesCount >= maxMajorChanges)
            {
                Debug.Log("Change aborted: Too many major detours.");
                selectedGate.ToggleGate(wasBlocked);
                return;
            }
            else
            {
                majorChangesCount++;
            }
        }

        bool isSeen = selectedGate.IsVisibleToPlayer(Camera.main);
        if (narrator != null) narrator.ReactToMapChange(isSeen);
    }

    bool IsGateSafe(TreeMover gate)
    {
        if (Vector3.Distance(gate.transform.position, player.position) < minSafeDistance) 
            return false;

        if (monster != null && monster.gameObject.activeInHierarchy)
        {
            if (Vector3.Distance(gate.transform.position, monster.position) < minSafeDistance) 
                return false;
        }

        return true;
    }

    float GetPathDistance()
    {
        NavMesh.CalculatePath(player.position, exitGoal.position, NavMesh.AllAreas, testPath);
        return GetPathLength(testPath);
    }

    float GetPathLength(NavMeshPath path)
    {
        if (path.corners.Length < 2) return 0f;
        float dist = 0f;
        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            dist += Vector3.Distance(path.corners[i], path.corners[i + 1]);
        }
        return dist;
    }
}*/