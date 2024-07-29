using UnityEditor;
using UnityEngine;

public class EnableGIForAllMeshRenderersEditor : EditorWindow
{
    [MenuItem("Custom/Enable GI for All Mesh Renderers")]
    static void EnableGI()
    {
        GameObject[] rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

        Undo.RecordObjects(rootObjects, "Enable GI for All Mesh Renderers");

        foreach (GameObject rootObject in rootObjects)
        {
            EnableGIRecursively(rootObject);
        }
    }

    static void EnableGIRecursively(GameObject obj)
    {
        MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.gameObject.isStatic = true;
        }

        for (int i = 0; i < obj.transform.childCount; i++)
        {
            GameObject childObject = obj.transform.GetChild(i).gameObject;
            EnableGIRecursively(childObject);
        }
    }
}
