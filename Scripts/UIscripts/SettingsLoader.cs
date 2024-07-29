using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsLoader : MonoBehaviour
{
    private SettingsDatabase settingsDatabase;
    private AudioSource audioSource;
    private Slider soundSlider;
    private Slider monsterSoundSlider;
    private int previousSceneIndex;
    private Toggle moveType;
    private Slider turnSpeedSlider;
    private float delay = 0.01f;
    private static SettingsLoader instance;
    private PlayerStats playerStats;
    private GameObject player;
    private GameObject slums;
    private GameObject forest;
    private GameObject dwarf;
    private GameObject dragon;
    private GameObject seated;
    private Toggle seatedToggle;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        forest = GameObject.Find("Forest");
        slums = GameObject.Find("Slums");
        dwarf = GameObject.Find("Dwarf");
        dragon = GameObject.Find("Dragon");
        seated = GameObject.Find("Seated");
        settingsDatabase = GetComponent<SettingsDatabase>();
        SceneManager.sceneLoaded += OnSceneLoaded;
        Invoke(nameof(LoadSceneSettings), delay);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        forest = GameObject.Find("Forest");
        slums = GameObject.Find("Slums");
        dwarf = GameObject.Find("Dwarf");
        dragon = GameObject.Find("Dragon");
        seated = GameObject.Find("Seated");
        Invoke(nameof(LoadSceneSettings), delay);
        previousSceneIndex = scene.buildIndex; // Update the previous scene index
        ItemSlotController itemSlotController = FindAnyObjectByType<ItemSlotController>();
        if (itemSlotController != null)
        {
            itemSlotController.LoadConfiguration();
        }
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerStats = player.GetComponent<PlayerStats>();
        }
    }

    public void SaveChangedSettings(int sceneIndex)
    {
        if (settingsDatabase != null)
        {
            float[] soundSettings = settingsDatabase.GetSoundSettings();
            soundSettings[sceneIndex] = soundSlider.value;
            PlayerStats playerStats = FindObjectOfType<PlayerStats>();
            // Add the check for smoothTurnObject's active state
            GameObject smoothTurnObject = GameObject.Find("smoothTurn");
            if (smoothTurnObject != null && smoothTurnObject.activeSelf)
            {
                settingsDatabase.SaveSettings(true, turnSpeedSlider.value, soundSettings, playerStats.Level, playerStats.Strength, playerStats.Constitution, playerStats.Dexterity, playerStats.ExperiencePoints, playerStats.AvailablePoints, playerStats.avaliblemaps, false, playerStats.hasDiedOnce, playerStats.hasSeenItemOI, playerStats.seatedToggle);
            }
            else if (sceneIndex != 0)
            {
                settingsDatabase.SaveSettings(false, settingsDatabase.GetSmoothMovementSpeed(), soundSettings, playerStats.Level, playerStats.Strength, playerStats.Constitution, playerStats.Dexterity, playerStats.ExperiencePoints, playerStats.AvailablePoints, playerStats.avaliblemaps, false, playerStats.hasDiedOnce, playerStats.hasSeenItemOI, playerStats.seatedToggle);
            }
            if (sceneIndex == 0)
            {
                settingsDatabase.SaveSettings(settingsDatabase.GetMovementType(), settingsDatabase.GetSmoothMovementSpeed(), soundSettings, settingsDatabase.GetLevel(), settingsDatabase.GetSTR(), settingsDatabase.GetCON(), settingsDatabase.GetDEX(), settingsDatabase.GetEXP(), settingsDatabase.GetAPP(), settingsDatabase.GetMAP(), false, settingsDatabase.GetDied(), settingsDatabase.Getitemoi(), seatedToggle.isOn);
            }
        }
    }

    public void EraseProgress()
    {
        if (settingsDatabase != null)
        {
            float[] soundSettings = settingsDatabase.GetSoundSettings();
            soundSettings[0] = soundSlider.value;
            settingsDatabase.SaveSettings(settingsDatabase.GetMovementType(), settingsDatabase.GetSmoothMovementSpeed(), soundSettings, 1, 1, 1, 1, 0, 2, 0, true, false, false, false);
            LoadSceneSettings();
        }
    }

    public void RefundPoints(int sceneIndex)
    {
        if (settingsDatabase != null)
        {
            float[] soundSettings = settingsDatabase.GetSoundSettings();
            soundSettings[sceneIndex] = soundSlider.value;
            PlayerStats playerStats = FindObjectOfType<PlayerStats>();
            GameObject smoothTurnObject = GameObject.Find("smoothTurn");
            int bonuspoints = (playerStats.Strength / 5 + playerStats.Constitution / 5 + playerStats.Dexterity / 5);
            int refundedpoints = (playerStats.Strength + playerStats.Constitution + playerStats.Dexterity + playerStats.AvailablePoints - bonuspoints);
            int refendedHealth = playerStats.SetHealth();
            if (smoothTurnObject != null && smoothTurnObject.activeSelf)
            {
                settingsDatabase.SaveSettings(true, turnSpeedSlider.value, soundSettings, playerStats.Level, 0, 0, 0, playerStats.ExperiencePoints, refundedpoints, playerStats.avaliblemaps, false, settingsDatabase.GetDied(), settingsDatabase.Getitemoi(), settingsDatabase.GetSeated());
            }
            else
            {
                settingsDatabase.SaveSettings(false, settingsDatabase.GetSmoothMovementSpeed(), soundSettings, playerStats.Level, 0, 0, 0, playerStats.ExperiencePoints, refundedpoints, playerStats.avaliblemaps, false, settingsDatabase.GetDied(), settingsDatabase.Getitemoi(), settingsDatabase.GetSeated());
            }
            playerStats.Strength = 0;
            playerStats.Constitution = 0;
            playerStats.Dexterity = 0;
            playerStats.AvailablePoints = refundedpoints;
            playerStats.CurrentHealth -= refendedHealth;
            playerStats.maxhealth = playerStats.startingHealth;

        }
    }

    private void OnApplicationQuit()
    {
        SaveChangedSettings(previousSceneIndex);
        settingsDatabase.CloseConcection();
    }

    private void LoadSceneSettings()
    {
        bool movementType = settingsDatabase.GetMovementType();
        float smoothMovementSpeed = settingsDatabase.GetSmoothMovementSpeed();
        float[] soundSettings = settingsDatabase.GetSoundSettings();
        int level = settingsDatabase.GetLevel();
        int str = settingsDatabase.GetSTR();
        int con = settingsDatabase.GetCON();
        int dex = settingsDatabase.GetDEX();
        int exp = settingsDatabase.GetEXP();
        int app = settingsDatabase.GetAPP();
        bool died = settingsDatabase.GetDied();
        bool itemio = settingsDatabase.Getitemoi();
        bool seatedtoggle = settingsDatabase.GetSeated();

        Scene currentScene = SceneManager.GetActiveScene();
        int sceneIndex = currentScene.buildIndex;

        if (sceneIndex >= 0 && sceneIndex < soundSettings.Length)
        {
            float selectedSoundSetting = soundSettings[sceneIndex];

            if (sceneIndex == 0)
            {
                int map = settingsDatabase.GetMAP();
                seatedToggle = seated.GetComponent<Toggle>();
                seatedToggle.isOn = seatedtoggle;

                GameObject uniStormObject = GameObject.Find("UniStorm VR System");
                if (uniStormObject != null)
                {
                    Thunderstormloader thunderstormLoader = uniStormObject.GetComponent<Thunderstormloader>();
                    if (thunderstormLoader != null)
                    {
                        thunderstormLoader.SetVolume(selectedSoundSetting);
                    }
                }

                // Update Sound_Slider value
                GameObject sliderObject = GameObject.Find("Sound_Slider");
                if (sliderObject != null)
                {
                    soundSlider = sliderObject.GetComponent<Slider>();
                    if (soundSlider != null)
                    {
                        soundSlider.value = selectedSoundSetting;
                    }
                }
                switch (map)
                {
                    case 0:
                        slums.SetActive(false);
                        goto case 1;
                    case 1:
                        forest.SetActive(false);
                        goto case 2;
                    case 2:
                        dwarf.SetActive(false);
                        goto case 3;
                    case 3:
                        dragon.SetActive(false);
                        break;
                }
            }

            if (sceneIndex != 0)
            {
                // Update Sound_Slider value
                GameObject soundSliderObject = GameObject.Find("Sound_Slider");
                if (soundSliderObject != null)
                {
                    soundSlider = soundSliderObject.GetComponent<Slider>();
                    if (soundSlider != null)
                    {
                        soundSlider.value = selectedSoundSetting;
                    }
                }

                // Update Monster_Sound_Slider value
                GameObject monsterSoundSliderObject = GameObject.Find("Monster_Sound_Slider");
                if (monsterSoundSliderObject != null)
                {
                    monsterSoundSlider = monsterSoundSliderObject.GetComponent<Slider>();
                    if (monsterSoundSlider != null)
                    {
                        monsterSoundSlider.value = soundSettings[6];
                    }
                }

                // Update Turn_Speed value
                GameObject turnSpeedObject = GameObject.Find("Turn_Speed");
                if (turnSpeedObject != null)
                {
                    turnSpeedSlider = turnSpeedObject.GetComponent<Slider>();
                    if (turnSpeedSlider != null)
                    {
                        turnSpeedSlider.value = smoothMovementSpeed;
                    }
                }

                // Update Ambiance audio volume
                GameObject ambianceObject = GameObject.Find("Ambiance");
                if (ambianceObject != null)
                {
                    audioSource = ambianceObject.GetComponent<AudioSource>();
                    if (audioSource != null)
                    {
                        audioSource.volume = selectedSoundSetting;
                    }
                }

                // Update movement type and smooth turn/snap turn objects
                GameObject smoothTurnObject = GameObject.Find("smoothTurn");
                GameObject snapTurnObject = GameObject.Find("snapTurn");
                GameObject moveTypeObject = GameObject.Find("TurnType");
                if (moveTypeObject != null)
                {
                    moveType = moveTypeObject.GetComponent<Toggle>();
                }

                if (smoothTurnObject != null && snapTurnObject != null)
                {
                    moveType.isOn = movementType;
                    smoothTurnObject.SetActive(movementType);
                    snapTurnObject.SetActive(!movementType);
                }

                // Update smooth turn rotation speed
                GameObject smoothTurnSpeed = GameObject.Find("smoothTurn");
                if (smoothTurnSpeed != null)
                {
                    if (smoothTurnSpeed.activeSelf)
                    {
                        SmoothTurn scriptComponent = smoothTurnSpeed.GetComponent<SmoothTurn>();
                        if (scriptComponent != null)
                        {
                            scriptComponent.SetRotationSpeed(smoothMovementSpeed);
                        }
                    }
                }

                // Update UI elements
                GameObject menuUI = GameObject.Find("Menu_Canvas");

                if (turnSpeedObject != null)
                {
                    turnSpeedObject.SetActive(movementType);
                }
                if (menuUI != null)
                {
                    InventorySlot[] inventorySlots = FindObjectsOfType<InventorySlot>();
                    for (var i = 0; i < inventorySlots.Length; i++)
                    {
                        inventorySlots[i].LoadConfiguration();
                    }
                    menuUI.SetActive(false);
                }

                PlayerStats playerStats = FindObjectOfType<PlayerStats>();
                if (playerStats != null)
                {
                    playerStats.Level = level;
                    playerStats.Strength = str;
                    playerStats.Constitution = con;
                    playerStats.Dexterity = dex;
                    playerStats.ExperiencePoints = exp;
                    playerStats.AvailablePoints = app;
                    playerStats.hasDiedOnce = died;
                    playerStats.hasSeenItemOI = itemio;
                    playerStats.seatedToggle = seatedtoggle;
                }
            }
            if (playerStats != null)
            {
                playerStats.StatsLoaded();
            }
        }
    }
}