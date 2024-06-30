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

public enum Direction
{
    RIGHT,
    LEFT
}

public class PlayerMovementHandler : MonoBehaviour
{
    Rigidbody2D playerRigidbody;
    private float xAccleration;
    public Direction direction;

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
        if (xAccleration == 0)
        {
            return;
        }
        Vector2 xTickDir = xTickDirection();
        if (xTickDir.x > 0)
        {
            direction = Direction.RIGHT;
        }
        else
        {
            direction = Direction.LEFT;
        }

        Vector2 targetPosition = xTickDir + (Vector2)playerRigidbody.transform.position;
        playerRigidbody.transform.position = targetPosition;

        // What about the in the air
        // What about in mid dash
    }

    private Vector2 xTickDirection()
    {
        float clampedMultiplier = Mathf.Clamp(
            (float)flowXDetail.xFlowCounter / flowXDetail.xMaxFlowCounter,
            0,
            1
        );

        float xDirection = xAccleration * clampedMultiplier;

        flowXDetail.xFlowCounter = flowXDetail.xFlowCounter + 1;

        Vector2 targetPosition = new Vector2(xDirection * flowXDetail.xTickDirectionMultiple, 0);

        return targetPosition;
    }

    public void handlePlayerXDirection(float direction)
    {
        if (flowXDetail.xFlowDirection != direction)
        {
            flowXDetail.xFlowDirection = direction;
            flowXDetail.xFlowCounter = 0;
        }

        xAccleration = direction;
    }
}
