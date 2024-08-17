using System;

[Serializable]
public class Settings
{
    public int Id;

    public bool MovementType;
    public float SmoothMovementSpeed;
    public float MenuSound;
    public float DungeonSound;
    public float SlumsSound;
    public float ForestSound;
    public float DwarfSound;
    public float DragonSound;
    public float OrcSound;
    public float MonsterSound;

    public int CharecterLevel;
    public int STR;
    public int INT;
    public int WIS;
    public int DEF;
    public int CON;
    public int CharecterExp;
    public int AvailableCharacterPoints;
    public int AvailableMaps;
    public int[] PhysicalSkillLevel = new int[9];
    public int[] PhysicalSkillExp = new int[9];
    public bool[] PhysicalSkillLocked = new bool[9];
    public int[] MagicalSkillLevel = new int[9];
    public int[] MagicalSkillExp = new int[9];
    public bool[] MagicalSkillLocked = new bool[9];
    public int[] UtilitySkillLevel = new int[9];
    public int[] UtilitySkillExp = new int[9];
    public bool[] UtilitySkillLocked = new bool[9];
    public int AvalibleSkillPoints;

    public bool HasDiedOnce;
    public bool HasSeenItemOI;
    public bool SeatedToggle;
}
