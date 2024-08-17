using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "Skills/Skill")]
public class Skill : ScriptableObject
{
    public SkillType SkillName;
    public int DamageType;
    public int Level;
    public int Damage;
    public float DamageMultiplier;
    public int SkillCost;
}