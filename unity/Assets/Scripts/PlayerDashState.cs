using UnityEngine;

public class PlayerDashState : MonoBehaviour
{
    private Rigidbody2D playerRigidbody;
    private PlayerState playerState;
    private PlayerMovementHandler playerMovementHandler;
    private bool isStateActive;

    private Vector2 currentDirection;
    public int totalTickCountForDash;

    public float positonAddPerTick;

    private float cachedGravityScale;

    private float currentTickCount;

    void Start()
    {
        playerState = GetComponent<PlayerState>();
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerMovementHandler = GetComponent<PlayerMovementHandler>();
    }

    private Vector2 getCurrentDirectionVector()
    {
        Vector2 directionVector = Vector2.zero;
        if (currentDirection.x != 0)
        {
            directionVector.x = positonAddPerTick * currentDirection.x;
        }

        if (currentDirection.y != 0)
        {
            directionVector.y = positonAddPerTick * currentDirection.y;
        }
        return directionVector;
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
            playerState.changeState(
                PlayerPossibleState.GROUND,
                PlayerStateSource.DASH_STATE,
                false
            );
        }
    }

    public void stateEnd()
    {
        isStateActive = false;
        currentTickCount = 0;

        playerRigidbody.gravityScale = cachedGravityScale;
        playerMovementHandler.handleUnDisableMovement();
    }

    public void stateStart(Vector2 direction)
    {
        isStateActive = true;
        cachedGravityScale = playerRigidbody.gravityScale;

        playerRigidbody.gravityScale = 0;
        currentDirection = direction;

        playerMovementHandler.handleDisableMovement();
    }
}
