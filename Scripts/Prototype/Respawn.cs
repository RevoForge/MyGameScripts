using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    public GameObject rigReference;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == rigReference)
        {
            rigReference.transform.position = new Vector3(0,1,0);
        }
    }
}
