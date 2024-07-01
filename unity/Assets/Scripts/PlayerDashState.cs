using UnityEngine;

public class PlayerDashState : MonoBehaviour
{
    private Rigidbody2D playerRigidbody;
    private PlayerState playerState;
    private PlayerMovementHandler playerMovementHandler;
    private bool isStateActive;

    private Direction currentDirection;
    public int totalTickCountForDash;
    public Vector2 positonAddPerTick;

    private float cachedGravityScale;

    private float currentTickCount;

    void Start()
    {
        playerState = GetComponent<PlayerState>();
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerMovementHandler = GetComponent<PlayerMovementHandler>();
    }

    private void FixedUpdate()
    {
        if (!isStateActive)
        {
            return;
        }
        Vector2 directionTick = positonAddPerTick * (currentDirection == Direction.RIGHT ? 1 : -1);
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

    public void stateStart(Direction direction)
    {
        isStateActive = true;
        cachedGravityScale = playerRigidbody.gravityScale;

        playerRigidbody.gravityScale = 0;
        currentDirection = direction;

        playerMovementHandler.handleDisableMovement();
    }
}
