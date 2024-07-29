using UnityEngine;
using static Revo.Methods.ObjectInteraction;

public class AttackMonster : MonoBehaviour
{
    private GameObject player;
    private PlayerStats playerstats;
    private AudioSource swordAudio;
    public int baseWeaponDamage = 10;
    public DamageType selectedDamageType;

    void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerstats = player.GetComponent<PlayerStats>();
        swordAudio = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Mob"))
        {
            MonsterStats monsterStats = other.GetComponentInParent<MonsterStats>();
            if (monsterStats != null)
            {
                playerstats.AttackMonster(monsterStats, baseWeaponDamage, false, (int)selectedDamageType);
            }
            swordAudio.Play();
        }
    }

}
