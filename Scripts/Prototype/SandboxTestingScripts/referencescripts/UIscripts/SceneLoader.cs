using Revo.Methods;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public string one;
    public string two;
    public string three;
    public string four;
    public string five;
    public string six;
    private AsyncOperation loadingOperation;
    public GameObject loadingBar;
    public Image loadingBarFill;
    public GameObject buttons;
    private SettingsLoader settings;

    void OnEnable()
    {
        GameObject database = GameObject.Find("DataBase");
        settings = database.GetComponent<SettingsLoader>();
    }
    public void SceneOne()
    {
        SceneController.LoadLevelScene(settings, buttons, loadingBar);
        loadingOperation = SceneManager.LoadSceneAsync(one);
    }
    public void SceneTwo()
    {
        SceneController.LoadLevelScene(settings, buttons, loadingBar);
        loadingOperation = SceneManager.LoadSceneAsync(two);
    }
    public void SceneThree()
    {
        SceneController.LoadLevelScene(settings, buttons, loadingBar);
        loadingOperation = SceneManager.LoadSceneAsync(three);
    }
    public void SceneFour()
    {
        SceneController.LoadLevelScene(settings, buttons, loadingBar);
        loadingOperation = SceneManager.LoadSceneAsync(four);
    }
    public void SceneFive()
    {
        SceneController.LoadLevelScene(settings, buttons, loadingBar);
        loadingOperation = SceneManager.LoadSceneAsync(five);
    }
    public void SceneSix()
    {
        SceneController.LoadLevelScene(settings, buttons, loadingBar);
        loadingOperation = SceneManager.LoadSceneAsync(six);
    }

    void Update()
    {
        if (loadingOperation != null)
            loadingBarFill.fillAmount = Mathf.Clamp01(loadingOperation.progress / 0.9f);
    }
}
