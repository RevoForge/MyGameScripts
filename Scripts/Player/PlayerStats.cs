using Revo.Methods;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using static Revo.Methods.SoundController;

public class PlayerStats : MonoBehaviour
{
    public int startingHealth = 100;
    [Header("UI Attribute Points Buttons")]
    public GameObject points1;
    public GameObject points2;
    public GameObject points3;

    [Header("UI References")]
    public TextMeshProUGUI textMeshHealth;
    public TextMeshProUGUI textMeshSTR;
    public TextMeshProUGUI textMeshCON;
    public TextMeshProUGUI textMeshDEX;
    public TextMeshProUGUI textMeshEXP;
    public TextMeshProUGUI textMeshNeedEXP;
    public TextMeshProUGUI textMeshPoints;
    public TextMeshProUGUI textMeshLevel;
    public GameObject healthObject;
    public float maxHealthScale = 0.025f;
    private readonly float minScale = 0f;
    public Slider experienceSlider;

    private AudioSource soundSource;
    public NamedAudioClip[] playerSounds = new NamedAudioClip[3]
{
        new NamedAudioClip { name = "Death", clip = null },
        new NamedAudioClip { name = "Level Up", clip = null },
        new NamedAudioClip { name = "First Time Death", clip = null }
};
    private PickupObjectScript[] heldPickupObject;
    private PickupObjectsPhysics[] heldPhysicsPickupObject;
    [HideInInspector] public bool hasSeenItemOI;
    [HideInInspector] public bool hasDiedOnce;
    [HideInInspector] public bool seatedToggle;
    private int _currentHealth;
    public int CurrentHealth
    {
        get { return _currentHealth; }
        set
        {
            if (_currentHealth != value)
            {
                _currentHealth = value;
                textMeshHealth.text = _currentHealth.ToString();
            }
        }
    }
    private int _strength = 1;
    public int Strength
    {
        get { return _strength; }
        set
        {
            if (_strength != value)
            {
                _strength = value;
                textMeshSTR.text = _strength.ToString();
            }
        }
    }
    private int _dexterity = 1;
    public int Dexterity
    {
        get { return _dexterity; }
        set
        {
            if (_dexterity != value)
            {
                _dexterity = value;
                textMeshDEX.text = _dexterity.ToString();
            }
        }
    }
    private int _constitution = 1;
    public int Constitution
    {
        get { return _constitution; }
        set
        {
            if (_constitution != value)
            {
                _constitution = value;
                textMeshCON.text = _constitution.ToString();
            }
        }
    }
    private int _level = 1;
    public int Level
    {
        get { return _level; }
        set
        {
            if (_level != value)
            {
                _level = value;
                textMeshLevel.text = _level.ToString();
            }
        }
    }
    [Header("EXP Settings")]
    public int pointsPerLevel = 3;
    private int _availablePoints;
    public int AvailablePoints
    {
        get { return _availablePoints; }
        set
        {
            if (_availablePoints != value)
            {
                _availablePoints = value;
                textMeshPoints.text = _availablePoints.ToString();

                if (_availablePoints > 0)
                {
                    points1.SetActive(true);
                    points2.SetActive(true);
                    points3.SetActive(true);
                }
                else if (AvailablePoints <= 0)
                {
                    points1.SetActive(false);
                    points2.SetActive(false);
                    points3.SetActive(false);
                }
            }
        }
    }
    private int _experiencePoints;
    public int ExperiencePoints
    {
        get { return _experiencePoints; }
        set
        {
            _experiencePoints = value;
            textMeshEXP.text = _experiencePoints.ToString();
            UpdateSlider();
        }
    }
    public int baseExperienceToLevelUp = 100;
    public float experienceIncreaseMultiplier = 1.5f;
    private PlayerController playerController;
    private bool dead;
    [HideInInspector] public int avaliblemaps = 0;
    private bool healthIsRegening = false;
    [HideInInspector] public bool mobsAreClose = false;
    [HideInInspector] public int maxhealth;
    SteamVR_Action_Vibration playerHitAction;

