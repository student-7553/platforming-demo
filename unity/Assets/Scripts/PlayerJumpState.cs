using System;
using UnityEngine;

public enum PlayerJumpDirection
{
    RIGHT,
    LEFT,
    STILL
}

public class PlayerJumpState : MonoBehaviour
{
    private Rigidbody2D playerRigidbody;

    private PlayerState playerState;

    private bool isStateActive;

    private int tickCounter = 0;
    public int maxJumpingFixedCounter;

    public int singleTickThrust;

    private PlayerJumpDirection direction;
    public Vector2 jumpInitialVelocity;

    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerState = GetComponent<PlayerState>();
    }

    private void FixedUpdate()
    {
        handleJumpTick();
    }

    private void handleJumpTick()
    {
        if (!isStateActive)
        {
            return;
        }

        if (tickCounter == 0)
        {
            handleInitialVeloctyBoost();
        }

        // can be between 0 - 1
        float progressScaled = tickCounter / (float)maxJumpingFixedCounter;
        float curvedPercentageThrust = Mathf.Pow(progressScaled, 2);

        float thrust = singleTickThrust - (singleTickThrust * curvedPercentageThrust);

        Vector2 force = new Vector2(0, thrust);

        playerRigidbody.AddForce(force);

        tickCounter = tickCounter + 1;

        if (tickCounter >= maxJumpingFixedCounter)
        {
            handleJumpEnd();
        }
    }

    private void handleInitialVeloctyBoost()
    {
        Vector2 effectiveJumpInitialVelocity = jumpInitialVelocity;

        if (direction == PlayerJumpDirection.STILL)
        {
            effectiveJumpInitialVelocity.x = 0;
        }
        else if (direction == PlayerJumpDirection.LEFT)
        {
            effectiveJumpInitialVelocity.x = -effectiveJumpInitialVelocity.x;
        }

        playerRigidbody.velocity = playerRigidbody.velocity + effectiveJumpInitialVelocity;
    }

    public void stateEnd()
    {
        isStateActive = false;
    }

    public void handleJumpEnd()
    {
        playerState.changeState(PlayerPossibleState.NONE);
    }

    public void stateStart(Vector2 _direction)
    {
        isStateActive = true;
        tickCounter = 0;

        if (_direction.x == 0)
        {
            direction = PlayerJumpDirection.STILL;
        }
        else if (_direction.x > 0)
        {
            direction = PlayerJumpDirection.RIGHT;
        }
        else
        {
            direction = PlayerJumpDirection.LEFT;
        }
    }
}
