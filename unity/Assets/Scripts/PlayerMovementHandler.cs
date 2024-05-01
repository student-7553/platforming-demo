using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct FlowXDetail
{
    [NonSerialized]
    public int xFlowCounter;

    [NonSerialized]
    public float xFlowDirection;

    public int xMaxFlowCounter;

    public float xTickDirectionMultiple;
}

public class PlayerMovementHandler : MonoBehaviour
{
    Rigidbody2D playerRigidbody;
    private float xAccleration;

    [SerializeField]
    public FlowXDetail flowXDetail;

    void Start()
    {
        if (!!Singelton.GetPlayerInputHandler())
        {
            Destroy(this);
            return;
        }
        Singelton.SetPlayerInputHandler(this);
        playerRigidbody = GetComponent<Rigidbody2D>();
    }

    private void OnDestroy()
    {
        Singelton.SetPlayerInputHandler(null);
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

        Vector2 targetPosition = xTickDirection() + (Vector2)playerRigidbody.transform.position;

        playerRigidbody.transform.position = targetPosition;

        // What about the in the air
        // What about in mid dash
    }

    private Vector2 xTickDirection()
    {
        float xDirection = xAccleration;

        float clampedMultiplier = Mathf.Clamp(
            (float)flowXDetail.xFlowCounter / flowXDetail.xMaxFlowCounter,
            0,
            1
        );

        xDirection = xDirection * clampedMultiplier;
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
