using Revo.Methods;
using UnityEngine;
using static Revo.Methods.ObjectInteraction;

public class AI_PlayerDetectionZone : MonoBehaviour
{
    private IPlayerDetectionAI mainAI;
    [HideInInspector]
    public GameObject chasedPlayer;
    private GameObject playerCamera;

    void Start()
    {
        mainAI = GetComponentInParent<IPlayerDetectionAI>();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && chasedPlayer == null)
        {
            chasedPlayer = other.gameObject;
            playerCamera = PlayerDetection.FindChildWithTag(other.transform, "MainCamera");
            StartCoroutine(mainAI.PlayerDetected(playerCamera));
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && chasedPlayer != null)
        {
            mainAI.PlayerLost();
            chasedPlayer = null;
        }
    }
}
