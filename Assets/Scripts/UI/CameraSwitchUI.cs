using TMPro;
using Unity.Cinemachine;
using UnityEngine;

public class CameraSwitchUI : MonoBehaviour
{
    private SpectatorCameraController cameraController;

    [SerializeField] private TextMeshProUGUI buttonText;

    private void Start()
    {
        cameraController = FindFirstObjectByType<SpectatorCameraController>();
        buttonText.text = cameraController.CurrentCamera.name;
        cameraController.onCameraChanged.AddListener(OnCameraChanged);
    }

    private void OnCameraChanged(CinemachineCamera camera)
    {
        buttonText.text = camera.name;
    }
}