    void OnEnable()
    {
        soundSource = GetComponent<AudioSource>();
        playerController = GetComponent<PlayerController>();
        playerHitAction = SteamVR_Input.GetVibrationAction("PlayerHit");
        CurrentHealth = startingHealth + SetHealth();
        AvailablePoints = pointsPerLevel;
        UpdateSlider();
        textMeshNeedEXP.text = GetRequiredExperienceForNextLevel().ToString();
        MonsterStats.MonsterDeathEvent += OnMonsterDeath;
        maxhealth = startingHealth + SetHealth();

    }

    void OnDisable()
    {
        MonsterStats.MonsterDeathEvent -= OnMonsterDeath;
    }

    public void StatsLoaded()
    {
        CurrentHealth = startingHealth + SetHealth();
        textMeshNeedEXP.text = GetRequiredExperienceForNextLevel().ToString();
        maxhealth = startingHealth + SetHealth();
        playerController.HandleSeated(seatedToggle);
    }

    void Update()
    {
        if ((CurrentHealth < maxhealth) && !mobsAreClose && !healthIsRegening)
        {
            healthIsRegening = true;
            StartCoroutine(RegenerateHealth());
        }

    }

    public void TakeDamage(int damage)
    {
        if (!dead)
        {
            int reducedDamage = damage - ReduceDamage();
            if (reducedDamage > 0)
            {
                reducedDamage += 1;
                CurrentHealth -= reducedDamage;
                float scaleFactor = Mathf.Lerp(minScale, maxHealthScale, (float)CurrentHealth / maxhealth);
                healthObject.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
            }

            playerHitAction.Execute(0, 0.25f, 10, 1, SteamVR_Input_Sources.LeftHand);
            playerHitAction.Execute(0, 0.25f, 10, 1, SteamVR_Input_Sources.RightHand);

            if (CurrentHealth <= 0)
            {
                SetDeadTrue();
                Die();
            }
        }
    }


    private void SetDeadTrue()
    {
        dead = true;
        StartCoroutine(ResetBoolAfterDelay());
    }

