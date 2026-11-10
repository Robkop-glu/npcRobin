using TMPro;
using Unity.Cinemachine;
using UnityEngine;

public class CharacterSwitchUI : MonoBehaviour
{
    private SpectatorCameraController cameraController;

    [SerializeField] private TextMeshProUGUI buttonText;

    private int currentCharacterIndex = -1;

    private void Start()
    {
        cameraController = FindFirstObjectByType<SpectatorCameraController>();
        cameraController.onCameraChanged.AddListener(OnCameraChanged);
        GoToNextCharacter();
    }

    public void GoToNextCharacter()
    {
        var characters = SpawnManager.Instance.Characters;
        if (characters == null || characters.Count == 0)
        {
            buttonText.text = "Try again later.";
            return;
        }

        currentCharacterIndex = (currentCharacterIndex + 1) % characters.Count;
        buttonText.text = characters[currentCharacterIndex].name;
        cameraController.SetTargetToFollowForAllCharacterCameras(characters[currentCharacterIndex].transform);
    }

    private void OnCameraChanged(CinemachineCamera newCamera)
    {
        if (newCamera.TryGetComponent(out CameraInfo cameraInfo)) 
        {
            gameObject.SetActive(cameraInfo.attachment == CameraInfo.Attachment.CharacterDependent);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }
}
