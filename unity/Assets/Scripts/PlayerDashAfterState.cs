using UnityEngine;

public enum DashAfterDirection
{
    SUPER_RIGHT,
    SUPER_LEFT,
}

public class PlayerDashAfterState : MonoBehaviour
{
    private Rigidbody2D playerRigidbody;
    private PlayerState playerState;
    private DashAfterDirection directionState;

    private bool isStateActive;

    private bool isActivated;
    private int totalTickCount;
    private int currentTickCount;

    public int graceTickCount;
    public int jumpTotalTickCount;

    public Vector2 jumpInitialVelocity;
    public Vector2 jumpTickThrust;

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
        if (isActivated)
        {
            // can be between 0 - 1
            float progressScaled = currentTickCount / (float)totalTickCount;
            float curvedPercentageThrust = Mathf.Pow(progressScaled, 2);

            Vector2 thrust = jumpTickThrust - (jumpTickThrust * curvedPercentageThrust);

            Vector2 force =
                directionState == DashAfterDirection.SUPER_RIGHT
                    ? new Vector2(thrust.x, thrust.y)
                    : new Vector2(-thrust.x, thrust.y);

            playerRigidbody.AddForce(force);
        }
        if (currentTickCount > totalTickCount)
        {
            playerState.changeState(PlayerPossibleState.NONE);
        }
    }

    public void stateEnd()
    {
        isStateActive = false;
        isActivated = false;

        // need to cap the velocity if lower than a certain threshold
        // playerRigidbody.gravityScale = cachedGravityScale;
    }

    public void handleJumpActivation()
    {
        if (isActivated)
        {
            return;
        }
        isActivated = true;

        totalTickCount = jumpTotalTickCount + currentTickCount;

        // Todo: dash needs to save the velocity tbh
        // playerRigidbody.velocity = (
        //     directionState == DashAfterDirection.SUPER_RIGHT
        //         ? new Vector2(playerRigidbody.velocity.x, jumpInitialVelocity.y)
        //         : new Vector2(-playerRigidbody.velocity.x, jumpInitialVelocity.y)
        // );
        playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, 4);

        Debug.Log("Triggered...");

        // playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, jumpInitialVelocity);
        // jumpInitialVelocity
    }

    public void stateStart(Vector2 direction)
    {
        isStateActive = true;
        currentTickCount = 0;
        totalTickCount = graceTickCount;

        directionState =
            direction.x > 0 ? DashAfterDirection.SUPER_RIGHT : DashAfterDirection.SUPER_LEFT;

        // cachedGravityScale = playerRigidbody.gravityScale;

        // playerRigidbody.gravityScale = 0;
        // currentDirection = direction;
    }
}
