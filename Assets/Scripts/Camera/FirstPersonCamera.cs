using UnityEngine;
using UnityEngineInternal;

public class FirstPersonCamera : MonoBehaviour
{
    [SerializeField] private Transform aimTarget;
    private Camera mainCamera;
    [SerializeField] private CameraInfo playerCameraInfo;
    [SerializeField] private LayerMask detectionMask;
    [SerializeField] private float maxDistance = 100f;

    [Tooltip("The layers to NOT draw when in first person perspective.")]
    [SerializeField] private LayerMask firstPersonIgnoreLayers;

    [Tooltip("The health controller used by the character this camera is attached to.")]
    [SerializeField] private HealthController healthController;

    private void Start()
    {
        mainCamera = Camera.main;

        playerCameraInfo.onBecameActive.AddListener(OnCameraBecameActive);
        OnCameraBecameActive(playerCameraInfo.CurrentlyActive);

        if (healthController != null) healthController.onAliveStateChanged.AddListener(OnHealthControllerAliveChanged);
    }

    private void OnCameraBecameActive(bool active)
    {
        RefreshFirstPersonCameraLayers();
    }

    public void OnHealthControllerAliveChanged(HealthController hc, bool alive)
    {
        RefreshFirstPersonCameraLayers();
    }

    private void RefreshFirstPersonCameraLayers()
    {
        if (playerCameraInfo.CurrentlyActive && healthController.Alive) mainCamera.cullingMask &= ~firstPersonIgnoreLayers;
        else mainCamera.cullingMask |= firstPersonIgnoreLayers;
    }

    private void FixedUpdate()
    {
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if(Physics.Raycast(ray.origin, ray.direction, out var hit, maxDistance, detectionMask, QueryTriggerInteraction.Ignore))
        {
            aimTarget.position = hit.point;
        }
        else
        {
            aimTarget.position = playerCameraInfo.transform.position + playerCameraInfo.transform.forward * maxDistance;
        }
    }

}
