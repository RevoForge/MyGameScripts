using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnTrigger : MonoBehaviour
{
    private SettingsLoader settings;

    void Start()
    {
        GameObject database = GameObject.Find("DataBase");
        settings = database.GetComponent<SettingsLoader>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            settings.SaveChangedSettings(0);

            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            int nextSceneIndex = (currentSceneIndex + 1) % SceneManager.sceneCountInBuildSettings;

            SceneManager.LoadSceneAsync(nextSceneIndex);
        }
    }
}
