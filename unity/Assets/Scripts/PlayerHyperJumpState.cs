using System;
using UnityEngine;

public enum PlayerHyperJumpDirection
{
    RIGHT,
    LEFT,
}

public class PlayerHyperJumpState : MonoBehaviour
{
    private Rigidbody2D playerRigidbody;
    private PlayerState playerState;

    private PlayerHyperJumpDirection directionState;

    private bool isStateActive;

    public int totalTickCount;
    private int currentTickCount;

    private Vector2 cachedVelocity;

    public Vector2 hyperDashJumpInitialVelocity;
    public Vector2 hyperDashJumpTickThrust;

    void Start()
    {
        playerState = GetComponent<PlayerState>();
        playerRigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (!isStateActive)
        {
            return;
        }

        currentTickCount = currentTickCount + 1;

        handleHyperTick();

        if (currentTickCount > totalTickCount)
        {
            playerState.changeState(PlayerPossibleState.NONE);
        }
    }

    private void handleHyperTick()
    {
        float progressScaled = currentTickCount / (float)totalTickCount;
        float curvedPercentageThrust = Mathf.Pow(progressScaled, 2);

        Vector2 thrust =
            hyperDashJumpTickThrust - (hyperDashJumpTickThrust * curvedPercentageThrust);

        Vector2 force =
            directionState == PlayerHyperJumpDirection.RIGHT
                ? new Vector2(thrust.x, thrust.y)
                : new Vector2(-thrust.x, thrust.y);

        playerRigidbody.AddForce(force);
    }

    public void stateEnd()
    {
        isStateActive = false;
    }

    public void handleInitialVeloctyBoost()
    {
        Vector2 effectiveAbsoluteInitialVelocity = new Vector2(
            Math.Abs(cachedVelocity.x),
            Math.Abs(cachedVelocity.y)
        );

        effectiveAbsoluteInitialVelocity =
            effectiveAbsoluteInitialVelocity + hyperDashJumpInitialVelocity;

        playerRigidbody.velocity = (
            directionState == PlayerHyperJumpDirection.RIGHT
                ? new Vector2(
                    effectiveAbsoluteInitialVelocity.x,
                    effectiveAbsoluteInitialVelocity.y
                )
                : new Vector2(
                    -effectiveAbsoluteInitialVelocity.x,
                    effectiveAbsoluteInitialVelocity.y
                )
        );
    }

    public void stateStart(Vector2 direction, Vector2 _cachedVelocity)
    {
        isStateActive = true;

        currentTickCount = 0;

        cachedVelocity = _cachedVelocity;

        directionState =
            direction.x > 0 ? PlayerHyperJumpDirection.RIGHT : PlayerHyperJumpDirection.LEFT;

        handleInitialVeloctyBoost();
    }
}
