using Revo.Methods;
using UnityEngine;

public class WanderingAI_PlayerDetection : MonoBehaviour
{
    private BasicWanderingAI mainAI;
    private GameObject chasedPlayer;
    private GameObject playerCamera;

    void Start()
    {
        mainAI = GetComponentInParent<BasicWanderingAI>();
    }
    void OnTriggerStay(Collider other)
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
