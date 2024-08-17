using System.Collections;
using UnityEngine;

public class AssetBundleManager : MonoBehaviour
{
    private static AssetBundleManager instance;
    private AssetBundle weaponAssetBundle;
    private AssetBundle spriteAssetBundle;

    public static AssetBundleManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AssetBundleManager>();
                if (instance == null)
                {
                    GameObject go = new("AssetBundleManager");
                    instance = go.AddComponent<AssetBundleManager>();
                }
            }
            return instance;
        }
    }
    private void Start()
    {
        StartCoroutine(LoadAssetBundle1Async());
        StartCoroutine(LoadAssetBundle2Async());
    }
    private IEnumerator LoadAssetBundle1Async()
    {
        AssetBundleCreateRequest spriteBundleRequest = AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath + "/AssetBundles/sprites");
        yield return spriteBundleRequest;
        if (spriteBundleRequest.isDone)
        {
            spriteAssetBundle = spriteBundleRequest.assetBundle;
        }
        else
        {
            Debug.LogError("Failed to load Asset Bundle: sprites");
        }
    }
    private IEnumerator LoadAssetBundle2Async()
    {
        AssetBundleCreateRequest weaponBundleRequest = AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath + "/AssetBundles/weapons");
        yield return weaponBundleRequest;
        if (weaponBundleRequest.isDone)
        {
            weaponAssetBundle = weaponBundleRequest.assetBundle;
        }
        else
        {
            Debug.LogError("Failed to load Asset Bundle: weapons");
        }
    }
    public AssetBundle GetWeaponAssetBundle()
    {
        return weaponAssetBundle;
    }
    public AssetBundle GetSpriteAssetBundle()
    {
        return spriteAssetBundle;
    }
    public void UnloadAssetBundles()
    {
        if (weaponAssetBundle != null)
        {
            weaponAssetBundle.Unload(false);
        }

        if (spriteAssetBundle != null)
        {
            spriteAssetBundle.Unload(false);
        }
    }
}
