using UnityEngine;

public enum DashAfterDirection
{
    SUPER_RIGHT,
    SUPER_LEFT,
}

public enum DashAfterState
{
    NONE,
    HYPER_DASH_JUMP
}

public class PlayerDashAfterState : MonoBehaviour
{
    private PlayerState playerState;

    private bool isStateActive;

    public int totalTickCount;
    private int currentTickCount;
    private DASH_TYPE preDashType;

    void Start()
    {
        playerState = GetComponent<PlayerState>();
    }

    private void FixedUpdate()
    {
        if (!isStateActive)
        {
            return;
        }

        currentTickCount = currentTickCount + 1;

        if (currentTickCount > totalTickCount)
        {
            playerState.changeState(PlayerPossibleState.NONE);
        }
    }

    public void stateEnd()
    {
        isStateActive = false;
    }

    public void handleJumpActivation()
    {
        switch (preDashType)
        {
            case DASH_TYPE.PRE_SUPERDASH_OK:
                if (
                    playerState.playerObserver.observedState == ObservedState.GROUND
                    || playerState.playerObserver.observedState == ObservedState.NEAR_LEFT_WALL
                    || playerState.playerObserver.observedState == ObservedState.NEAR_RIGHT_WALL
                )
                {
                    playerState.changeState(PlayerPossibleState.SUPER_DASH_JUMP);
                }
                break;
            case DASH_TYPE.PRE_HYPERDASH_OK:
                if (playerState.playerObserver.observedState == ObservedState.GROUND)
                {
                    playerState.changeState(PlayerPossibleState.HYPER_DASH_JUMP);
                }
                break;
        }
    }

    public void stateStart(DASH_TYPE _preDashType)
    {
        isStateActive = true;
        currentTickCount = 0;
        preDashType = _preDashType;
    }
}
