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
    public int startingResource = 100;
    [Header("UI Attribute Points Buttons")]
    public GameObject points1;
    public GameObject points2;
    public GameObject points3;
    public GameObject points4;
    public GameObject points5;

    [Header("UI References")]
    public TextMeshProUGUI textMeshHealth;
    public TextMeshProUGUI textMeshResource;
    public TextMeshProUGUI textMeshSTR;
    public TextMeshProUGUI textMeshINT;
    public TextMeshProUGUI textMeshWIS;
    public TextMeshProUGUI textMeshCON;
    public TextMeshProUGUI textMeshDEF;
    public TextMeshProUGUI textMeshEXP;
    public TextMeshProUGUI textMeshNeedEXP;
    public TextMeshProUGUI textMeshPoints;
    public TextMeshProUGUI textMeshSkillPoints;
    public TextMeshProUGUI textMeshLevel;
    public GameObject healthObject;
    public GameObject resourceObject;
    public float maxHealthScale = 0.025f;
    private readonly float minScale = 0f;
    public Slider experienceSlider;

    private AudioSource soundSource;
    public NamedAudioClip[] playerSounds = new NamedAudioClip[3]
{
        new() { name = "Death", clip = null },
        new() { name = "Level Up", clip = null },
        new() { name = "First Time Death", clip = null }
};
    private PickupObjectScript[] heldPickupObject;
    private PickupObjectsPhysics[] heldPhysicsPickupObject;
    [HideInInspector] public bool hasSeenItemOI;
    [HideInInspector] public bool hasDiedOnce;
    [HideInInspector] public bool seatedToggle;
    private int _currentSkillCost;
    public int CurrentSkillCost
    {
        get { return _currentSkillCost; }
        set
        {
            if (_currentSkillCost != value)
            {
                _currentSkillCost = value;
            }
        }
    }
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
    private int _currentResource;
    public int CurrentResource
    {
        get { return _currentResource; }
        set
        {
            if (_currentResource != value)
            {
                _currentResource = value;
                textMeshResource.text = _currentResource.ToString();
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
    private int _intelligence = 1;
    public int Intelligence
    {
        get { return _intelligence; }
        set
        {
            if (_intelligence != value)
            {
                _intelligence = value;
                textMeshINT.text = _intelligence.ToString();
            }
        }
    }
    private int _wisdom = 1;
    public int Wisdom
    {
        get { return _wisdom; }
        set
        {
            if (_wisdom != value)
            {
                _wisdom = value;
                textMeshWIS.text = _wisdom.ToString();
            }
        }
    }
    private int _defence = 1;
    public int Defence
    {
        get { return _defence; }
        set
        {
            if (_defence != value)
            {
                _defence = value;
                textMeshDEF.text = _defence.ToString();
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
    // Skill Variables
    //TODO: Need to implement skill system for the player
    public int[] PhysicalSkillLevel = new int[9];
    public int[] PhysicalSkillExp = new int[9];
    public bool[] PhysicalSkillLocked = new bool[9];
    public int[] MagicalSkillLevel = new int[9];
    public int[] MagicalSkillExp = new int[9];
    public bool[] MagicalSkillLocked = new bool[9];
    public int[] UtilitySkillLevel = new int[9];
    public int[] UtilitySkillExp = new int[9];
    public bool[] UtilitySkillLocked = new bool[9];
    private int _skillPoints = 1;
    public int AvalibleSkillPoints
    {
        get { return _skillPoints; }
        set
        {
            if (_skillPoints != value)
            {
                _skillPoints = value;
                textMeshSkillPoints.text = _skillPoints.ToString();
            }
        }
    }
        //-------------------
        [Header("EXP Settings")]
    private int _pointsPerLevel = 3;
    public int PointsPerLevel
    {
        get { return _pointsPerLevel; }
        set
        {
            if (_pointsPerLevel != value)
            {
                _pointsPerLevel = value;
            }
        }
    }
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
                    points4.SetActive(true);
                    points5.SetActive(true);
                }
                else if (AvailablePoints <= 0)
                {
                    points1.SetActive(false);
                    points2.SetActive(false);
                    points3.SetActive(false);
                    points4.SetActive(false);
                    points5.SetActive(false);
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
    [HideInInspector] public int AvalibleMaps = 0;
    private bool healthIsRegening = false;
    private bool resourceIsRegening = false;
    [HideInInspector] public bool mobsAreClose = false;
    [HideInInspector] public int maxhealth;
    [HideInInspector] public int maxResource;
    SteamVR_Action_Vibration playerHitAction;

    void OnEnable()
    {
        soundSource = GetComponent<AudioSource>();
        playerController = GetComponent<PlayerController>();
        playerHitAction = SteamVR_Input.GetVibrationAction("PlayerHit");
        CurrentHealth = startingHealth + SetHealth();
        AvailablePoints = _pointsPerLevel;
        UpdateSlider();
        textMeshNeedEXP.text = GetRequiredExperienceForNextLevel().ToString();
        MonsterStats.MonsterDeathEvent += OnMonsterDeath;
        maxhealth = startingHealth + SetHealth();
        maxResource = startingResource + SetResourcePool();
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
        if ((CurrentResource < maxResource) && !resourceIsRegening)
        {
            resourceIsRegening = true;
            StartCoroutine(RegenerateResource());
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

    public int DealDamageBasedOnStrength()
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
    public int DealSkillDamageBasedOnIntelligence()
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

        return StatSystem.CalculateStat(Intelligence, multipliers);
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

    public int SetResourcePool()
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

        return StatSystem.CalculateStat(Wisdom, multipliers);
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

        return StatSystem.CalculateStat(Defence, multipliers);
    }
    public void AttackMonster(MonsterStats monsterStats, int weapondmg, bool isranged, int attacktype)
    {
        int damage = DealDamageBasedOnStrength() + weapondmg;
        monsterStats.TakeDamage(damage, isranged, gameObject, attacktype);
    }
    // Called by skill system when you level up a skill
    public void SkillLeveledUp(Skill skill, int skillCostIncrease)
    {
        // Update level and cost
        skill.Level++;
        skill.SkillCost += skillCostIncrease;
    }
    public void SkillAttackMonster(MonsterStats monsterStats, Skill skill)
    {
        // Calculate skill damage based on level and multiplier
        int adjustedSkillDamage = (int)Mathf.RoundToInt(skill.Damage * Mathf.Pow(skill.DamageMultiplier, skill.Level - 1));
        // Calculate total damage based on intelligence and skill damage
        int damage = DealSkillDamageBasedOnIntelligence() + adjustedSkillDamage;
        // Send damage to monster
        monsterStats.TakeDamage(damage, true, gameObject, skill.DamageType);
    }
    public bool SkillWasUsed(Skill skill)
    {
        if (_currentResource >= skill.SkillCost)
        {
            // Remove skill cost from resource pool
            _currentResource -= skill.SkillCost;
            // Tell the skill system that the skill was able to be used
            return true;
        }
        else
        {
            // Tell the skill system that the skill was unable to be used
            return false;
        }
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
            AvailablePoints += _pointsPerLevel;
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
        AvalibleMaps = passed;
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
                Defence++;
            }
        }
    }
    public void IncreaseIntelligence()
    {
        if (AvailablePoints >= 1)
        {
            AvailablePoints--;
            Intelligence++;
            if (Intelligence % 5 == 0)
            {
                Wisdom++;
            }
        }
    }
    public void IncreaseWisdom()
    {
        if (AvailablePoints >= 1)
        {
            AvailablePoints--;
            Wisdom++;
            if (Wisdom % 5 == 0)
            {
                Intelligence++;
            }
        }
    }
    public void IncreaseDefense()
    {
        if (AvailablePoints >= 1)
        {
            AvailablePoints--;
            Defence++;
            if (Defence % 5 == 0)
            {
                ConstitutionChanged();
            }
        }
    }

    public void IncreaseConstitution()
    {
        if (AvailablePoints >= 1)
        {
            AvailablePoints--;
            ConstitutionChanged();
        }
    }
    private void ConstitutionChanged()
    {
        Constitution++;
        int previousHealth = SetHealth();
        if (Constitution % 5 == 0)
        {
            Defence++;
        }
        int newHealth = SetHealth();
        int healthIncrease = newHealth - previousHealth;
        CurrentHealth += healthIncrease;
        maxhealth = startingHealth + newHealth;
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
    private IEnumerator RegenerateResource()
    {
        resourceIsRegening = true;
        while (CurrentResource < maxResource)
        {
                int oldResource = CurrentResource;
                CurrentResource += 1 + (int)Math.Round(Constitution * 0.75);

                if (oldResource != CurrentResource)
                {
                    float scaleFactor = Mathf.Lerp(minScale, maxHealthScale, (float)CurrentHealth / maxhealth);
                    resourceObject.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
                }
                yield return new WaitForSeconds(1f);
        }
        resourceIsRegening = false;
    }


    private void Die()
    {
        
        var EperienceBeforeDeath = ExperiencePoints;
        // Add Game object (Dead Body) with experience before death for player to fetch
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
    // Call this method when player fetches dead body
    public void PlayerFetchedDeadBodyEXP(int EperienceBeforeDeath)
    {
        ExperiencePoints += EperienceBeforeDeath;
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