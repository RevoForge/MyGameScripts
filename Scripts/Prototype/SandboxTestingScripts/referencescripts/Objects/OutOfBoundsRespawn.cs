using UnityEngine;

public class OutOfBoundsRespawn : MonoBehaviour
{
    private PlayerController mainAI;
    private PickupObjectScript PickupObject;
    private PickupObjectsPhysics PhysicsObject;

    private AudioSource ohNoAudioSource;

    private void OnEnable()
    {
        ohNoAudioSource = GetComponent<AudioSource>();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            mainAI = other.GetComponent<PlayerController>();
            if (mainAI != null)
            {
                mainAI.ResetPositionAndRotation();
                ohNoAudioSource.Play();
            }
        }
        if (other.CompareTag("Weapon"))
        {
            PickupObject = other.GetComponent<PickupObjectScript>();
            PhysicsObject = other.GetComponent<PickupObjectsPhysics>();

            if (PickupObject != null)
            {
                PickupObject.ResetPositionAndRotation();
            }
            if (PhysicsObject != null)
            {
                PhysicsObject.ResetPositionAndRotation();
            }
        }
    }
}
