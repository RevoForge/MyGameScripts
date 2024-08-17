using System.Collections;
using System.IO;
using UnityEngine;
using static Revo.Methods.SceneController;

public class ItemSlotController : MonoBehaviour
{
    [SerializeField] private GameObject cameraReference;
    private SceneConfiguration currentSceneConfig;
    private Transform rightSlot;
    private Transform leftSlot;
    private bool isRotating; // Flag to control rotation status
    private Quaternion targetRotation;
    private AssetBundle weaponAssetBundle;
    private AssetBundleManager assetBundleManager;

    private void OnEnable()
    {
        rightSlot = transform.GetChild(0);
        leftSlot = transform.GetChild(1);
        assetBundleManager = AssetBundleManager.Instance;
        weaponAssetBundle = assetBundleManager.GetWeaponAssetBundle();
    }
    private float lastMatchedRotation = 0.0f;

    private void Update()
    {
        // Calculate the camera's current rotation
        float cameraYRotation = cameraReference.transform.localEulerAngles.y;

        // Calculate the difference between the camera's rotation and the last matched rotation
        float rotationDifference = cameraYRotation - lastMatchedRotation;

        // Calculate the adjusted rotation for the item
        float adjustedRotation = lastMatchedRotation + rotationDifference / 4.0f;

        // Check if the camera has moved at least 60 degrees since the last match
        if (Mathf.Abs(rotationDifference) >= 60.0f)
        {
            lastMatchedRotation = cameraYRotation;
            adjustedRotation = cameraYRotation; // Match their rotations

            // Rotate the item over time to match the camera's rotation
            if (!isRotating)
            {
                isRotating = true;
                targetRotation = Quaternion.Euler(0, adjustedRotation, 0);
                StartCoroutine(RotateToTarget(150.0f)); // Adjust the speed as needed
            }
        }
        else
        {
            if (!isRotating)
            {
                transform.localRotation = Quaternion.Euler(0, adjustedRotation, 0);
            }

            // Match the item's position to the camera's position
            transform.localPosition = new Vector3(
                cameraReference.transform.localPosition.x,
                cameraReference.transform.localPosition.y - 0.5f,
                cameraReference.transform.localPosition.z);
        }
    }

    private IEnumerator RotateToTarget(float rotateSpeed)
    {
        while (Quaternion.Angle(transform.localRotation, targetRotation) > 0.1f)
        {
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetRotation, rotateSpeed * Time.deltaTime);
            yield return null;
        }
        transform.localRotation = targetRotation;
        isRotating = false; // Rotation finished, allow position update
    }


    public void SaveConfiguration()
    {
        string filePath = Application.persistentDataPath + "/sceneConfig.json";
        SceneConfiguration currentSceneConfig;

        if (File.Exists(filePath))
        {
            // Read the JSON data from the file
            string json = File.ReadAllText(filePath);

            // Convert the JSON data back to a SceneConfiguration object
            currentSceneConfig = JsonUtility.FromJson<SceneConfiguration>(json);
        }
        else
        {
            // If the file doesn't exist, create a new SceneConfiguration
            currentSceneConfig = new SceneConfiguration();
        }

        // Update the prefab names in the SceneConfiguration
        currentSceneConfig.rightItemPrefabName = CleanPrefabName(FindPrefabName(true));
        currentSceneConfig.leftItemPrefabName = CleanPrefabName(FindPrefabName(false));

        // Convert the configuration object to JSON
        string updatedJson = JsonUtility.ToJson(currentSceneConfig);

        // Write the JSON data to the file
        File.WriteAllText(filePath, updatedJson);
    }


    private string FindPrefabName(bool isRightItem)
    {
        Transform targetSlot = isRightItem ? rightSlot : leftSlot;

        if (targetSlot.childCount > 0)
        {
            return targetSlot.GetChild(0).gameObject.name;
        }
        else
        {
            return "";
        }
    }

    private string CleanPrefabName(string name)
    {
        // Remove the "(Clone)" suffix
        if (name.EndsWith("(Clone)"))
        {
            name = name.Substring(0, name.Length - 7); // Remove the "(Clone)" part
        }
        return name;
    }


    public void LoadConfiguration()
    {
        string filePath = Application.persistentDataPath + "/sceneConfig.json";
        if (!File.Exists(filePath))
        {
            // The file doesn't exist, so create a default configuration and run again
            SaveConfiguration();
            LoadConfiguration();
        }
        else
        {
            // Read the JSON data from the file
            string json = File.ReadAllText(filePath);

            // Convert the JSON data back to a SceneConfiguration object
            currentSceneConfig = JsonUtility.FromJson<SceneConfiguration>(json);

            // Load and instantiate the prefabs
            string rightItemPrefabName = currentSceneConfig.rightItemPrefabName;
            string leftItemPrefabName = currentSceneConfig.leftItemPrefabName;

            // Instantiate the right item prefab
            if (!string.IsNullOrEmpty(rightItemPrefabName))
            {
                InstantiateWeaponFromAssetBundle(rightItemPrefabName, rightSlot);
            }
            // Instantiate the left item prefab
            if (!string.IsNullOrEmpty(leftItemPrefabName))
            {
                InstantiateWeaponFromAssetBundle(leftItemPrefabName, leftSlot);
            }
        }
    }

    private void InstantiateWeaponFromAssetBundle(string weaponPrefabName, Transform parentTransform)
    {
        if (weaponAssetBundle == null)
        {
            Debug.LogError("Failed to load Asset Bundle: weapons");
            return;
        }

        // Load the weapon prefab from the Asset Bundle
        GameObject weaponPrefab = weaponAssetBundle.LoadAsset<GameObject>(weaponPrefabName);

        if (weaponPrefab == null)
        {
            Debug.LogError("Failed to load weapon prefab: " + weaponPrefabName);
            return;
        }

        // Instantiate the weapon prefab and set its parent
        GameObject instantiatedWeapon = Instantiate(weaponPrefab, parentTransform);

        // Get the Rigidbody component and set isKinematic to true
        Rigidbody weaponRigidbody = instantiatedWeapon.GetComponent<Rigidbody>();
        if (weaponRigidbody != null)
        {
            weaponRigidbody.isKinematic = true;
        }
        // Compensate for the parent's scale
        instantiatedWeapon.transform.localScale = Vector3.Scale(instantiatedWeapon.transform.localScale, new Vector3(1 / parentTransform.localScale.x, 1 / parentTransform.localScale.y, 1 / parentTransform.localScale.z));
    }
}
