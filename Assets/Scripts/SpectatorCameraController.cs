using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class SpectatorCameraController : MonoBehaviour
{
    public UnityEvent<CinemachineCamera> onCameraChanged;

    [SerializeField] private List<CinemachineCamera> cameras;

    private int currentCamera = -1;

    public CinemachineCamera CurrentCamera
    {
        get
        {
            if (cameras == null || cameras.Count == 0) return null;
            if (currentCamera < 0 ||  currentCamera > cameras.Count) return null;
            return cameras[currentCamera];
        }
    }

    private void Awake()
    {
        ActivateNextCamera();
    }

    public void ActivateNextCamera()
    {
        cameras = FindObjectsByType<CinemachineCamera>(FindObjectsSortMode.InstanceID).ToList();

        currentCamera = (currentCamera + 1) % cameras.Count;
        for (int i = 0; i < cameras.Count; i++)
        {
            cameras[i].Priority = i == currentCamera ? 1 : 0;
            if (cameras[i].TryGetComponent(out CameraInfo info))
            {
                info.CurrentlyActive = i == currentCamera;
            }
        }

        onCameraChanged.Invoke(CurrentCamera);
    }

    public void SetTargetToFollowForAllCharacterCameras(Transform target)
    {
        foreach (CinemachineCamera camera in cameras)
        {
            if (camera.TryGetComponent(out CameraInfo cameraInfo) && cameraInfo.attachment == CameraInfo.Attachment.CharacterDependent)
            {
                camera.Target = new CameraTarget() { TrackingTarget = target };
            }           
        }
    }
}
