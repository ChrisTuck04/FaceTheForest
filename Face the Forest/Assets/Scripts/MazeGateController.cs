using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MazeGateController : MonoBehaviour
{
    [Header("Gate References")]
    public MazeGate[] gates;

    [Header("Pathfinding Validation")]
    public Transform playerStartPosition;
    public Transform mazeExit;
    public float sampleDistance = 2f;

    [Header("Timing Settings")]
    public float intervalSeconds = 45f;
    public float navMeshWaitTime = 0.1f; // Use actual time instead of frames

    [Header("Randomization Settings")]
    [Range(0f, 1f)]
    public float chanceToBlock = 0.2f;

    [Header("Debug")]
    public bool showDebugInfo = true;
    public bool drawDebugPath = true;

    [System.NonSerialized]
    public bool isProcessing = false;
    private Coroutine currentRandomization = null;

    void Start()
    {
        if (gates == null || gates.Length == 0)
        {
            Debug.LogError("No gates assigned!");
            return;
        }

        if (playerStartPosition == null || mazeExit == null)
        {
            Debug.LogError("Player start or exit not assigned!");
            return;
        }

        StartCoroutine(RandomizeGatesRoutine());
    }

    IEnumerator RandomizeGatesRoutine()
    {
        if (!isProcessing)
        {
            yield return StartCoroutine(RandomizeGatesWithValidation());
        }

        while (true)
        {
            Debug.Log("Waiting for next cycle...");
            yield return new WaitForSeconds(intervalSeconds);

            if (!isProcessing)
            {
                yield return StartCoroutine(RandomizeGatesWithValidation());
            }
        }
    }

    public IEnumerator RandomizeGatesWithValidation()
    {
        if (isProcessing)
        {
            Debug.LogWarning("Already processing");
            yield break;
        }
        isProcessing = true;
        float startTime = Time.realtimeSinceStartup;

        Debug.Log("=== Starting Gate Randomization ===");

        RandomizeGates();

        // Use WaitForSeconds instead of yield return null
        yield return new WaitForSeconds(navMeshWaitTime);

        float elapsed = Time.realtimeSinceStartup - startTime;
        Debug.Log($"After randomize: {elapsed:F3}s");

        bool pathExists = IsPathAvailableQuick();
        Debug.Log($"Path exists: {pathExists}");

        if (!pathExists)
        {
            Debug.LogWarning("No valid path! Opening gates...");
            yield return StartCoroutine(OpenBlockingGatesFast());
        }
        else
        {
            Debug.Log("? Valid path exists!");
        }

        elapsed = Time.realtimeSinceStartup - startTime;
        Debug.Log($"=== Complete in {elapsed:F3}s ===");

        isProcessing = false;
    }

    IEnumerator OpenBlockingGatesFast()
    {
        List<MazeGate> raisedGates = new List<MazeGate>();

        foreach (MazeGate gate in gates)
        {
            if (gate != null)
            {
                NavMeshObstacle obs = gate.GetComponent<NavMeshObstacle>();
                if (obs != null && obs.enabled)
                {
                    raisedGates.Add(gate);
                }
            }
        }

        if (raisedGates.Count == 0)
        {
            Debug.LogWarning("No raised gates!");
            yield break;
        }

        Debug.Log($"Testing {raisedGates.Count} gates...");
        ShuffleList(raisedGates);

        foreach (MazeGate gate in raisedGates)
        {
            gate.SetGateState(false);
            yield return new WaitForSeconds(navMeshWaitTime);

            if (IsPathAvailableQuick())
            {
                Debug.Log($"? Opened {gate.name}!");
                yield break;
            }

            gate.SetGateState(true);
            yield return new WaitForSeconds(navMeshWaitTime);
        }

        Debug.LogWarning("Opening all gates...");
        OpenAllGates();
    }

    void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    void OpenAllGates()
    {
        foreach (MazeGate gate in gates)
        {
            if (gate != null) gate.SetGateState(false);
        }
    }

    bool IsPathAvailableQuick()
    {
        NavMeshPath path = new NavMeshPath();

        Vector3 startPos = playerStartPosition.position;
        Vector3 exitPos = mazeExit.position;

        NavMeshHit hit;
        if (!NavMesh.SamplePosition(startPos, out hit, sampleDistance, NavMesh.AllAreas))
            return false;
        startPos = hit.position;

        if (!NavMesh.SamplePosition(exitPos, out hit, sampleDistance, NavMesh.AllAreas))
            return false;
        exitPos = hit.position;

        return NavMesh.CalculatePath(startPos, exitPos, NavMesh.AllAreas, path)
               && path.status == NavMeshPathStatus.PathComplete;
    }

    public bool IsPathAvailableDetailed()
    {
        NavMeshPath path = new NavMeshPath();

        Vector3 startPos = playerStartPosition.position;
        Vector3 exitPos = mazeExit.position;

        Debug.Log($"[PATH] Start: {startPos}, Exit: {exitPos}");

        NavMeshHit startHit, exitHit;
        bool startFound = NavMesh.SamplePosition(startPos, out startHit, sampleDistance, NavMesh.AllAreas);
        bool exitFound = NavMesh.SamplePosition(exitPos, out exitHit, sampleDistance, NavMesh.AllAreas);

        if (!startFound || !exitFound)
        {
            Debug.LogError($"[PATH] Not on NavMesh! Start: {startFound}, Exit: {exitFound}");
            return false;
        }

        bool pathCalc = NavMesh.CalculatePath(startHit.position, exitHit.position, NavMesh.AllAreas, path);
        Debug.Log($"[PATH] Status: {path.status}, Corners: {path.corners.Length}");

        int blocking = 0;
        foreach (MazeGate gate in gates)
        {
            if (gate != null)
            {
                NavMeshObstacle obs = gate.GetComponent<NavMeshObstacle>();
                if (obs != null && obs.enabled)
                {
                    blocking++;
                    Debug.Log($"[PATH] {gate.name} BLOCKING");
                }
            }
        }
        Debug.Log($"[PATH] Blocking gates: {blocking}");

        bool complete = pathCalc && path.status == NavMeshPathStatus.PathComplete;
        Debug.Log($"[PATH] Result: {(complete ? "COMPLETE ?" : "NO PATH ?")}");
        return complete;
    }

    void RandomizeGates()
    {
        foreach (MazeGate gate in gates)
        {
            if (gate != null)
            {
                gate.SetGateState(Random.value < chanceToBlock);
            }
        }
    }

    void OnDrawGizmos()
    {
        if (!drawDebugPath || playerStartPosition == null || mazeExit == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(playerStartPosition.position, 0.5f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(mazeExit.position, 0.5f);

        NavMeshPath path = new NavMeshPath();
        Vector3 startPos = playerStartPosition.position;
        Vector3 exitPos = mazeExit.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(startPos, out hit, sampleDistance, NavMesh.AllAreas))
            startPos = hit.position;
        if (NavMesh.SamplePosition(exitPos, out hit, sampleDistance, NavMesh.AllAreas))
            exitPos = hit.position;

        if (NavMesh.CalculatePath(startPos, exitPos, NavMesh.AllAreas, path))
        {
            Gizmos.color = path.status == NavMeshPathStatus.PathComplete ? Color.cyan : Color.yellow;
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                Gizmos.DrawLine(path.corners[i], path.corners[i + 1]);
                Gizmos.DrawWireSphere(path.corners[i], 0.3f);
            }
        }
    }

    public void StopCurrentRandomization()
    {
        if (currentRandomization != null)
        {
            StopCoroutine(currentRandomization);
            currentRandomization = null;
        }
        isProcessing = false;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MazeGateController))]
    public class MazeGateControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            MazeGateController controller = (MazeGateController)target;

            EditorGUI.BeginDisabledGroup(controller.isProcessing);
            if (GUILayout.Button("Test Randomize Gates"))
            {
                controller.StopCurrentRandomization();
                controller.currentRandomization = controller.StartCoroutine(controller.RandomizeGatesWithValidation());
            }
            EditorGUI.EndDisabledGroup();

            if (controller.isProcessing && GUILayout.Button("STOP"))
            {
                controller.StopCurrentRandomization();
            }

            if (GUILayout.Button("Test Path Check"))
            {
                controller.IsPathAvailableDetailed();
            }

            if (GUILayout.Button("Force Open All"))
            {
                controller.OpenAllGates();
            }
        }
    }
#endif
}