    private IEnumerator ResetBoolAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        dead = false;
    }

    public int DealDamage()
    {
        Dictionary<int, int> multipliers = new()
{
    { 10, 10 },
    { 20, 9 },
    { 30, 8 },
    { 40, 7 },
    { 50, 6 },
    { 60, 5 },
    { 70, 4 },
    { 80, 3 },
    { 90, 2 },
    { int.MaxValue, 1 }
};

        return StatSystem.CalculateStat(Strength, multipliers);
    }

    public int SetHealth()
    {
        Dictionary<int, int> multipliers = new()
{
    { 10, 20 },
    { 20, 18 },
    { 30, 16 },
    { 40, 14 },
    { 50, 12 },
    { 60, 10 },
    { 70, 8 },
    { 80, 6 },
    { 90, 4 },
    { int.MaxValue, 2 }
};

        return StatSystem.CalculateStat(Constitution, multipliers);
    }

    public int ReduceDamage()
    {
        Dictionary<int, int> multipliers = new()
{
    { 10, 10 },
    { 20, 9 },
    { 30, 8 },
    { 40, 7 },
    { 50, 6 },
    { 60, 5 },
    { 70, 4 },
    { 80, 3 },
    { 90, 2 },
    { int.MaxValue, 1 }
};

        return StatSystem.CalculateStat(Dexterity, multipliers);
    }



    public void AttackMonster(MonsterStats monsterStats, int weapondmg, bool isranged, int attacktype)
    {
        int damage = DealDamage() + weapondmg;
        monsterStats.TakeDamage(damage, isranged, gameObject, attacktype);
    }

    public void GainExperience(int amount)
    {
        ExperiencePoints += amount;
        CheckLevelUp();
    }
    private void OnMonsterDeath(int experience, bool isboss)
    {
        GainExperience(experience);
    }

    private void CheckLevelUp()
    {
        int requiredExperience = GetRequiredExperienceForNextLevel();

        if (ExperiencePoints >= requiredExperience)
        {
            LevelUp();
        }
    }

    private int GetRequiredExperienceForNextLevel()
    {
        return Mathf.RoundToInt(baseExperienceToLevelUp * Mathf.Pow(experienceIncreaseMultiplier, Level - 1));
    }

    public void LevelUp()
    {
        int totalExperience = ExperiencePoints;
        int numLevels = 0;

        while (totalExperience >= GetRequiredExperienceForNextLevel())
        {
            int requiredExperience = GetRequiredExperienceForNextLevel();

            totalExperience -= requiredExperience;
            Level++;
            AvailablePoints += pointsPerLevel;
            numLevels++;
        }
        int leftoverExperience = totalExperience;
        ExperiencePoints = leftoverExperience;
        soundSource.clip = playerSounds[1].clip;
        soundSource.pitch = 1f;
        soundSource.Play();
        UpdateSlider();
        textMeshNeedEXP.text = GetRequiredExperienceForNextLevel().ToString();
    }

    public void PassMap(int passed)
    {
        avaliblemaps = passed;
    }

    public void IncreaseAvaliblePoints()
    {
        AvailablePoints += 1;
    }

    public void IncreaseStrength()
    {
        if (AvailablePoints >= 1)
        {
            AvailablePoints--;
            Strength++;
            if (Strength % 5 == 0)
            {
                Dexterity++;
            }
        }
    }

    public void IncreaseDexterity()
    {
        if (AvailablePoints >= 1)
        {
            AvailablePoints--;
            Dexterity++;
            if (Dexterity % 5 == 0)
            {
                AvailablePoints++;
                IncreaseConstitution();
            }
        }
    }

    public void IncreaseConstitution()
    {
        if (AvailablePoints >= 1)
        {
            AvailablePoints--;
            int previousHealth = SetHealth();
            Constitution++;
            if (Constitution % 5 == 0)
            {
                Strength++;
            }
            int newHealth = SetHealth();
            int healthIncrease = newHealth - previousHealth;
            CurrentHealth += healthIncrease;
            maxhealth = startingHealth + newHealth;
        }
    }


    private IEnumerator RegenerateHealth()
    {
        while (CurrentHealth < maxhealth)
        {
            if (!mobsAreClose)
            {
                int oldHealth = CurrentHealth;
                CurrentHealth += 1 + (int)Math.Round(Constitution * 0.75);

                if (oldHealth != CurrentHealth)
                {
                    float scaleFactor = Mathf.Lerp(minScale, maxHealthScale, (float)CurrentHealth / maxhealth);
                    healthObject.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
                }
                yield return new WaitForSeconds(1f);
            }
            else
            {
                healthIsRegening = false;
                yield break;
            }
        }
        CurrentHealth = maxhealth;
        healthIsRegening = false;
    }


    private void Die()
    {
        ExperiencePoints = 0;
        heldPickupObject = GetComponentsInChildren<PickupObjectScript>();
        if (heldPickupObject != null)
        {
            foreach (var obj in heldPickupObject)
            {
                obj.PlayerDeath();
            }
        }
        heldPhysicsPickupObject = GetComponentsInChildren<PickupObjectsPhysics>();
        if (heldPhysicsPickupObject != null)
        {
            foreach (var obj in heldPickupObject)
            {
                obj.PlayerDeath();
            }
        }
        soundSource.clip = playerSounds[0].clip;
        soundSource.pitch = 1.25f;
        soundSource.Play();
        playerController.ResetPositionAndRotation();
        CurrentHealth = startingHealth + SetHealth();
        float scaleFactor = Mathf.Lerp(minScale, maxHealthScale, (float)CurrentHealth / maxhealth);
        healthObject.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
        if (!hasDiedOnce)
        {
            soundSource.clip = playerSounds[2].clip;
            soundSource.Play();
            hasDiedOnce = true;
        }
    }

    private void UpdateSlider()
    {
        if (experienceSlider != null)
        {
            experienceSlider.gameObject.SetActive(true);
            float requiredExp = GetRequiredExperienceForNextLevel();
            float sliderValue = ExperiencePoints / requiredExp;
            experienceSlider.value = sliderValue;
            StartCoroutine(DisableSliderAfterDelay(15f));
        }
    }
    private IEnumerator DisableSliderAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (experienceSlider != null)
        {
            experienceSlider.gameObject.SetActive(false);
        }
    }
}