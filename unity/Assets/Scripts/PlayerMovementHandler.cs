using System;
using UnityEngine;

[Serializable]
public struct FlowXDetail
{
    [NonSerialized]
    public int xFlowCounter;

    [Tooltip("Speed ramp linear (Higher means slower speed ramp)")]
    public int xMaxFlowCounter;

    [Tooltip("Top speed")]
    public float xTickDirectionMultiple;
}

public enum LookingDirection
{
    LEFT,
    RIGHT,
}

public class PlayerMovementHandler : MonoBehaviour
{
    private Rigidbody2D playerRigidbody;

    public Vector2 direction;
    public LookingDirection lookingDirection;

    public int velocityLimit;

    private bool isDisabled;

    [SerializeField]
    public FlowXDetail flowXDetail;

    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        handleXTick();
    }

    private void handleXTick()
    {
        if (direction.x == 0 || isDisabled)
        {
            return;
        }

        Vector2 xTickDir = xTickDirection();

        Vector2 newVelocity = playerRigidbody.velocity + xTickDir;

        newVelocity.x = Mathf.Clamp(
            newVelocity.x,
            Math.Min(-velocityLimit, playerRigidbody.velocity.x),
            Math.Max(velocityLimit, playerRigidbody.velocity.x)
        );

        playerRigidbody.velocity = newVelocity;

        flowXDetail.xFlowCounter = flowXDetail.xFlowCounter + 1;
    }

    private Vector2 xTickDirection()
    {
        float xDirection = direction.x > 0 ? 1 : -1;

        Vector2 targetPosition = new Vector2(xDirection * flowXDetail.xTickDirectionMultiple, 0);

        return targetPosition;
    }

    public void handlePlayerDirectionInput(Vector2 directionInput)
    {
        if (direction.x == 0 && directionInput.x != 0)
        {
            flowXDetail.xFlowCounter = 0;
        }
        else if (direction.x > 0 && directionInput.x <= 0)
        {
            flowXDetail.xFlowCounter = 0;
        }
        else if (direction.x < 0 && directionInput.x >= 0)
        {
            flowXDetail.xFlowCounter = 0;
        }

        direction = directionInput;

        if (direction.x != 0)
        {
            lookingDirection = direction.x > 0 ? LookingDirection.RIGHT : LookingDirection.LEFT;
        }
    }

    public void handleDisableMovement()
    {
        isDisabled = true;
    }

    public void handleUnDisableMovement()
    {
        isDisabled = false;
    }
}
