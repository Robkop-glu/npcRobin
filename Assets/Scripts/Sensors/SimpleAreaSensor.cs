using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Collections.ObjectModel;

[RequireComponent(typeof(SphereCollider))]
public class SimpleAreaSensor : Sensor
{
    [SerializeField] private UnityEvent<GameObject> OnColliderDetected;
    [SerializeField] private UnityEvent<GameObject> OnGameObjectLost;

    [SerializeField] private float checkInterval = 0.5f;
    private float checkTimer = 0f;

    [SerializeField] private LayerMask layerMask;
    [SerializeField] private QueryTriggerInteraction queryTriggers = QueryTriggerInteraction.Ignore;

    [SerializeField] private List<GameObject> ignoreList;
    private SphereCollider sphereCollider;

    [SerializeField] private List<GameObject> allDetectedObjects = new();

    public List<GameObject> AllDetectedObjects => allDetectedObjects;

    [SerializeField] private bool printDebug = false;

    private Collider[] resultsBuffer = new Collider[10];

    private void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.enabled = false;
    }

    public override bool IsTargetDetectable(GameObject target)
    {
        return allDetectedObjects.Contains(target);
    }

    private void FixedUpdate()
    {
        checkTimer += Time.fixedDeltaTime;
        if (checkTimer >= checkInterval)
        {
            DoCollisionCheck();
            checkTimer = 0f;
        }
    }

    private void DoCollisionCheck()
    {
        int hitCount = Physics.OverlapSphereNonAlloc(transform.position, sphereCollider.radius * transform.lossyScale.x, resultsBuffer, layerMask, queryTriggers);
        for (int i = 0; i < hitCount; i++)
        {
            TryAddNewObject(resultsBuffer[i].gameObject);
        }

        // Remove all objects that were not found in this new check.
        for (int i = allDetectedObjects.Count - 1; i >= 0; i--)
        {
            bool found = false;
            for (int x = 0; x < hitCount; x++)
            {
                if (allDetectedObjects[i] == resultsBuffer[x].gameObject)
                {
                    found = true;
                    break;
                }
            }

            if (!found) RemoveObjectAtIndex(i);
        }
    }

    private void TryAddNewObject(GameObject other)
    {
        // Ignore any object in the ignore list, or dead objects.
        if (allDetectedObjects.Contains(other)) return;
        if (ignoreList.Contains(other)) return;
        if (other.TryGetComponent(out HealthController healthController) && !healthController.Alive) return;

        allDetectedObjects.Add(other.gameObject);
        OnColliderDetected.Invoke(other.gameObject);

        if (printDebug) Debug.Log(name + " detected " + other.gameObject.name);
    }

    private void RemoveObjectAtIndex(int index)
    {
        GameObject obj = allDetectedObjects[index];
        AllDetectedObjects.RemoveAt(index);
        OnGameObjectLost.Invoke(obj);

        if (printDebug) Debug.Log(name + " lost " + obj.name);
    }

    public override void Clear()
    {
        allDetectedObjects.Clear();
    }
}
