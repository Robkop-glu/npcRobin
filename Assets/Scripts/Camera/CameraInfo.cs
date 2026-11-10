using UnityEngine;
using UnityEngine.Events;

public class CameraInfo : MonoBehaviour
{
    [SerializeField] private bool currentlyActive = false;

    public bool CurrentlyActive
    {
        get { return currentlyActive; }
        set {
            if (currentlyActive == value) return;

            currentlyActive = value;
            onBecameActive?.Invoke(value);
        }
    }

    public UnityEvent<bool> onBecameActive;

    public enum Attachment
    {
        /// <summary> This camera is not dependent on any character existing. Can for example be a free-fly spectator camera, or overview cam. </summary>
        Free = 0,

        /// <summary> This camera is dependent on characters existing, but is able to switch between different characters. Can be a follow camera for example. </summary>
        CharacterDependent = 1,

        /// <summary> The camera is locked to this specific character. </summary>
        CharacterLocked = 2
    }

    public Attachment attachment;
}
