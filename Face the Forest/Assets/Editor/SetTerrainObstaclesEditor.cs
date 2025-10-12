using UnityEditor;
using UnityEngine;

// NOTE: This script must be placed in a folder named 'Editor'.
public class SetTerrainObstaclesEditor : EditorWindow
{
    // The mono script field from the previous version is removed as it's no longer necessary.

    [MenuItem("Tools/Set Terrain Tree Obstacles")]
    public static void ShowWindow()
    {
        // Display the custom window
        EditorWindow.GetWindow<SetTerrainObstaclesEditor>("Set Tree Terrain Obstacles");
    }

    private void OnGUI()
    {
        GUILayout.Label("Set Tree Terrain Obstacles", EditorStyles.boldLabel);
        
        EditorGUILayout.Space();
        GUILayout.Label("Editor-Time Baking (Recommended)", EditorStyles.largeLabel);
        
        EditorGUILayout.HelpBox(
            "This tool creates invisible, static obstacle objects for every tree on the terrain and automatically re-bakes the NavMesh. This is the correct, stable way to handle thousands of static trees for AI navigation.", 
            MessageType.Info
        );

        EditorGUILayout.Space();

        if (GUILayout.Button("Place Obstacles and Re-Bake NAVMESH"))
        {
            PlaceAndBakeObstacles();
        }
    }

    private void PlaceAndBakeObstacles()
    {
        try
        {
            // Call the static method from the dedicated placer class
            TreeObstaclePlacer.PlaceAndBakeObstacles();
        }
        catch (System.Exception e)
        {
            // Catches any unexpected errors during the lengthy generation process
            Debug.LogError($"Obstacle Placement Failed! Error: {e.Message}");
            EditorUtility.DisplayDialog("Error", $"Obstacle Placement Failed! See Console for details.", "OK");
        }
    }
}
