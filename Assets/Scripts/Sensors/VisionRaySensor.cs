using UnityEngine;

public class VisionRaySensor : Sensor
{
    [SerializeField] private Transform raycastOrigin;
    [SerializeField] private float maxRaycastDistance = 50f;
    [SerializeField] private LayerMask detectionMask = 0;
    [SerializeField] private QueryTriggerInteraction detectTriggers = QueryTriggerInteraction.Ignore;

    [SerializeField] private bool showDebug = false;
    [SerializeField, Range(0f, 360f)] private float maxViewAngle = 90f;

    public override void Clear()
    {
        return;
    }

    public override bool IsTargetDetectable(GameObject target)
    {
        Vector3 targetPosition = target.transform.position;

        if (target.TryGetComponent(out CharacterController characterController))
        {
            targetPosition += characterController.center;
        }

        Vector3 toTarget = targetPosition - raycastOrigin.position;
        float distanceToTarget = toTarget.magnitude;

        // Early out: too far
        if (distanceToTarget > maxRaycastDistance) return false;

        Vector3 direction = toTarget.normalized;

        float angleToTarget = Vector3.Angle(raycastOrigin.forward, direction);
        if (angleToTarget > maxViewAngle * 0.5f)  return false;

        Ray ray = new(raycastOrigin.position, direction);
        bool hitAnything = Physics.Raycast(ray, out RaycastHit hitInfo, maxRaycastDistance, detectionMask, detectTriggers);

        if (showDebug)
        {
            Vector3 end = hitAnything ? hitInfo.point : raycastOrigin.position + direction * maxRaycastDistance;

            Color color = Color.grey;

            if (hitAnything && hitInfo.collider.gameObject == target)
            {
                color = Color.green;
            }
            else if (hitAnything)
            {
                color = Color.black;
            }
            else
            {
                color = Color.red; // outside or nothing hit
            }

            Debug.DrawLine(raycastOrigin.position, end, color, 0.5f);
        }

        if (!hitAnything) return false;
        return hitInfo.collider.gameObject == target;
    }
}
