using System;
using System.IO;
using UnityEngine;
using Revo.Methods;

public class SettingsDatabase : MonoBehaviour
{
    private const string fileName = "settings.json";

    public Settings settings;

    private void Start()
    {
        LoadSettings();
    }
    public Settings ResetGameSave()
    {
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        if (File.Exists(filePath))
        {
            settings = DefaultSettings();
            string json = JsonUtility.ToJson(settings);
            SavedGameEncryption.EncryptAndSaveJson(filePath, json);
        }
        return settings;
    }
    private Settings DefaultSettings()
    {
        return new Settings
        {
            MovementType = false,
            SmoothMovementSpeed = 100f,
            MenuSound = 0.25f,
            DungeonSound = 0.5f,
            SlumsSound = 0.05f,
            ForestSound = 0.1f,
            DwarfSound = 0.1f,
            DragonSound = 0.075f,
            OrcSound = 0.075f,
            MonsterSound = 0.05f,
            CharecterLevel = 0,
            STR = 0,
            INT = 0,
            WIS = 0,
            DEF = 0,
            CON = 0,
            CharecterExp = 0,
            AvailableCharacterPoints = 3,
            AvailableMaps = 0,
            PhysicalSkillLevel = new int[9],
            PhysicalSkillExp = new int[9],
            PhysicalSkillLocked = new bool[9],
            MagicalSkillLevel = new int[9],
            MagicalSkillExp = new int[9],
            MagicalSkillLocked = new bool[9],
            UtilitySkillLevel = new int[9],
            UtilitySkillExp = new int[9],
            UtilitySkillLocked = new bool[9],
            AvalibleSkillPoints = 0,
            HasDiedOnce = false,
            HasSeenItemOI = false,
            SeatedToggle = false,
        };
    }
    public Settings LoadSettings()
    {
        if (settings == null)
        {
            string filePath = Path.Combine(Application.persistentDataPath, fileName);
            if (File.Exists(filePath))
            {
                string json = SavedGameEncryption.DecryptAndReadJson(filePath);
                settings = JsonUtility.FromJson<Settings>(json);
            }
            else
            {
                // Handle case when settings file doesn't exist
                settings = DefaultSettings();
            }
            return settings;
        }
        else
        {
            return settings;
        }

    }
    public void SaveSettings(bool movementType, float smoothMovementSpeed, float[] soundSettings, int Charlevel, int strength, int intelligence, int wisdom, int defense, int constitution,int charecterExp, int avCharPnt, int AvMap,int[] PhysSkllLvl, int[] PhysSkllExp, bool[] PhysSkllLocked, int[] MagSkllLvl, int[] MagSkllExp, bool[] MagSkllLocked, int[] UtilSkllLvl, int[] UtilSkllExp, bool[] UtilSkllLocked,int avSkllPnt, bool died, bool itemoi, bool seatedtoggle)
    {
        settings.MovementType = movementType;
        settings.SmoothMovementSpeed = smoothMovementSpeed;
        settings.MenuSound = soundSettings[0];
        settings.DungeonSound = soundSettings[1];
        settings.SlumsSound = soundSettings[2];
        settings.ForestSound = soundSettings[3];
        settings.DwarfSound = soundSettings[4];
        settings.DragonSound = soundSettings[5];
        settings.OrcSound = soundSettings[6];
        settings.MonsterSound = soundSettings[7];
        settings.CharecterLevel = Charlevel;
        settings.STR = strength;
        settings.INT = intelligence;
        settings.WIS = wisdom;
        settings.DEF = defense;
        settings.CON = constitution;
        settings.CharecterExp = charecterExp;
        settings.AvailableCharacterPoints = avCharPnt;
        if (AvMap > settings.AvailableMaps)
        {
            settings.AvailableMaps = AvMap;
        }
        settings.PhysicalSkillLevel = PhysSkllLvl;
        settings.PhysicalSkillExp = PhysSkllExp;
        settings.PhysicalSkillLocked = PhysSkllLocked;
        settings.MagicalSkillLevel = MagSkllLvl;
        settings.MagicalSkillExp = MagSkllExp;
        settings.MagicalSkillLocked = MagSkllLocked;
        settings.UtilitySkillLevel = UtilSkllLvl;
        settings.UtilitySkillExp = UtilSkllExp;
        settings.UtilitySkillLocked = UtilSkllLocked;
        settings.AvalibleSkillPoints = avSkllPnt;
        settings.HasDiedOnce = died;
        settings.HasSeenItemOI = itemoi;
        settings.SeatedToggle = seatedtoggle;

        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        string json = JsonUtility.ToJson(settings);
        SavedGameEncryption.EncryptAndSaveJson(filePath, json);
    }

    public float[] GetSoundSettings()
    {
        var soundSettings = new float[8];
        if (settings != null)
        {
            soundSettings[0] = settings.MenuSound;
            soundSettings[1] = settings.DungeonSound;
            soundSettings[2] = settings.SlumsSound;
            soundSettings[3] = settings.ForestSound;
            soundSettings[4] = settings.DwarfSound;
            soundSettings[5] = settings.DragonSound;
            soundSettings[6] = settings.OrcSound;
            soundSettings[7] = settings.MonsterSound;
        }
        return soundSettings;
    }
}


