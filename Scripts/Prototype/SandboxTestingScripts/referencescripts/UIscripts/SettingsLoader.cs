using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsLoader : MonoBehaviour
{
    private SettingsDatabase settingsDatabase;
    public Settings currentSettings;
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
    private GameObject orc;
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
        orc = GameObject.Find("Orc");
        settingsDatabase = GetComponent<SettingsDatabase>();
        currentSettings = settingsDatabase.LoadSettings();
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
        orc = GameObject.Find("Orc");
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
            if (sceneIndex == 0)
            {
                // This will be defunct after we change from the Prototype Main Menu
                // Exsists because PlayerStats does not exist in the Prototype Main Menu
                settingsDatabase.SaveSettings(
                    currentSettings.MovementType,
                    currentSettings.SmoothMovementSpeed,
                    soundSettings,
                    currentSettings.CharecterLevel,
                    currentSettings.STR,
                    currentSettings.INT,
                    currentSettings.WIS,
                    currentSettings.DEF,
                    currentSettings.CON,
                    currentSettings.CharecterExp,
                    currentSettings.AvailableCharacterPoints,
                    currentSettings.AvailableMaps,
                    currentSettings.PhysicalSkillLevel,
                    currentSettings.PhysicalSkillExp,
                    currentSettings.PhysicalSkillLocked,
                    currentSettings.MagicalSkillLevel,
                    currentSettings.MagicalSkillExp,
                    currentSettings.MagicalSkillLocked,
                    currentSettings.UtilitySkillLevel,
                    currentSettings.UtilitySkillExp,
                    currentSettings.UtilitySkillLocked,
                    currentSettings.AvalibleSkillPoints,
                    currentSettings.HasDiedOnce,
                    currentSettings.HasSeenItemOI,
                    seatedToggle.isOn
                );
                //---------------------------------------------
            }
            else
            {
                settingsDatabase.SaveSettings(
                // movementType
                currentSettings.MovementType,
                // smoothMovementSpeed
                turnSpeedSlider.value,
                // soundSettings
                soundSettings,
                // Charecter level
                playerStats.Level,
                // strength
                playerStats.Strength,
                // intelligence
                playerStats.Intelligence,
                // wisdom
                playerStats.Wisdom,
                // defense
                playerStats.Defence,
                // constitution
                playerStats.Constitution,
                // charecterExp
                playerStats.ExperiencePoints,
                // Avlable character points
                playerStats.AvailablePoints,
                // What maps are avalible
                playerStats.AvalibleMaps,
                // Physical skill lvl
                playerStats.PhysicalSkillLevel,
                // Physical skill exp
                playerStats.PhysicalSkillExp,
                // Are Physical skills locked
                playerStats.PhysicalSkillLocked,
                // Magical skill lvl
                playerStats.MagicalSkillLevel,
                // Magical skill exp
                playerStats.MagicalSkillExp,
                // Are Magical skills locked
                playerStats.MagicalSkillLocked,
                // Utility skill lvl
                playerStats.UtilitySkillLevel,
                // Utility skill exp
                playerStats.UtilitySkillExp,
                // Are utility skills locked
                playerStats.UtilitySkillLocked,
                // Avalible skill points
                playerStats.AvalibleSkillPoints,
                // Dont play player first death sound
                playerStats.hasDiedOnce,
                // item of interest check
                playerStats.hasSeenItemOI,
                // Does the player have seated mode on
                seatedToggle.isOn
                );
            }
            // update current reference
            currentSettings = settingsDatabase.settings;
        }
    }
    public void EraseProgress()
    {
        if (settingsDatabase != null)
        {
            float[] soundSettings = settingsDatabase.GetSoundSettings();
            soundSettings[0] = soundSlider.value;
            currentSettings = settingsDatabase.ResetGameSave();
            LoadSceneSettings();
        }
    }
    public void RefundPoints()
    {
        if (settingsDatabase != null)
        {
            int bonuspoints = (playerStats.Strength / 5 + playerStats.Intelligence / 5 + playerStats.Wisdom / 5 + playerStats.Defence / 5 + playerStats.Constitution / 5);
            int refundedpoints = (playerStats.Strength + playerStats.Intelligence + playerStats.Wisdom + playerStats.Defence + playerStats.Constitution + playerStats.AvailablePoints - bonuspoints);
            int refendedHealth = playerStats.SetHealth();
            playerStats.Strength = 0;
            playerStats.Intelligence = 0;
            playerStats.Wisdom = 0;
            playerStats.Constitution = 0;
            playerStats.Defence = 0;
            playerStats.AvailablePoints = refundedpoints;
            playerStats.maxhealth = playerStats.startingHealth;
            playerStats.CurrentHealth -= refendedHealth;
        }
    }
    // Prototype
    public void RefundSkillLevelPoints()
    {
        if (settingsDatabase != null)
        {
            // Sum up all the elements in the int[] arrays
            int totalPhysicalSkillLevel = playerStats.PhysicalSkillLevel.Sum();
            int totalMagicalSkillLevel = playerStats.MagicalSkillLevel.Sum();
            int totalUtilitySkillLevel = playerStats.UtilitySkillLevel.Sum();
            // Reset the int[] arrays
            playerStats.PhysicalSkillLevel = new int[playerStats.PhysicalSkillLevel.Length];
            playerStats.MagicalSkillLevel = new int[playerStats.MagicalSkillLevel.Length];
            playerStats.UtilitySkillLevel = new int[playerStats.UtilitySkillLevel.Length];
            // Add the sums of all three arrays to AvalibleSkillPoints
            playerStats.AvalibleSkillPoints += totalPhysicalSkillLevel + totalMagicalSkillLevel + totalUtilitySkillLevel;
        }
    }

    private void OnApplicationQuit()
    {
        SaveChangedSettings(previousSceneIndex);
    }

    private void LoadSceneSettings()
    {
        float[] soundSettings = settingsDatabase.GetSoundSettings();
        Scene currentScene = SceneManager.GetActiveScene();
        int sceneIndex = currentScene.buildIndex;

        if (sceneIndex >= 0 && sceneIndex < soundSettings.Length)
        {
            float selectedSoundSetting = soundSettings[sceneIndex];
            GameObject soundSliderObject = GameObject.Find("Sound_Slider");

            if (sceneIndex == 0)
            {
                seatedToggle = seated.GetComponent<Toggle>();
                seatedToggle.isOn = currentSettings.SeatedToggle;
                GameObject uniStormObject = GameObject.Find("UniStorm VR System");
                if (uniStormObject != null)
                {

                }
                // Update Sound_Slider value
                if (soundSliderObject != null)
                {
                    if (soundSliderObject.TryGetComponent(out soundSlider))
                    {
                        soundSlider.value = selectedSoundSetting;
                    }
                }
                switch (currentSettings.AvailableMaps)
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
                        goto case 4;
                    case 4:
                        orc.SetActive(false);
                        break;
                }
            }
            else
            {
                if (soundSliderObject != null)
                {
                    if (soundSliderObject.TryGetComponent(out soundSlider))
                    {
                        soundSlider.value = selectedSoundSetting;
                    }
                }

                // Update Monster_Sound_Slider value
                GameObject monsterSoundSliderObject = GameObject.Find("Monster_Sound_Slider");
                if (monsterSoundSliderObject != null)
                {
                    if (monsterSoundSliderObject.TryGetComponent(out monsterSoundSlider))
                    {
                        monsterSoundSlider.value = soundSettings[6];
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
                    moveType.isOn = currentSettings.MovementType;
                    smoothTurnObject.SetActive(moveType.isOn);
                    snapTurnObject.SetActive(moveType.isOn);
                }
                // Update Turn_Speed value on slider
                GameObject turnSpeedObject = GameObject.Find("Turn_Speed");
                if (turnSpeedObject != null)
                {
                    if (turnSpeedObject.TryGetComponent(out turnSpeedSlider))
                    {
                        turnSpeedSlider.value = currentSettings.SmoothMovementSpeed;
                    }
                }
                // Update smooth turn rotation speedof player
                if (smoothTurnObject != null)
                {
                    if (smoothTurnObject.activeSelf)
                    {
                        if (smoothTurnObject.TryGetComponent<SmoothTurn>(out var scriptComponent))
                        {
                            scriptComponent.SetRotationSpeed(currentSettings.SmoothMovementSpeed);
                        }
                    }
                }
                // Update Ambiance audio volume
                GameObject ambianceObject = GameObject.Find("Ambiance");
                if (ambianceObject != null)
                {
                    if (ambianceObject.TryGetComponent(out audioSource))
                    {
                        audioSource.volume = selectedSoundSetting;
                    }
                }
                // Update UI elements
                GameObject menuUI = GameObject.Find("Menu_Canvas");
                if (turnSpeedObject != null)
                {
                    turnSpeedObject.SetActive(currentSettings.MovementType);
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
                // Update Stats on the Player object
                if (playerStats != null)
                {
                    playerStats.Level = currentSettings.CharecterLevel;
                    playerStats.Strength = currentSettings.STR;
                    playerStats.Intelligence = currentSettings.INT;
                    playerStats.Defence = currentSettings.DEF;
                    playerStats.Wisdom = currentSettings.WIS;
                    playerStats.Constitution = currentSettings.CON;
                    playerStats.ExperiencePoints = currentSettings.CharecterExp;
                    playerStats.AvailablePoints = currentSettings.AvailableCharacterPoints;
                    playerStats.PhysicalSkillLevel = currentSettings.PhysicalSkillLevel;
                    playerStats.PhysicalSkillExp = currentSettings.PhysicalSkillExp;
                    playerStats.PhysicalSkillLocked = currentSettings.PhysicalSkillLocked;
                    playerStats.MagicalSkillLevel = currentSettings.MagicalSkillLevel;
                    playerStats.MagicalSkillExp = currentSettings.MagicalSkillExp;
                    playerStats.MagicalSkillLocked = currentSettings.MagicalSkillLocked;
                    playerStats.UtilitySkillLevel = currentSettings.UtilitySkillLevel;
                    playerStats.UtilitySkillExp = currentSettings.UtilitySkillExp;
                    playerStats.UtilitySkillLocked = currentSettings.UtilitySkillLocked;
                    playerStats.hasDiedOnce = currentSettings.HasDiedOnce;
                    playerStats.hasSeenItemOI = currentSettings.HasSeenItemOI;
                    playerStats.seatedToggle = currentSettings.SeatedToggle;
                }
            }
            if (playerStats != null)
            {
                playerStats.StatsLoaded();
            }
        }
    }
}