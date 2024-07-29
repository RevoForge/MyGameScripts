using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MissingMeshFilterChecker : EditorWindow
{
    private List<GameObject> objectsWithMissingMeshes = new List<GameObject>();

    [MenuItem("Custom/Check Missing Mesh Filters")]
    private static void Init()
    {
        MissingMeshFilterChecker window = (MissingMeshFilterChecker)EditorWindow.GetWindow(typeof(MissingMeshFilterChecker));
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("GameObjects with Missing Mesh Filters:", EditorStyles.boldLabel);

        if (GUILayout.Button("Check"))
        {
            objectsWithMissingMeshes.Clear();

            // Get all GameObjects with MeshFilter components in the scene
            MeshFilter[] meshFilters = FindObjectsOfType<MeshFilter>();

            foreach (MeshFilter meshFilter in meshFilters)
            {
                // Check if the mesh of the MeshFilter component is missing (null)
                if (meshFilter.sharedMesh == null)
                {
                    // The GameObject has a missing mesh
                    objectsWithMissingMeshes.Add(meshFilter.gameObject);
                }
            }
        }

        EditorGUILayout.Space();

        // Display the list of GameObjects with missing meshes
        foreach (GameObject obj in objectsWithMissingMeshes)
        {
            if (GUILayout.Button(obj.name))
            {
                Selection.activeGameObject = obj;
            }
        }
    }
}
