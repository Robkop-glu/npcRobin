using Unity.Cinemachine;
using UnityEngine;

public class FirstPersonLookController : LookController
{
    [SerializeField] private float horizontalRotationSpeedBase = 10f;
    [SerializeField] private float verticalRotationSpeedBase = 10f;

    [SerializeField] private Transform graphics;
    [SerializeField] private CameraInfo cam;
    [SerializeField] private float maxAngleVerticalDegrees = 80f;
   

    public override void OnLookInput(Vector2 input)
    {
        if (!cam.CurrentlyActive) return;

        if (Mathf.Abs(input.x) > float.Epsilon)
        {
            graphics.Rotate(Vector3.up, input.x *  horizontalRotationSpeedBase * Time.deltaTime);
        }
        if (Mathf.Abs (input.y) > float.Epsilon)
        {
            float currentEulerX = cam.transform.localRotation.eulerAngles.x;
            // Convert 0..360 into -180..180
            if (currentEulerX > 180f) currentEulerX -= 360f;
            float rotAmount = input.y * verticalRotationSpeedBase * Time.deltaTime;
            float newXRot = Mathf.Clamp(currentEulerX - rotAmount, -maxAngleVerticalDegrees, maxAngleVerticalDegrees);
            cam.transform.localRotation = Quaternion.Euler(newXRot, 0f, 0f);
        }
    }
}
