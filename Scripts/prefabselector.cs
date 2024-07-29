using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PrefabSelectorEditor : EditorWindow
{
    private GameObject targetGameObject;
    private List<GameObject> selectedPrefabs = new List<GameObject>();

    [MenuItem("Window/Select Prefabs in Children")]
    private static void Init()
    {
        PrefabSelectorEditor window = (PrefabSelectorEditor)EditorWindow.GetWindow(typeof(PrefabSelectorEditor));
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Select Prefabs in Children", EditorStyles.boldLabel);

        targetGameObject = EditorGUILayout.ObjectField("Target GameObject", targetGameObject, typeof(GameObject), true) as GameObject;

        if (GUILayout.Button("Select Prefabs"))
        {
            selectedPrefabs.Clear();

            if (targetGameObject != null)
            {
                FindPrefabsInChildren(targetGameObject.transform);
                SelectPrefabs();
            }
            else
            {
                Debug.LogWarning("Please assign a target GameObject.");
            }
        }
    }

    private void FindPrefabsInChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (PrefabUtility.GetPrefabAssetType(child.gameObject) == PrefabAssetType.NotAPrefab)
            {
                // If the child is not a prefab, check its children recursively.
                FindPrefabsInChildren(child);
            }
            else
            {
                // If the child is a prefab, add it to the selectedPrefabs list.
                selectedPrefabs.Add(child.gameObject);
            }
        }
    }

    private void SelectPrefabs()
    {
        Selection.objects = selectedPrefabs.ToArray();
    }
}
