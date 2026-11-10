using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Targeter : MonoBehaviour
{
    public UnityEvent<TargetingInfo> onTargetFound;
    public UnityEvent<TargetingInfo> onTargetLost;
    public UnityEvent<TargetingInfo> onTargetForgotten;

    [SerializeField] private bool debugPrint = false;

    [SerializeField] private List<Sensor> visionRequiredSensors = new List<Sensor>();

    [SerializeField] private float refreshLiveStatusInterval = 0.5f;
    private float refreshLiveStatusTimer = 0f;

    [Tooltip("How long this character remembers targets. If set to 0, it forgets immediately. If set to below zero, it will remember forever.")]
    [SerializeField] [Range(-1f, 120f)] private float memoryTime = 10f;

    private readonly Dictionary<GameObject, TargetingInfo> targetingInfos = new();

    public void AddPotentialTarget(GameObject target)
    {
        if (!targetingInfos.ContainsKey(target))
        {
            targetingInfos.Add(target, new TargetingInfo(target));
        }
    }

    private void Update()
    {
        refreshLiveStatusTimer += Time.deltaTime;
        if (refreshLiveStatusTimer >= refreshLiveStatusInterval)
        {
            RefreshTargets();
            refreshLiveStatusTimer = 0f;
        }
    }

    private void RefreshTargets()
    {
        List<GameObject> toForget = new();

        foreach (TargetingInfo info in targetingInfos.Values)
        {
            bool isStillLive = true;
            foreach(Sensor sensor in visionRequiredSensors)
            {
                if (!sensor.IsTargetDetectable(info.TargetObject))
                {
                    isStillLive = false;
                    break;
                }
            }

            // We lost the target, but refound it!
            if (!info.IsLive && isStillLive)
            {
                info.IsLive = true;
                onTargetFound.Invoke(info);
            }

            // We lost the target.
            else if (info.IsLive && !isStillLive)
            {
                LoseTarget(info);
            }

            // We lost the target and its still lost.
            else if (!info.IsLive && !isStillLive)
            {
                info.TimeSinceLastSeen += refreshLiveStatusInterval;
                //Debug.Log("Lost target has been lost for " + info.TimeSinceLastSeen + " seconds.");
            }

            // If we have a memory and this info exceeds it, forget the target.
            if (memoryTime >= 0f && !info.IsLive && info.TimeSinceLastSeen > memoryTime)
            {
                toForget.Add(info.TargetObject);
            }
        }

        // Forget all targets we should forget. 
        foreach (GameObject obj in toForget) ForgetTarget(obj);
    }

    private void LoseTarget(TargetingInfo info)
    {
        info.IsLive = false;
        info.LastConfirmedPosition = info.TargetObject.transform.position;
        info.TimeSinceLastSeen = refreshLiveStatusInterval;
        if (debugPrint)  Debug.Log("Lost target " + info.TargetObject.name);
        onTargetLost.Invoke(info);
    }

    private void ForgetTarget(GameObject target)
    {
        if (debugPrint)  Debug.Log("Forgot target " + target.name);
        onTargetForgotten.Invoke(targetingInfos[target]);
        targetingInfos.Remove(target);
    }

    public void ClearAll()
    {
        targetingInfos.Clear();
        foreach (Sensor sensor in visionRequiredSensors)
        {
            sensor.Clear();
        }
    }
}


public class TargetingInfo
{
    private readonly GameObject targetObject;

    /// <summary> The target / subject GameObject of this targeting info. </summary>
    public GameObject TargetObject => targetObject;

    private float timeSinceLastSeen;

    public float TimeSinceLastSeen
    {
        get
        {
            if (isLive) return 0f;
            return timeSinceLastSeen;
        }
        set { timeSinceLastSeen = value; }
    }

    private Vector3 lastConfirmedPosition;
    public Vector3 LastConfirmedPosition
    {
        get
        {
            if (isLive) return targetObject.transform.position;
            return lastConfirmedPosition;
        }
        set
        {
            lastConfirmedPosition = value;
        }
    }

    private bool isLive;
    public bool IsLive
    {
        get { return isLive; }
        set { 
            isLive = value;
            if (isLive)
            {
                lastConfirmedPosition = targetObject.transform.position;
                timeSinceLastSeen = 0f;
            }
        }
    }

    public TargetingInfo(GameObject targetObject)
    {
        this.targetObject = targetObject;
        timeSinceLastSeen = 0f;
        lastConfirmedPosition = targetObject.transform.position;
        IsLive = false;
    }
}