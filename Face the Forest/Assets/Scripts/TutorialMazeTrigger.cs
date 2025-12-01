using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutorialMazeTrigger : MonoBehaviour
{
    [Header("Event Settings")]
    [Tooltip("Drag the specific MazeGate objects you want to move here.")]
    public List<MazeGate> gatesToControl;
    
    [Tooltip("How long the path stays blocked (in seconds) before opening again.")]
    public float blockDuration = 5f;
    private bool hasTriggered = false;
    public NarratorManager narrator; 

    void OnTriggerEnter(Collider other)
    {
        // Only trigger once, and only for the player
        if (hasTriggered || !other.CompareTag("Player")) return;

        hasTriggered = true;
        Debug.Log("Tutorial Event Started: Trap Player.");

        StartCoroutine(TimedMazeEvent());
    }

    IEnumerator TimedMazeEvent()
    {
        // ... inside TimedMazeEvent Coroutine ...

        // PHASE 1: BLOCK
        foreach (MazeGate gate in gatesToControl)
        {
            if (gate != null) gate.SetGateState(true);
        }

        // --- UPDATED NARRATOR CALL ---
        if (narrator != null)
        {
            narrator.PlayTutorialTrap(); // <--- NEW CALL
        }

        yield return new WaitForSeconds(blockDuration);

        // PHASE 2: OPEN
        foreach (MazeGate gate in gatesToControl)
        {
            if (gate != null) gate.SetGateState(false);
        }

        // --- UPDATED NARRATOR CALL ---
        if (narrator != null)
        {
            narrator.PlayTutorialClear(); // <--- NEW CALL
        }
    }
}