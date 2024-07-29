using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameObjectNamePair
{
    public GameObject gameObject;
    public string gameObjectName;
}

public class AvallibleGameObjectSelector : MonoBehaviour
{
    // Code Being used to handle mutiple versions of interactables on the same prefab


    [SerializeField] private List<GameObjectNamePair> targetGameObjects = new List<GameObjectNamePair>();

    private void OnEnable()
    {
        // Disable all GameObjects initially
        foreach (GameObjectNamePair pair in targetGameObjects)
        {
            pair.gameObject.SetActive(false);
        }

        // Enable a random GameObject
        if (targetGameObjects.Count > 0)
        {
            int randomIndex = Random.Range(0, targetGameObjects.Count);
            targetGameObjects[randomIndex].gameObject.SetActive(true);
        }
    }
}
