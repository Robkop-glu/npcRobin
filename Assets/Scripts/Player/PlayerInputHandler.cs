using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private CharacterMotor motor;
    private LookController lookController;
    private PlayerController playerController;

    private bool holdingShoot = false;

    private void Awake()
    {
        motor = GetComponent<CharacterMotor>();
        lookController = GetComponent<LookController>();
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (holdingShoot) {
            playerController.OnShootInput();
        }


    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        Vector2 movementInput = context.ReadValue<Vector2>();
        if (Mathf.Approximately(movementInput.magnitude, 0f))
        {
            motor.StopMovement();
        }
        else
        {
            movementInput.Normalize();
            motor.MoveInDirection(movementInput, CharacterMotor.MovementState.Run);
        }
    }

    public void OnLookInput(InputAction.CallbackContext context) { 
    
        Vector2 lookInput = context.ReadValue<Vector2>();
        if (Mathf.Approximately(lookInput.magnitude, 0f)) return;

        lookController.OnLookInput(lookInput);
    }

    public void OnShootInput(InputAction.CallbackContext context)
    {
        if (context.performed) holdingShoot = true;
        else if(context.canceled) holdingShoot = false;
    }

    public void OnMouseLockInput(InputAction.CallbackContext context)
    {
        if (context.performed) playerController.ToggleMouseLock();
    }

}
