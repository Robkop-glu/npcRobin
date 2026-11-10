using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(LineRenderer))]
public class DebugPathRenderer : MonoBehaviour
{
    [SerializeField] private NavMeshAgent navMeshAgent;

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (navMeshAgent.hasPath)
        {
            lineRenderer.enabled = true;
            lineRenderer.positionCount = navMeshAgent.path.corners.Length;
            lineRenderer.SetPositions(navMeshAgent.path.corners);
        }
        else
        {
            lineRenderer.enabled = false;
        }
        
    }
}
