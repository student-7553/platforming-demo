using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public InputActionReference jumpAction;
    public InputActionReference dashAction;
    public InputActionReference movement2dAction;

    private void Start()
    {
        jumpAction.action.performed += OnJumpAction;
        dashAction.action.performed += OnDashAction;
        movement2dAction.action.performed += OnMovementActionPerformed;
        movement2dAction.action.canceled += OnMovementActionCanceled;
    }

    private void OnDisable()
    {
        jumpAction.action.performed -= OnJumpAction;
        dashAction.action.performed -= OnDashAction;
        movement2dAction.action.performed -= OnMovementActionPerformed;
        movement2dAction.action.canceled -= OnMovementActionCanceled;
    }

    public void OnJumpAction(InputAction.CallbackContext context)
    {
        bool isButtonDown = context.ReadValueAsButton();
        if (isButtonDown)
        {
            Singelton.GetPlayer().playerstate.handleJumpAction();
        }
        else
        {
            Singelton.GetPlayer().playerstate.handleJumpActionEnd();
        }
    }

    public void OnDashAction(InputAction.CallbackContext context)
    {
        Singelton.GetPlayer().playerstate.handleDashAction();
    }

    public void OnMovementActionPerformed(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();
        Singelton.GetPlayer().playerstate.playerMovementHandler.handlePlayerDirectionInput(value);
    }

    public void OnMovementActionCanceled(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();
        Singelton.GetPlayer().playerstate.playerMovementHandler.handlePlayerDirectionInput(value);
    }
}
