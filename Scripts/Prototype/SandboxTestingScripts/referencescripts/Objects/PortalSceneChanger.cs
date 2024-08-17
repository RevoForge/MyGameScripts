using UnityEngine;
using UnityEngine.SceneManagement;
using static Revo.Methods.SoundController;

public class MonsterDeathCounter : MonoBehaviour
{
    public int deathThreshold = 10;
    public MonsterSpawn bossSpawn;
    private int deathCount = 0;
    private Transform portal;
    private BoxCollider trigger;
    private AudioSource notificationSound;
    private PlayerStats playerStats;
    private bool portalOpen = false;
    private SettingsLoader settings;
    private bool bossSpawned;
    private ItemSlotController inventoryController;

    public NamedAudioClip[] playerSounds = new NamedAudioClip[3]
    {
        new() { name = "BossSpawned", clip = null },
        new() { name = "PortalOpen", clip = null },
        new() { name = "GameFinished", clip = null }
    };

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            settings.SaveChangedSettings(0);
            inventoryController.SaveConfiguration();
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            int nextSceneIndex = (currentSceneIndex + 1) % SceneManager.sceneCountInBuildSettings;

            SceneManager.LoadSceneAsync(nextSceneIndex);
        }
    }

    private void OnEnable()
    {
        GameObject database = GameObject.Find("DataBase");
        settings = database.GetComponent<SettingsLoader>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerStats = player.GetComponent<PlayerStats>();
        }
        MonsterStats.MonsterDeathEvent += HandleMonsterDeath;
        portal = transform.Find("Portal");
        portal.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        trigger = GetComponent<BoxCollider>();
        notificationSound = GetComponent<AudioSource>();
        bossSpawned = false;
        inventoryController = FindAnyObjectByType<ItemSlotController>();


    }

    private void OnDisable()
    {
        MonsterStats.MonsterDeathEvent -= HandleMonsterDeath;
    }

    int GetCurrentSceneIndex()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        return currentScene.buildIndex;
    }

    private void HandleMonsterDeath(int experience, bool isBoss)
    {
        deathCount++;

        if ((deathCount >= deathThreshold) && !bossSpawned)
        {
            bossSpawn.SpawnBoss();
            bossSpawned = true;
            notificationSound.clip = playerSounds[0].clip;
            notificationSound.Play();
        }

        if (isBoss && portalOpen == false)
        {
            portalOpen = true;
            portal.transform.localScale = new Vector3(1f, 1f, 1f);
            trigger.enabled = true;
            if (GetCurrentSceneIndex() <= 4)
            {
                notificationSound.clip = playerSounds[1].clip;
            }
            else
            {
                notificationSound.clip = playerSounds[2].clip;
            }
            notificationSound.Play();
            playerStats.PassMap(GetCurrentSceneIndex());
        }
    }
}
