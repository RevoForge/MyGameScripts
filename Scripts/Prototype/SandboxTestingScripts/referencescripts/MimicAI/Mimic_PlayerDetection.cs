using Revo.Methods;
using UnityEngine;

public class MimicPlayerDetection : MonoBehaviour
{
    private MimicAI mainAI;
    private GameObject chasedPlayer;
    private GameObject playerCamera;


    void OnEnable()
    {
        mainAI = GetComponentInParent<MimicAI>();
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
        if (other.tag == "Player" && chasedPlayer)
        {
            mainAI.PlayerLost();
            chasedPlayer = null;
        }
    }

}
