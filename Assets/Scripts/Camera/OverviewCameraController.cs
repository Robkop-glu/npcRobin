using Unity.Cinemachine;
using UnityEngine;
using static UnityEngine.AdaptivePerformance.Provider.AdaptivePerformanceSubsystemDescriptor;

[RequireComponent(typeof(CinemachineTargetGroup))]
public class OverviewCameraController : MonoBehaviour
{
    public void OnCharacterSpawned(GameObject character)
    {
        CinemachineTargetGroup targetGroup = GetComponent<CinemachineTargetGroup>();
        targetGroup.AddMember(character.transform, 1.0f, 1.0f);
    }

    public void OnCharacterDestroyed(GameObject character)
    {
        CinemachineTargetGroup targetGroup = GetComponent<CinemachineTargetGroup>();
        targetGroup.RemoveMember(character.transform);
    }
}
