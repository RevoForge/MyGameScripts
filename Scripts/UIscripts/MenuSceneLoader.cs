using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuSceneLoader : MonoBehaviour
{
    public string loadMenu;
    private AsyncOperation loadingOperation;
    public GameObject loadingBar;
    public Image loadingImage;
    public GameObject turnOffForLoading;
    private int sceneIndex;
    private SettingsLoader settings;

    public void LoadScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        sceneIndex = currentScene.buildIndex;
        GameObject database = GameObject.Find("DataBase");
        settings = database.GetComponent<SettingsLoader>();
        settings.SaveChangedSettings(sceneIndex);
        turnOffForLoading.SetActive(false);
        loadingBar.SetActive(true);
        loadingOperation = SceneManager.LoadSceneAsync(loadMenu);
    }

    void Update()
    {
        if (loadingOperation != null)
            loadingImage.fillAmount = Mathf.Clamp01(loadingOperation.progress / 0.9f);
    }
}
