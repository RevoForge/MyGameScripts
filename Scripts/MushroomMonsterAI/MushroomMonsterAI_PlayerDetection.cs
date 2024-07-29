using Revo.Methods;
using UnityEngine;

public class MushroomMonsterAI_PlayerDetection : MonoBehaviour
{
    private MushroomMonsterAI mainAI;
    private GameObject chasedPlayer;
    private GameObject playerCamera;


    void OnEnable()
    {
        mainAI = GetComponentInParent<MushroomMonsterAI>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && chasedPlayer == null)
        {
            chasedPlayer = other.gameObject;
            playerCamera = PlayerDetection.FindChildWithTag(other.transform, "MainCamera");
            mainAI.PlayerDetected(playerCamera);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && chasedPlayer)
        {
            mainAI.PlayerLost();
            chasedPlayer = null;
        }
    }
}
