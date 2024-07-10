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

        // Vector2 targetPosition = xTickDir + (Vector2)playerRigidbody.transform.position;
        // playerRigidbody.transform.position = targetPosition;
        // playerRigidbody.MovePosition(targetPosition);

        // What should I do
        // Initialization boost
        // or do a ramping down tick forces?


        // How will this interact with wall jump

        // Maybe when we switch direction we also flip the velocity


        // Todo fix this
        // Debug.Log(xTickDir);
        playerRigidbody.AddForce(xTickDir);
    }

    private Vector2 xTickDirection()
    {
        float clampedMultiplier = Mathf.Clamp(
            1f - ((float)flowXDetail.xFlowCounter / flowXDetail.xMaxFlowCounter),
            0,
            1
        );

        float xDirection = (direction.x > 0 ? 1 : -1) * clampedMultiplier;

        flowXDetail.xFlowCounter = flowXDetail.xFlowCounter + 1;

        Vector2 targetPosition = new Vector2(xDirection * flowXDetail.xTickDirectionMultiple, 0);

        return targetPosition;
    }

    public void handlePlayerDirectionInput(Vector2 directionInput)
    {
        // 3 stages: 0, -1, 1
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
