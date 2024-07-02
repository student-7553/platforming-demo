using System;
using UnityEngine;

[Serializable]
public struct FlowXDetail
{
    [NonSerialized]
    public int xFlowCounter;

    [NonSerialized]
    public float xFlowDirection;

    [Tooltip("Speed ramp linear (Higher means slower speed ramp)")]
    public int xMaxFlowCounter;

    [Tooltip("Top speed")]
    public float xTickDirectionMultiple;
}

public class PlayerMovementHandler : MonoBehaviour
{
    private Rigidbody2D playerRigidbody;

    public Vector2 direction;

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

        Vector2 targetPosition = xTickDir + (Vector2)playerRigidbody.transform.position;
        playerRigidbody.transform.position = targetPosition;
    }

    private Vector2 xTickDirection()
    {
        float clampedMultiplier = Mathf.Clamp(
            (float)flowXDetail.xFlowCounter / flowXDetail.xMaxFlowCounter,
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
        if (flowXDetail.xFlowDirection != direction.x)
        {
            flowXDetail.xFlowDirection = direction.x;
            flowXDetail.xFlowCounter = 0;
        }
        direction = directionInput;
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
