using UnityEngine;

public class SimpleFollow : MonoBehaviour
{
    [SerializeField] private bool useLateUpdate = false;

    public Transform target;
    [SerializeField] private Vector3 positionOffset;
    [SerializeField] private Vector3 positionOffsetRelative;

    [SerializeField] [Range(0f, 100f)] private float lerpSpeed = 0f;

    private void Update()
    {
        if (useLateUpdate) return;
        DoPositionUpdate();
    }

    private void LateUpdate()
    {
        if (!useLateUpdate) return;
        DoPositionUpdate();
    }

    private void DoPositionUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("Trying to do position update but target does not exist anymore.");
            return;
        }

        Vector3 newPosition = target.position + positionOffset;
        newPosition += positionOffsetRelative.x * target.right;
        newPosition += positionOffsetRelative.y * target.up;
        newPosition += positionOffsetRelative.z * target.forward;

        if (lerpSpeed > float.Epsilon) transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * lerpSpeed);
        else transform.position = newPosition;
    }


}
