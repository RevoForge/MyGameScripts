using UnityEngine;
using UnityEngine.SceneManagement;

public class PointRefundButton : MonoBehaviour
{
    private SettingsLoader settingsLoader;
    private Scene currentScene;
    private int sceneIndex;

    private void OnEnable()
    {
        currentScene = SceneManager.GetActiveScene();
        sceneIndex = currentScene.buildIndex;
    }

    public void ClickedButton()
    {
        settingsLoader = FindObjectOfType<SettingsLoader>();
        if (settingsLoader != null)
        {
            settingsLoader.RefundPoints(sceneIndex);
        }
    }
}
