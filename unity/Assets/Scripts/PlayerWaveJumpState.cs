using System;
using UnityEngine;

public enum PlayerWaveJumpDirection
{
    RIGHT,
    LEFT,
}

public class PlayerWaveJumpState : MonoBehaviour
{
    private Rigidbody2D playerRigidbody;
    private PlayerState playerState;

    private bool isStateActive;
    private PlayerWaveJumpDirection direction;

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
                direction == PlayerWaveJumpDirection.RIGHT
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
            direction == PlayerWaveJumpDirection.RIGHT ? thrust : new Vector2(-thrust.x, thrust.y);

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
        direction = _direction.x > 0 ? PlayerWaveJumpDirection.RIGHT : PlayerWaveJumpDirection.LEFT;
    }
}
