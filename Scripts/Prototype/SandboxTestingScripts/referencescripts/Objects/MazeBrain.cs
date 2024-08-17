using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class MazeBrain : MonoBehaviour
{
    public List<Transform> wallSections; // List of wall section GameObjects
    public Transform StartOfMaze;
    public Transform EndOfMaze;
    public NavMeshSurface navMeshSurface;
    public NavMeshData navMeshData;
    private NavMeshPath path;
    private NavMeshAgent navMeshAgent;
    public int MinimumPathLength = 35;
    public bool tryagain = false;
    public bool resetAgent = false;
    public bool KeepRetrying = true;
    private int OriginalMinimumPathLength;


    void Start()
    {
        OriginalMinimumPathLength = MinimumPathLength;
        path = new NavMeshPath();
        navMeshAgent = FindAnyObjectByType<NavMeshAgent>();
        wallSections = new List<Transform>(GetComponentsInChildren<Transform>());
        StartCoroutine(RotateWallSections());
    }
    private void FixedUpdate()
    {
        if (tryagain)
        {
            tryagain = false;
            StartCoroutine(RotateWallSections());
        }

        if (resetAgent)
        {
            resetAgent = false;
            navMeshAgent.Warp(StartOfMaze.position);
            StartCoroutine(RotateWallSections());
        }
    }
    private IEnumerator RotateWallSections()
    {
        StartCoroutine(LowerPathLength());
        while (KeepRetrying)
        {
            int randomRotation = Random.Range(0, 2);
            if (randomRotation == 0)
            {
                {
                    // Rotate odd-numbered walls
                    for (int j = 1; j < wallSections.Count; j += 2) // Increment by 2 to get odd indices
                    {
                        int rotationAngle = Random.Range(0, 2) * 90;
                        wallSections[j].transform.rotation = Quaternion.Euler(0, rotationAngle, 0);
                    }
                }
            }
            else
            {
                // Rotate even-numbered walls
                for (int j = 2; j < wallSections.Count; j += 2) // Increment by 2 to get even indices
                {
                    int rotationAngle = Random.Range(0, 2) * 90;
                    wallSections[j].transform.rotation = Quaternion.Euler(0, rotationAngle, 0);
                }
            }


            navMeshSurface.UpdateNavMesh(navMeshData);
            if (navMeshAgent.CalculatePath(EndOfMaze.position, path))
            {
                //if (path.status == NavMeshPathStatus.PathComplete)
                //{
                    // Path is complete between StartOfMaze and EndOfMaze
                    //Debug.Log("Path is complete.");
                    float pathLength = 0f;
                    for (int k = 1; k < path.corners.Length; k++)
                    {
                        pathLength += Vector3.Distance(path.corners[k - 1], path.corners[k]);
                    }
                    if (pathLength > MinimumPathLength)
                    {
                        Debug.Log("Length of the path: " + pathLength);
                        KeepRetrying = false;
                    }
                    else
                    {
                        //Debug.Log("Path too short. Retrying...");
                    }

                //}
                //else
                //{
                    // Path is not complete
                    //Debug.Log("Path Only Partial. Retrying...");
                //}
            }
            else
            {
                //Debug.Log("No Path Can be Found. Retrying...");
            }
            yield return null;
        }
        navMeshAgent.SetDestination(EndOfMaze.position);
    }
    private IEnumerator LowerPathLength()
    {
        while (true)
        {
            MinimumPathLength -= 1;
            yield return new WaitForSeconds(0.1f);
            if (!KeepRetrying)
            {
                MinimumPathLength = OriginalMinimumPathLength;
                break;
            }
        }
    }
}
