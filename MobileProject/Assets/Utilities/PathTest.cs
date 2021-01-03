using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PathTest : MonoBehaviour {
    public NavMeshAgent agent;
    public Transform otherTransform;

    NavMeshPath path;
    public int corners;

    void Start()
    {
        agent.isStopped = true;
        path = new NavMeshPath();
    }

    void Update () {
        agent.enabled = true;
        agent.CalculatePath(otherTransform.position, path);
        agent.enabled = false;
        corners = path.corners.Length;
    }

    void OnDrawGizmos()
    {
        if (path == null)
            return;

        for (int i = 0; i < path.corners.Length -1; ++i)
        {
            Gizmos.DrawLine(path.corners[i], path.corners[i + 1]);
        }
    }
}
