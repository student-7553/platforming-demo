using System;
using UnityEngine;

enum PlayerWaveJumpDirection
{
    JUMP_RIGHT,
    JUMP_LEFT,
    WALL_JUMP_LEFT,
    WALL_JUMP_RIGHT,
}

public class PlayerWaveJumpState : MonoBehaviour
{
    private Rigidbody2D playerRigidbody;
    private PlayerState playerState;

    private bool isStateActive;
    private PlayerWaveJumpDirection directionState;

    private int currentTickCount;

    public int maxTickCounter;

    public Vector2 jumpInitialVelocity;
    public Vector2 jumpTickThrust;

    private Vector2 cachedVelocity;

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
        if (currentTickCount == 0)
        {
            Vector2 effectiveAbsoluteInitialVelocity =
                jumpInitialVelocity
                + new Vector2(Math.Abs(cachedVelocity.x), Math.Abs(cachedVelocity.y));

            playerRigidbody.velocity =
                (
                    directionState == PlayerWaveJumpDirection.JUMP_RIGHT
                    || directionState == PlayerWaveJumpDirection.WALL_JUMP_RIGHT
                )
                    ? effectiveAbsoluteInitialVelocity
                    : new Vector2(
                        -effectiveAbsoluteInitialVelocity.x,
                        effectiveAbsoluteInitialVelocity.y
                    );
        }

        // can be between 0 - 1
        float progressScaled = currentTickCount / (float)maxTickCounter;
        float curvedPercentageThrust = Mathf.Pow(progressScaled, 2);

        Vector2 thrust = jumpTickThrust - (jumpTickThrust * curvedPercentageThrust);

        Vector2 force =
            (
                directionState == PlayerWaveJumpDirection.JUMP_RIGHT
                || directionState == PlayerWaveJumpDirection.WALL_JUMP_RIGHT
            )
                ? thrust
                : new Vector2(-thrust.x, thrust.y);

        playerRigidbody.AddForce(force);

        currentTickCount = currentTickCount + 1;

        if (currentTickCount >= maxTickCounter)
        {
            playerState.changeState(PlayerPossibleState.NONE);
        }
    }

    public void stateEnd()
    {
        isStateActive = false;
        currentTickCount = 0;
    }

    public void stateStart(Vector2 _direction, Vector2 _cachedVelocity)
    {
        isStateActive = true;
        cachedVelocity = _cachedVelocity;
        directionState = getInnerDirection(_direction);
    }

    private PlayerWaveJumpDirection getInnerDirection(Vector2 direction)
    {
        if (playerState.playerObserver.observedState == ObservedState.NEAR_LEFT_WALL)
        {
            return PlayerWaveJumpDirection.WALL_JUMP_RIGHT;
        }
        if (playerState.playerObserver.observedState == ObservedState.NEAR_RIGHT_WALL)
        {
            return PlayerWaveJumpDirection.WALL_JUMP_LEFT;
        }

        return direction.x > 0
            ? PlayerWaveJumpDirection.JUMP_RIGHT
            : PlayerWaveJumpDirection.JUMP_LEFT;
    }
}
