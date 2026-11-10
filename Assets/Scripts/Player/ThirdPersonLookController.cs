using UnityEngine;

public class ThirdPersonLookController : LookController
{
    [SerializeField] private Transform graphics;
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private float horizontalRotationSpeed = 10f;
    [SerializeField] private float verticalRotationSpeed = 10f;
    [SerializeField] private float maxAngleVerticalDegrees = 80f;

    public override void OnLookInput(Vector2 input)
    {
        if (Mathf.Abs(input.x) > float.Epsilon)
        {
            graphics.Rotate(Vector3.up, input.x * horizontalRotationSpeed * Time.deltaTime);
        }

        if (Mathf.Abs(input.y) > float.Epsilon)
        {
            float currentEulerX = cameraPivot.localRotation.eulerAngles.x;
            if (currentEulerX > 180f) currentEulerX -= 360f;

            float rotAmount = input.y * verticalRotationSpeed * Time.deltaTime;
            float newXRot = Mathf.Clamp(currentEulerX - rotAmount, -maxAngleVerticalDegrees, maxAngleVerticalDegrees);
            cameraPivot.localRotation = Quaternion.Euler(newXRot, 0f, 0f);
        }
    }
}
