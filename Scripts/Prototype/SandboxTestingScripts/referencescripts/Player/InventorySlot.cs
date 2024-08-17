using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using static Revo.Methods.ObjectInteraction;
using static Revo.Methods.SceneController;

public class InventorySlot : MonoBehaviour
{
    private Image itemSprite;
    public InventorySlots inventorySlot;
    private string currentObjectName;
    public bool itemStored = false;
    private AssetBundle weaponAssetBundle;
    private AssetBundle spriteAssetBundle;
    private AssetBundleManager assetBundleManager;
    void OnEnable()
    {
        itemSprite = GetComponentInChildren<Image>();
        if (!itemStored) { itemSprite.enabled = false; }
        assetBundleManager = AssetBundleManager.Instance;
        spriteAssetBundle = assetBundleManager.GetSpriteAssetBundle();
        weaponAssetBundle = assetBundleManager.GetWeaponAssetBundle();
    }
    public void ItemDeposited(string itemType, string objectName)
    {
        itemStored = true;
        currentObjectName = objectName;
        SpriteFromAssetBundle(itemType);
        SaveConfiguration(itemType);
    }
    public void ItemRemoved()
    {
        InstantiateWeaponFromAssetBundle(currentObjectName);
        currentObjectName = "";
        itemSprite.enabled = false;
        itemSprite.sprite = null;
        SaveConfiguration("");
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("RightHand"))
        {
            if (SteamVR_Input.GetStateDown("GrabGrip", SteamVR_Input_Sources.RightHand) && itemStored)
            {
                itemStored = false;
                ItemRemoved();
            }
        }
        if (other.CompareTag("LeftHand"))
        {
            if (SteamVR_Input.GetStateDown("GrabGrip", SteamVR_Input_Sources.LeftHand) && itemStored)
            {
                itemStored = false;
                ItemRemoved();
            }
        }

    }
    private void InstantiateWeaponFromAssetBundle(string weaponPrefabName)
    {
        // Load the weapon prefab from the Asset Bundle
        GameObject weaponPrefab = weaponAssetBundle.LoadAsset<GameObject>(weaponPrefabName);

        if (weaponPrefab == null)
        {
            Debug.LogError("Failed to load weapon prefab: " + weaponPrefabName);
            return;
        }

        Transform lootspot = transform.Find("Loot Spot");

        // Instantiate the object at the final position
        GameObject instantiatedWeapon = Instantiate(weaponPrefab, transform);
        instantiatedWeapon.transform.localScale = Vector3.Scale(instantiatedWeapon.transform.localScale, new Vector3(1 / transform.localScale.x, 1 / transform.localScale.y, 1 / transform.localScale.z));
        instantiatedWeapon.transform.SetParent(null);
        instantiatedWeapon.transform.SetPositionAndRotation(lootspot.position, Quaternion.identity);

        // Get the Rigidbody component and set isKinematic to true
        Rigidbody weaponRigidbody = instantiatedWeapon.GetComponent<Rigidbody>();
        if (weaponRigidbody != null)
        {
            weaponRigidbody.isKinematic = true;
        }
    }
    public void SaveConfiguration(string itemTypes)
    {
        string filePath = Application.persistentDataPath + "/sceneConfig.json";

        // Read the JSON data from the file and convert it to a SceneConfiguration object
        SceneConfiguration currentSceneConfig = JsonUtility.FromJson<SceneConfiguration>(File.ReadAllText(filePath));

        // Assign the item type and name based on the inventory slot
        switch (inventorySlot)
        {
            case InventorySlots.TopLeft:
                currentSceneConfig.topLeftItem = itemTypes;
                currentSceneConfig.topLeftItemName = currentObjectName;
                break;
            case InventorySlots.TopCenter:
                currentSceneConfig.topCenterItem = itemTypes;
                currentSceneConfig.topCenterItemName = currentObjectName;
                break;
            case InventorySlots.TopRight:
                currentSceneConfig.topRightItem = itemTypes;
                currentSceneConfig.topRightItemName = currentObjectName;
                break;
            case InventorySlots.BottomLeft:
                currentSceneConfig.bottomLeftItem = itemTypes;
                currentSceneConfig.bottomLeftItemName = currentObjectName;
                break;
            case InventorySlots.BottomCenter:
                currentSceneConfig.bottomCenterItem = itemTypes;
                currentSceneConfig.bottomCenterItemName = currentObjectName;
                break;
            case InventorySlots.BottomRight:
                currentSceneConfig.bottomRightItem = itemTypes;
                currentSceneConfig.bottomRightItemName = currentObjectName;
                break;
        }

        // Convert the modified configuration object to JSON
        string json2 = JsonUtility.ToJson(currentSceneConfig);

        // Write the JSON data back to the file
        File.WriteAllText(filePath, json2);
    }
    public void LoadConfiguration()
    {
        string filePath = Application.persistentDataPath + "/sceneConfig.json";

        // Read the JSON data from the file and convert it to a SceneConfiguration object
        SceneConfiguration currentSceneConfig = JsonUtility.FromJson<SceneConfiguration>(File.ReadAllText(filePath));

        string item = "";
        string itemName = "";

        // Assign the item type and name based on the inventory slot
        switch (inventorySlot)
        {
            case InventorySlots.TopLeft:
                item = currentSceneConfig.topLeftItem;
                itemName = currentSceneConfig.topLeftItemName;
                break;
            case InventorySlots.TopCenter:
                item = currentSceneConfig.topCenterItem;
                itemName = currentSceneConfig.topCenterItemName;
                break;
            case InventorySlots.TopRight:
                item = currentSceneConfig.topRightItem;
                itemName = currentSceneConfig.topRightItemName;
                break;
            case InventorySlots.BottomLeft:
                item = currentSceneConfig.bottomLeftItem;
                itemName = currentSceneConfig.bottomLeftItemName;
                break;
            case InventorySlots.BottomCenter:
                item = currentSceneConfig.bottomCenterItem;
                itemName = currentSceneConfig.bottomCenterItemName;
                break;
            case InventorySlots.BottomRight:
                item = currentSceneConfig.bottomRightItem;
                itemName = currentSceneConfig.bottomRightItemName;
                break;
        }

        // Check if an item exists for this inventory slot
        if (!string.IsNullOrEmpty(item))
        {
            currentObjectName = itemName;
            SpriteFromAssetBundle(item);
        }
    }
    private void SpriteFromAssetBundle(string itemName)
    {
        // Load the sprites from the Asset Bundle
        Sprite loadedItemSprite = spriteAssetBundle.LoadAsset<Sprite>(itemName);
        if (loadedItemSprite == null)
        {
            Debug.LogError("Failed to load sprite: " + itemName);
            return;
        }
        itemSprite.sprite = loadedItemSprite;
        itemSprite.enabled = true;
        itemStored = true;
    }
}
