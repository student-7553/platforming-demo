using UnityEngine;

public enum DASH_TYPE
{
    OTHERS,
    PRE_SUPERDASH_OK,
    PRE_WAVEDASH_1,
    PRE_WAVEDASH_OK,
}

public class PlayerDashState : MonoBehaviour
{
    private Rigidbody2D playerRigidbody;
    private PlayerState playerState;

    private bool isStateActive;

    private Vector2 currentDirection;
    public int totalTickCountForDash;

    public float positonAddPerTick;

    private float cachedGravityScale;

    private float currentTickCount;

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
            case ObservedState.SLIDING_LEFT:
            case ObservedState.SLIDING_RIGHT:

                if (effectiveDirection.x != 0 && effectiveDirection.y != 0)
                {
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
                        // PRE_WAVEDASH_OK
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

    // public void
    public void handleJumpActivation() { }

    //

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
        currentDashType = DASH_TYPE.OTHERS;

        playerRigidbody.gravityScale = cachedGravityScale;
    }

    public void stateStart(Vector2 direction)
    {
        isStateActive = true;
        cachedGravityScale = playerRigidbody.gravityScale;

        playerRigidbody.gravityScale = 0;
        currentDirection = direction;

        if (
            currentDirection.x != 0
            && currentDirection.y == 0
            && playerState.playerObserver.observedState == ObservedState.GROUND
        )
        {
            currentDashType = DASH_TYPE.PRE_SUPERDASH_OK;
        }
        else if (currentDirection.x != 0 && currentDirection.y < 0)
        {
            currentDashType = DASH_TYPE.PRE_WAVEDASH_1;
        }
        else
        {
            currentDashType = DASH_TYPE.OTHERS;
        }
    }
}
