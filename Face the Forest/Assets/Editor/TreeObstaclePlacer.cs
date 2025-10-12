using UnityEngine;
using UnityEditor;
using Unity.AI.Navigation;
using UnityEngine.AI; // Added for NavMeshArea functionality

// This class contains the static logic for placing obstacle GameObjects.
public static class TreeObstaclePlacer
{
    private const string ParentObjectName = "BAKED_STATIC_OBSTACLES";
    private const string ObstacleLayerName = "NavMeshObstacle";
    private const string NotWalkableAreaName = "Not Walkable"; // Define the area name constant

    public static void PlaceAndBakeObstacles()
    {
        Terrain terrain = Terrain.activeTerrain;
        if (terrain == null)
        {
            EditorUtility.DisplayDialog("Error", "No active Terrain found in the scene.", "OK");
            return;
        }

        // 1. Clean up old objects and set up parent
        GameObject existingParent = GameObject.Find(ParentObjectName);
        if (existingParent != null)
        {
            Object.DestroyImmediate(existingParent);
        }

        // Get the integer index for the "Not Walkable" area
        int notWalkableAreaIndex = NavMesh.GetAreaFromName(NotWalkableAreaName);
        if (notWalkableAreaIndex == -1)
        {
             Debug.LogError($"NavMesh Area '{NotWalkableAreaName}' not found. Please ensure it exists in Window > AI > Navigation > Areas tab.");
             return;
        }

        GameObject parent = new GameObject(ParentObjectName);
        parent.isStatic = true; // Crucial: Mark as static for the NavMesh bake
        
        // Ensure the Obstacle layer exists (optional but good practice)
        int obstacleLayer = LayerMask.NameToLayer(ObstacleLayerName);
        if (obstacleLayer == -1)
        {
            Debug.LogWarning($"Layer '{ObstacleLayerName}' not found. Using Default layer.");
            obstacleLayer = 0;
        }

        TreeInstance[] treeInstances = terrain.terrainData.treeInstances;
        float width = terrain.terrainData.size.x;
        float length = terrain.terrainData.size.z;
        float height = terrain.terrainData.size.y;
        Vector3 terrainPosition = terrain.GetPosition();

        Debug.Log($"Placing {treeInstances.Length} static obstacle objects...");

        // 2. Iterate through all trees and place an invisible collider
        for (int i = 0; i < treeInstances.Length; i++)
        {
            // We use EditorUtility.DisplayProgressBar to show progress during long loops
            if (EditorUtility.DisplayCancelableProgressBar("Placing Obstacles", $"Processing tree {i + 1}/{treeInstances.Length}", (float)i / treeInstances.Length))
            {
                Debug.LogWarning("Obstacle placement cancelled by user.");
                Object.DestroyImmediate(parent);
                EditorUtility.ClearProgressBar();
                return;
            }

            TreeInstance tree = treeInstances[i];
            TreePrototype prototype = terrain.terrainData.treePrototypes[tree.prototypeIndex];
            GameObject prefab = prototype.prefab;

            // Calculate exact world position
            Vector3 localPos = tree.position;
            Vector3 worldPosition = new Vector3(
                localPos.x * width,
                localPos.y * height,
                localPos.z * length
            ) + terrainPosition;

            // Use the prefab's collider to determine obstacle size (best effort)
            // Note: Collider.bounds is in world space, but we adjust it here since the source scale is not 1.
            Collider sourceCollider = prefab.GetComponent<Collider>();
            
            // Default size and center if no collider is found on the prefab
            Vector3 obstacleSize = Vector3.one * 2f;
            Vector3 obstacleCenter = Vector3.zero;

            if (sourceCollider != null)
            {
                // We use the local scale of the tree instance to correctly size the box
                Vector3 treeScale = new Vector3(tree.widthScale, tree.heightScale, tree.widthScale);
                
                // Get the local collider properties from the scaled tree
                if (sourceCollider is BoxCollider boxSource)
                {
                    obstacleSize = Vector3.Scale(boxSource.size, treeScale);
                    obstacleCenter = Vector3.Scale(boxSource.center, treeScale);
                }
                else if (sourceCollider is CapsuleCollider capsuleSource)
                {
                     // Approximate capsule with a box for NavMesh carving simplicity
                    obstacleSize = new Vector3(capsuleSource.radius * 2 * treeScale.x, capsuleSource.height * treeScale.y, capsuleSource.radius * 2 * treeScale.z);
                    obstacleCenter = Vector3.Scale(capsuleSource.center, treeScale);
                }
            }


            // Create a simple cube to act as the NavMesh obstacle
            GameObject obs = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obs.name = "StaticTreeBlock_" + i;
            obs.transform.SetParent(parent.transform);
            obs.transform.position = worldPosition;
            obs.transform.rotation = Quaternion.AngleAxis(tree.rotation * Mathf.Rad2Deg, Vector3.up);
            // obs.transform.localScale = new Vector3(tree.widthScale, tree.heightScale, tree.widthScale); // Scale is handled by BoxCollider size
            
            // Adjust the actual BoxCollider component
            BoxCollider boxColl = obs.GetComponent<BoxCollider>();
            if (boxColl != null)
            {
                // Set the size and center derived from the source collider
                boxColl.size = obstacleSize;
                boxColl.center = obstacleCenter; 
            }

            obs.layer = obstacleLayer;
            obs.isStatic = true; // Mark the individual object as static
            
            // We want this object to be an obstacle, so we disable the MeshRenderer
            // and hide it in the scene but keep the collider for the baker.
            MeshRenderer renderer = obs.GetComponent<MeshRenderer>();
            if (renderer != null) renderer.enabled = false;

            // Add the NavMeshModifier component
            NavMeshModifier modifier = obs.AddComponent<NavMeshModifier>();
            
            // FIX: Use the correct, single 'Area' property to set the area type
            modifier.area = notWalkableAreaIndex;
        }
        
        EditorUtility.ClearProgressBar();
        Debug.Log($"Successfully placed {treeInstances.Length} static obstacles for baking.");

        // 3. Trigger the NavMesh Bake
        NavMeshSurface surface = terrain.GetComponent<NavMeshSurface>();
        if (surface != null)
        {
            surface.RemoveData();
            surface.BuildNavMesh();
            EditorUtility.DisplayDialog("Bake Complete", $"Successfully carved {treeInstances.Length} obstacles into the NavMesh.", "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("Warning", "NavMeshSurface component not found on the Terrain. Please add one and bake manually.", "OK");
        }
    }
}
