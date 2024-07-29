using UnityEngine;

public class HealthRegenTrigger : MonoBehaviour
{
    private PlayerStats playerStats;
    private bool mobInRange = false;
    private float timer = 0f;
    private float interval = 10f;

    private void OnEnable()
    {
        playerStats = GetComponentInParent<PlayerStats>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Mob") && !mobInRange)
        {
            playerStats.mobsAreClose = true;
            mobInRange = true;
        }
    }

    void LateUpdate()
    {
        timer += Time.deltaTime;
        if (mobInRange && timer >= interval)
        {
            mobInRange = false;
            playerStats.mobsAreClose = false;
            timer = 0f;
        }
    }

}
