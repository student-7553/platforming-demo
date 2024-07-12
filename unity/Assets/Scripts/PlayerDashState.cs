using UnityEngine;

public enum DASH_TYPE
{
    OTHERS,

    // Ready for superdash
    PRE_SUPERDASH_OK,

    // Ready for hyperdash
    PRE_HYPERDASH_OK,

    // wave dash check 1
    PRE_WAVEDASH_1,

    // Ready for wavedash
    PRE_WAVEDASH_OK,
}

public class PlayerDashState : MonoBehaviour
{
    private Rigidbody2D playerRigidbody;
    private PlayerState playerState;

    private bool isStateActive;
    private Vector2 currentDirection;
    private float cachedGravityScale;

    public Vector2 cachedVelocity;

    private int currentTickCount;

    public int totalTickCountForDash;
    public float positonAddPerTick;

    public DASH_TYPE currentDashType;

    void Start()
    {
        playerState = GetComponent<PlayerState>();
        playerRigidbody = GetComponent<Rigidbody2D>();
    }

    private Vector2 getCurrentDirectionVector()
    {
        Vector2 effectiveDirection = currentDirection;

        switch (playerState.playerObserver.observedState)
        {
            case ObservedState.NEAR_LEFT_WALL:
            case ObservedState.NEAR_RIGHT_WALL:

                if (effectiveDirection.x != 0 && effectiveDirection.y != 0)
                {
                    if (currentDashType == DASH_TYPE.PRE_WAVEDASH_1)
                    {
                        currentDashType = DASH_TYPE.PRE_WAVEDASH_OK;
                    }
                    if (effectiveDirection.y > 0)
                    {
                        // upward
                        effectiveDirection = new Vector2(0, 1);
                    }
                    else
                    {
                        effectiveDirection = new Vector2(0, -1);
                    }
                }
                break;
            case ObservedState.GROUND:
                if (effectiveDirection.x != 0 && effectiveDirection.y < 0)
                {
                    if (currentDashType == DASH_TYPE.PRE_WAVEDASH_1)
                    {
                        currentDashType = DASH_TYPE.PRE_WAVEDASH_OK;
                    }

                    if (effectiveDirection.x > 0)
                    {
                        // upward
                        effectiveDirection = new Vector2(1, 0);
                    }
                    else
                    {
                        effectiveDirection = new Vector2(-1, 0);
                    }
                }
                break;
        }

        Vector2 directionVector = Vector2.zero;

        if (effectiveDirection.x != 0)
        {
            directionVector.x = positonAddPerTick * effectiveDirection.x;
        }

        if (currentDirection.y != 0)
        {
            directionVector.y = positonAddPerTick * effectiveDirection.y;
        }

        return directionVector;
    }

    public void handleJumpActivation()
    {
        switch (currentDashType)
        {
            case DASH_TYPE.PRE_HYPERDASH_OK:
                if (playerState.playerObserver.observedState == ObservedState.GROUND)
                {
                    playerState.changeState(PlayerPossibleState.HYPER_DASH_JUMP);
                }
                break;
            case DASH_TYPE.PRE_WAVEDASH_OK:
                if (
                    playerState.playerObserver.observedState == ObservedState.GROUND
                    || playerState.playerObserver.observedState == ObservedState.NEAR_LEFT_WALL
                    || playerState.playerObserver.observedState == ObservedState.NEAR_RIGHT_WALL
                )
                {
                    playerState.changeState(PlayerPossibleState.WAVE_DASH_JUMP);
                }
                break;
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
        }
    }

    private void FixedUpdate()
    {
        if (!isStateActive)
        {
            return;
        }

        Vector2 directionTick = getCurrentDirectionVector();

        Vector2 newPosition = (Vector2)gameObject.transform.position + directionTick;

        playerRigidbody.MovePosition(newPosition);

        currentTickCount++;
        if (currentTickCount > totalTickCountForDash)
        {
            playerState.changeState(PlayerPossibleState.DASHING_AFTER);
        }
    }

    public void stateEnd()
    {
        isStateActive = false;
        currentTickCount = 0;

        playerRigidbody.gravityScale = cachedGravityScale;

        playerRigidbody.velocity = Vector2.zero;
    }

    public void stateStart(Vector2 direction)
    {
        isStateActive = true;
        currentDirection = direction;

        cachedGravityScale = playerRigidbody.gravityScale;

        playerRigidbody.gravityScale = 0;

        playerRigidbody.velocity = Vector2.zero;

        if (currentDirection.x == 0 || currentDirection.y == 0)
        {
            currentDashType = DASH_TYPE.PRE_SUPERDASH_OK;
        }
        else if (
            playerState.playerObserver.observedState == ObservedState.GROUND
            && currentDirection.x != 0
            && currentDirection.y < 0
        )
        {
            currentDashType = DASH_TYPE.PRE_HYPERDASH_OK;
        }
        else if (currentDirection.x != 0 && currentDirection.y != 0)
        {
            currentDashType = DASH_TYPE.PRE_WAVEDASH_1;
        }
        else
        {
            currentDashType = DASH_TYPE.OTHERS;
        }
    }
}
