using System.Collections.Generic;
using UnityEngine;
// script to create a holographic minimap of the world around the player 
public class SphereTrigger : MonoBehaviour
{
    public GameObject miniSphere; // Assign your mini sphere in the Inspector
    public Material miniMaterial;
    private Material sharedMiniMaterial;
    private float removeTimer = 0;
    private Vector3 sphereScaleRatio;
    private float scaleAdjustmentFactor;

    private HashSet<GameObject> currentObjectsInTrigger = new();
    private HashSet<GameObject> objectsWithMiniVersions = new();
    private HashSet<GameObject> previousObjectsInTrigger = new();


    private void Start()
    {
        // Create Shared Material to Save memory
        sharedMiniMaterial = new Material(miniMaterial);

        // Calculate the adjustment factor based on the scale of the miniSphere
        scaleAdjustmentFactor = 1 / miniSphere.transform.lossyScale.x;

        // Calculate the ratio factor based on the scale of the miniSphere
        sphereScaleRatio = new Vector3(
        miniSphere.transform.lossyScale.x / transform.lossyScale.x,
        miniSphere.transform.lossyScale.y / transform.lossyScale.y,
        miniSphere.transform.lossyScale.z / transform.lossyScale.z
        );
    }

    private void Update()
    {
        removeTimer += Time.deltaTime;
        // Move Objects if there are any
        if (currentObjectsInTrigger.Count > 0)
        {
            MoveMiniVersions();
        }
        // Sync the MiniSphere forward direction with the World
        miniSphere.transform.forward = Vector3.forward;

        // Timer To Give OnStay method time to do its thing
        if (removeTimer >= 0.1f)
        {
            // Remove mini versions for objects that are no longer in the trigger area
            RemoveExitedObjects();
            removeTimer = 0;
        }

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("World"))
        {
            if (!currentObjectsInTrigger.Contains(other.gameObject))
            {
                currentObjectsInTrigger.Add(other.gameObject);
            }

            if (!objectsWithMiniVersions.Contains(other.gameObject))
            {
                CreateMiniVersion(other.gameObject);
                objectsWithMiniVersions.Add(other.gameObject);
            }
        }
    }
    private void RemoveExitedObjects()
    {
        // Identify objects that were in the previous trigger area but not in the current one
        HashSet<GameObject> objectsToRemove = new HashSet<GameObject>(previousObjectsInTrigger);
        // Debug.Log($"Checking {objectsToRemove.Count} Objects To Be Removed");
        // Remove mini versions for objects that are no longer in the trigger area
        foreach (GameObject currentObject in currentObjectsInTrigger)
        {
            objectsToRemove.Remove(currentObject);
        }

        // Remove mini versions for objects that are no longer in the trigger area
        foreach (GameObject objectToRemove in objectsToRemove)
        {
            // Debug.Log("Sent Object For Removal");
            RemoveMiniVersion(objectToRemove);
            // Additionally, remove the object from the previousObjectsInTrigger set
            previousObjectsInTrigger.Remove(objectToRemove);
        }

        if (objectsToRemove.Count > 0)
        {
            // Debug.Log($"{objectsToRemove.Count} Objects were Removed");
        }
        else
        {
            // Debug.Log("No Objects were Removed");
        }
        previousObjectsInTrigger = new HashSet<GameObject>(currentObjectsInTrigger);
        currentObjectsInTrigger.Clear();
    }
    private void CreateMiniVersion(GameObject originalObject)
    {
        GameObject miniVersion = new GameObject("MiniVersion");
        miniVersion.transform.parent = miniSphere.transform;

        MeshFilter originalMeshFilter = originalObject.GetComponent<MeshFilter>();
        if (originalMeshFilter != null)
        {
            MeshFilter miniMeshFilter = miniVersion.AddComponent<MeshFilter>();
            miniMeshFilter.sharedMesh = Instantiate(originalMeshFilter.sharedMesh);
        }

        MeshRenderer originalMeshRenderer = originalObject.GetComponent<MeshRenderer>();
        if (originalMeshRenderer != null)
        {
            MeshRenderer miniMeshRenderer = miniVersion.AddComponent<MeshRenderer>();
            miniMeshRenderer.sharedMaterials = originalMeshRenderer.sharedMaterials;
            miniMeshRenderer.material = sharedMiniMaterial;
        }

        // Calculate the Scale ratio between the object and the mini sphere
        Vector3 scaleRatio = new Vector3(
            originalObject.transform.localScale.x / transform.localScale.x,
            originalObject.transform.localScale.y / transform.localScale.y,
            originalObject.transform.localScale.z / transform.localScale.z
        );

        // Set the local Scale of the mini version based on the Scale ratio
        miniVersion.transform.localScale = scaleRatio;
        miniVersion.transform.localRotation = originalObject.transform.rotation;

        // Give the mini version a unique name based on a unique identifier
        int uniqueIdentifier = originalObject.GetHashCode();
        miniVersion.name = "MiniVersion_ID_" + uniqueIdentifier;
    }

    private void MoveMiniVersions()
    {
        foreach (GameObject originalObject in previousObjectsInTrigger)
        {
            GameObject miniVersion = FindMiniVersion(originalObject);

            if (miniVersion != null)
            {
                // Calculate relative position and rotation
                Vector3 relativePosition = originalObject.transform.position - transform.position;

                // Adjust positions based on Scale difference
                Vector3 adjustedPosition = Vector3.Scale(relativePosition, sphereScaleRatio);

                // Apply the dynamic scale adjustment to the local position of the MiniVersion
                miniVersion.transform.localPosition = adjustedPosition * scaleAdjustmentFactor;
            }
        }
    }
    private void RemoveMiniVersion(GameObject originalObject)
    {
        GameObject miniVersion = FindMiniVersion(originalObject);
        // Debug.Log("Tried to Remove MiniVersion");
        if (miniVersion != null)
        {
            // Remove the mini version from the miniSphere's children
            Destroy(miniVersion);
            objectsWithMiniVersions.Remove(originalObject);
        }
        else
        {
            // Debug.Log("MiniVersion Is Null");
        }
    }
    private GameObject FindMiniVersion(GameObject originalObject)
    {
        int uniqueIdentifier = originalObject.GetHashCode();
        string miniVersionName = "MiniVersion_ID_" + uniqueIdentifier;

        foreach (Transform child in miniSphere.transform)
        {
            if (child.name == miniVersionName)
            {
                return child.gameObject;
            }
        }

        // Debug.Log("Could Not Find MiniVersion");
        return null;
    }
}
