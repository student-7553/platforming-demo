using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public InputActionReference movementAction;
    public InputActionReference jumpAction;
    public InputActionReference dashAction;

    private void Start()
    {
        movementAction.action.performed += OnXMovementAction;
        jumpAction.action.performed += OnJumpAction;
        dashAction.action.performed += OnDashAction;
    }

    private void OnDisable()
    {
        movementAction.action.performed -= OnXMovementAction;
        jumpAction.action.performed -= OnJumpAction;
        dashAction.action.performed -= OnDashAction;
    }

    public void OnJumpAction(InputAction.CallbackContext context)
    {
        bool isButtonDown = context.ReadValueAsButton();
        //
        if (isButtonDown)
        {
            Singelton.GetPlayerState().handleJumpAction();
        }
        else
        {
            Singelton.GetPlayerState().handleJumpActionEnd();
        }
    }

    public void OnDashAction(InputAction.CallbackContext context)
    {
        // Singelton.GetPlayerState()?.handleJumpAction();
    }

    public void OnXMovementAction(InputAction.CallbackContext context)
    {
        float xDirection = context.ReadValue<float>();
        Singelton.GetPlayerState().handleXMovement(xDirection);
    }
}
