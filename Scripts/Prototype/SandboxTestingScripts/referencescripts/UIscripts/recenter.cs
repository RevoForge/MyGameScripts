using UnityEngine;
using Valve.VR;

public class Recenter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        OpenVR.Chaperone.ResetZeroPose(ETrackingUniverseOrigin.TrackingUniverseStanding);
    }
}
