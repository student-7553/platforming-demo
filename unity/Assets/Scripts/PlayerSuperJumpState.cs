using System;
using UnityEngine;

enum PlayerSuperJumpDirection
{
    JUMP_RIGHT,
    JUMP_LEFT,
    WALL_JUMP_LEFT,
    WALL_JUMP_RIGHT,
}

public class PlayerSuperJumpState : MonoBehaviour
{
    private Rigidbody2D playerRigidbody;
    private PlayerState playerState;

    private PlayerSuperJumpDirection directionState;

    private bool isStateActive;

    public int totalTickCount;
    private int currentTickCount;

    private Vector2 cachedVelocity;

    public Vector2 superDashJumpInitialVelocity;
    public Vector2 superDashJumpTickThrust;

    public Vector2 superDashWallJumpInitialVelocity;
    public Vector2 superDashWallJumpTickThrust;

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

        handleSuperTick();

        if (currentTickCount > totalTickCount)
        {
            playerState.changeState(PlayerPossibleState.NONE);
        }
    }

    private void handleSuperTick()
    {
        float progressScaled = currentTickCount / (float)totalTickCount;
        float curvedPercentageThrust = Mathf.Pow(progressScaled, 2);

        Vector2 force = Vector2.zero;

        if (
            directionState == PlayerSuperJumpDirection.JUMP_RIGHT
            || directionState == PlayerSuperJumpDirection.JUMP_LEFT
        )
        {
            Vector2 thrust =
                superDashJumpTickThrust - (superDashJumpTickThrust * curvedPercentageThrust);

            force =
                directionState == PlayerSuperJumpDirection.JUMP_RIGHT
                    ? new Vector2(thrust.x, thrust.y)
                    : new Vector2(-thrust.x, thrust.y);
        }
        else if (
            directionState == PlayerSuperJumpDirection.WALL_JUMP_LEFT
            || directionState == PlayerSuperJumpDirection.WALL_JUMP_RIGHT
        )
        {
            Vector2 thrust =
                superDashWallJumpTickThrust
                - (superDashWallJumpTickThrust * curvedPercentageThrust);

            force =
                directionState == PlayerSuperJumpDirection.JUMP_RIGHT
                    ? new Vector2(thrust.x, thrust.y)
                    : new Vector2(-thrust.x, thrust.y);
        }

        playerRigidbody.AddForce(force);
    }

    public void stateEnd()
    {
        isStateActive = false;
    }

    private void handleInitialVeloctyBoost()
    {
        Vector2 effectiveAbsoluteInitialVelocity = new Vector2(
            Math.Abs(cachedVelocity.x),
            Math.Abs(cachedVelocity.y)
        );

        if (
            directionState == PlayerSuperJumpDirection.JUMP_RIGHT
            || directionState == PlayerSuperJumpDirection.JUMP_LEFT
        )
        {
            effectiveAbsoluteInitialVelocity =
                effectiveAbsoluteInitialVelocity + superDashJumpInitialVelocity;
            playerRigidbody.velocity = (
                directionState == PlayerSuperJumpDirection.JUMP_RIGHT
                    ? new Vector2(
                        effectiveAbsoluteInitialVelocity.x,
                        effectiveAbsoluteInitialVelocity.y
                    )
                    : new Vector2(
                        -effectiveAbsoluteInitialVelocity.x,
                        effectiveAbsoluteInitialVelocity.y
                    )
            );
            return;
        }
        else if (
            directionState == PlayerSuperJumpDirection.WALL_JUMP_LEFT
            || directionState == PlayerSuperJumpDirection.WALL_JUMP_RIGHT
        )
        {
            effectiveAbsoluteInitialVelocity =
                effectiveAbsoluteInitialVelocity + superDashWallJumpInitialVelocity;

            playerRigidbody.velocity = (
                directionState == PlayerSuperJumpDirection.WALL_JUMP_RIGHT
                    ? new Vector2(
                        effectiveAbsoluteInitialVelocity.x,
                        effectiveAbsoluteInitialVelocity.y
                    )
                    : new Vector2(
                        -effectiveAbsoluteInitialVelocity.x,
                        effectiveAbsoluteInitialVelocity.y
                    )
            );
            return;
        }
    }

    public void stateStart(Vector2 direction, Vector2 _cachedVelocity)
    {
        isStateActive = true;

        currentTickCount = 0;

        cachedVelocity = _cachedVelocity;

        directionState = getInnerDirection(direction);

        handleInitialVeloctyBoost();
    }

    private PlayerSuperJumpDirection getInnerDirection(Vector2 direction)
    {
        if (playerState.playerObserver.observedState == ObservedState.NEAR_LEFT_WALL)
        {
            return PlayerSuperJumpDirection.WALL_JUMP_RIGHT;
        }
        if (playerState.playerObserver.observedState == ObservedState.NEAR_RIGHT_WALL)
        {
            return PlayerSuperJumpDirection.WALL_JUMP_LEFT;
        }

        return direction.x > 0
            ? PlayerSuperJumpDirection.JUMP_RIGHT
            : PlayerSuperJumpDirection.JUMP_LEFT;
    }
}
