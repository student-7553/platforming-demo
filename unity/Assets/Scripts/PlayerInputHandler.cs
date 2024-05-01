using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public InputActionReference movementAction;

    private void Start()
    {
        movementAction.action.performed += OnXMovementAction;
    }

    private void OnDisable()
    {
        movementAction.action.performed -= OnXMovementAction;
    }

    public void OnXMovementAction(InputAction.CallbackContext context)
    {
        float xDirection = context.ReadValue<float>();
        Singelton.GetPlayerInputHandler().handlePlayerXDirection(xDirection);
    }
}
