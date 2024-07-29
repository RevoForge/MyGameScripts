using Revo.Methods;
using UnityEngine;

public class RockMonsterAI_PlayerDetection : MonoBehaviour
{
    private RockMonsterAI mainAI;
    private GameObject chasedPlayer;
    private GameObject playerCamera;


    void OnEnable()
    {
        mainAI = GetComponentInParent<RockMonsterAI>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && chasedPlayer == null)
        {
            chasedPlayer = other.gameObject;
            playerCamera = PlayerDetection.FindChildWithTag(other.transform, "MainCamera");
            mainAI.PlayerDetected(playerCamera);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && chasedPlayer != null)
        {
            mainAI.PlayerLost();
            chasedPlayer = null;
        }
    }
}
