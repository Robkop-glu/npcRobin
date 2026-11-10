using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(LineRenderer))]
public class BasicHitscanWeapon : Weapon
{
    [SerializeField] private GameObject owner;

    [SerializeField] private bool printAccuracy = false;

    [Header("Stats")]
    public float damage = 10f;
    public float range = 100f;
    public float fireRate = 5f;

    [Header("Spread")]
    [Tooltip("Spread in degrees")]
    public float minSpreadAngle = 1.5f;
    public float maxSpreadAngle = 20f;

    [Tooltip("How long it will take, in seconds, to get full focus fire.")]
    [SerializeField] private float focusDuration = 1.0f;
    private float focusTimer = 0f;

    [Tooltip("The multiplier to the spread if focus is full. Set to a low value to reduce spread a lot when focussing.")]
    [Range(0f, 1f)]
    [SerializeField] private float focusSpreadRatio = 0.1f;

    [Header("Layer Mask")]
    public LayerMask hitMask;

    [Header("References")]
    public Transform firePoint;
    public GameObject muzzleFlash;

    [Tooltip("The object to spawn where we hit a target. This is basically an impact effect.")]
    public GameObject hitEffect;

    [SerializeField] private AudioSource audioSource;

    [Header("Tracer")]
    public float tracerDuration = 0.05f;

    private float nextTimeToFire = 0f;
    private float prevFireTime = 0f;
    private LineRenderer lineRenderer;
    private int shootCount = 0;
    private int hitCount = 0;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        lineRenderer.positionCount = 2;

        if (owner == null)
        {
            Debug.Log("Weapon " + name + " is missing an owner!");
        }
    }

    private void Update()
    {
        float timeSinceLastFired = Time.time - prevFireTime;

        // We fired almost as fast as we could.
        if (timeSinceLastFired <= 2f / fireRate)
        {
            focusTimer = Mathf.Min(focusTimer +  Time.deltaTime, focusDuration);
        }
        else
        {
            focusTimer = 0f;
        }
    }

    /// <summary> Try shoot at a certain target. </summary>
    /// <param name="direction"> The direction to shoot in, taken from the firepoint. </param>
    /// <param name="accuracyLerp"> The accuracy used. Accuracy of 1 is best, 0 is worst. </param>
    public void TryShoot(Vector3 direction, float accuracyLerp, GameObject desiredTarget = null)
    {
        if (Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            prevFireTime = Time.time;
            Shoot(direction, accuracyLerp, desiredTarget);
        }
    }

    void Shoot(Vector3 direction, float accuracyLerp, GameObject desiredTarget)
    {
        Vector3 startPoint = firePoint.position;

        // Apply random spread
        direction = GetSpreadDirection(direction, accuracyLerp);

        if (muzzleFlash != null)
        {
            Instantiate(muzzleFlash, firePoint.position, Quaternion.LookRotation(direction, Vector3.up), firePoint.transform);
        }

        Vector3 endPoint;
        RaycastHit hit;
        shootCount++;

        if (Physics.Raycast(startPoint, direction, out hit, range, hitMask))
        {
            endPoint = hit.point;

            // Debug.Log("Hit: " + hit.transform.name);

            if (hit.transform.TryGetComponent(out HealthController healthController))
            {
                HealthChangeInfo info = new()
                {
                    originAgent = owner,
                    originDevice = gameObject,
                    targetPosition = hit.point,
                    originalTarget = desiredTarget,
                    rawAmount = Mathf.Abs(damage) * -1.0f
                };

                healthController.ApplyHealthChangeInfo(info);
                hitCount++;
            }

            if (hitEffect != null)
            {
                GameObject impact = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impact, 2f);
            }
        }
        else
        {
            endPoint = startPoint + direction * range;
        }

        StartCoroutine(ShowTracer(startPoint, endPoint));
        if (printAccuracy) Debug.Log(transform.parent.name + " accuracy: " + ((float)hitCount / shootCount * 100f) + "%");

        if (audioSource != null)
        {
            audioSource.Play();
        }
    }

    Vector3 GetSpreadDirection(Vector3 direction, float accuracyLerp)
    {
        // Convert spread angle to a random cone direction
        float spreadRad = Mathf.Lerp(maxSpreadAngle, minSpreadAngle, accuracyLerp) * Mathf.Deg2Rad;

        float relativeFocusTime = focusTimer / focusDuration; // How long we have focused, relatively.
        // The focus multiplier, 1f is low focus and high spread. 0 is high focus and low spread.
        float focusMultiplier = 1.0f - Mathf.Lerp(focusSpreadRatio, 1.0f, relativeFocusTime);

        float randomYaw = UnityEngine.Random.Range(-spreadRad, spreadRad) * focusMultiplier;
        float randomPitch = UnityEngine.Random.Range(-spreadRad, spreadRad) * focusMultiplier;

        Quaternion spreadRotation = Quaternion.Euler(
            randomPitch * Mathf.Rad2Deg,
            randomYaw * Mathf.Rad2Deg,
            0f
        );

        Vector3 originalDirection = direction.normalized;

        return spreadRotation * originalDirection;
    }

    IEnumerator ShowTracer(Vector3 start, Vector3 end)
    {
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        lineRenderer.enabled = true;

        yield return new WaitForSeconds(tracerDuration);

        lineRenderer.enabled = false;
    }
}
