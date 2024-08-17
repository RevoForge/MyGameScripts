using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class StartOnTracking : MonoBehaviour
{
    public Transform vrcamera;
    public float distanceFromFace;
    public float yPositionOffset;
    public float delay = 0.5f;
    private bool isHeadTracked = false;

    void OnEnable()
    {
        PositionMenu();
    }
    void Update()
    {
        if (!isHeadTracked)
        {
            if (IsHeadTracked())
            {
                isHeadTracked = true;
                StartCoroutine(PositionMenuWithDelay());
            }
        }
    }

    private IEnumerator PositionMenuWithDelay()
    {
        yield return new WaitForSeconds(delay);
        PositionMenu();
    }

    private bool IsHeadTracked()
    {
        List<XRNodeState> nodeStates = new List<XRNodeState>();
        InputTracking.GetNodeStates(nodeStates);

        foreach (XRNodeState nodeState in nodeStates)
        {
            if (nodeState.nodeType == XRNode.Head)
            {
                return nodeState.tracked;
            }
        }
        return false;
    }

    void PositionMenu()
    {
        Vector3 targetPosition = vrcamera.position + (vrcamera.forward * distanceFromFace);
        targetPosition.y = vrcamera.position.y + yPositionOffset;
        transform.position = targetPosition;
        Quaternion targetRotation = Quaternion.Euler(0f, vrcamera.rotation.eulerAngles.y, 0f);
        transform.rotation = targetRotation;
    }

}
