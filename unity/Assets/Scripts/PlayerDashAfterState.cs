using UnityEngine;

public enum DashAfterDirection
{
    SUPER_RIGHT,
    SUPER_LEFT,
}

public enum DashAfterState
{
    NONE,
    SUPER_DASH_JUMP,
    HYPER_DASH_JUMP
}

// Handles super dash and hyper dash, prob better if super and hyper dash gets its own state
public class PlayerDashAfterState : MonoBehaviour
{
    private Rigidbody2D playerRigidbody;
    private PlayerState playerState;
    private DashAfterDirection directionState;

    private bool isStateActive;

    private DashAfterState dashState;

    private int totalTickCount;
    private int currentTickCount;
    private DASH_TYPE preDashType;

    public int graceTickCount;
    public int jumpTotalTickCount;

    public Vector2 superDashJumpInitialVelocity;
    public Vector2 superDashJumpTickThrust;

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

        if (dashState == DashAfterState.SUPER_DASH_JUMP)
        {
            handleSuperTick();
        }
        else if (dashState == DashAfterState.HYPER_DASH_JUMP)
        {
            handleHyperTick();
        }

        if (currentTickCount > totalTickCount)
        {
            playerState.changeState(PlayerPossibleState.NONE);
        }
    }

    private void handleSuperTick()
    {
        float progressScaled = currentTickCount / (float)totalTickCount;
        float curvedPercentageThrust = Mathf.Pow(progressScaled, 2);

        Vector2 thrust =
            superDashJumpTickThrust - (superDashJumpTickThrust * curvedPercentageThrust);

        Vector2 force =
            directionState == DashAfterDirection.SUPER_RIGHT
                ? new Vector2(thrust.x, thrust.y)
                : new Vector2(-thrust.x, thrust.y);

        playerRigidbody.AddForce(force);
    }

    private void handleHyperTick()
    {
        float progressScaled = currentTickCount / (float)totalTickCount;
        float curvedPercentageThrust = Mathf.Pow(progressScaled, 2);

        Vector2 thrust =
            hyperDashJumpTickThrust - (hyperDashJumpTickThrust * curvedPercentageThrust);

        Vector2 force =
            directionState == DashAfterDirection.SUPER_RIGHT
                ? new Vector2(thrust.x, thrust.y)
                : new Vector2(-thrust.x, thrust.y);

        playerRigidbody.AddForce(force);
    }

    public void stateEnd()
    {
        isStateActive = false;
        dashState = DashAfterState.NONE;
    }

    public void handleJumpActivation()
    {
        if (dashState != DashAfterState.NONE)
        {
            return;
        }

        if (playerState.playerObserver.observedState != ObservedState.GROUND)
        {
            return;
        }

        switch (preDashType)
        {
            case DASH_TYPE.PRE_SUPERDASH_OK:
                dashState = DashAfterState.SUPER_DASH_JUMP;
                totalTickCount = jumpTotalTickCount + currentTickCount;
                playerRigidbody.velocity = (
                    directionState == DashAfterDirection.SUPER_RIGHT
                        ? new Vector2(
                            superDashJumpInitialVelocity.x,
                            superDashJumpInitialVelocity.y
                        )
                        : new Vector2(
                            -superDashJumpInitialVelocity.x,
                            superDashJumpInitialVelocity.y
                        )
                );
                break;
            case DASH_TYPE.PRE_HYPERDASH_OK:
                dashState = DashAfterState.HYPER_DASH_JUMP;
                totalTickCount = jumpTotalTickCount + currentTickCount;
                playerRigidbody.velocity = (
                    directionState == DashAfterDirection.SUPER_RIGHT
                        ? new Vector2(
                            hyperDashJumpInitialVelocity.x,
                            hyperDashJumpInitialVelocity.y
                        )
                        : new Vector2(
                            -hyperDashJumpInitialVelocity.x,
                            hyperDashJumpInitialVelocity.y
                        )
                );
                break;
        }
    }

    public void stateStart(Vector2 direction, DASH_TYPE _preDashType)
    {
        isStateActive = true;

        currentTickCount = 0;

        totalTickCount = graceTickCount;

        directionState =
            direction.x > 0 ? DashAfterDirection.SUPER_RIGHT : DashAfterDirection.SUPER_LEFT;

        preDashType = _preDashType;
    }
}
