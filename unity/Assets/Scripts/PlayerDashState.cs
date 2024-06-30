using UnityEngine;

public class PlayerDashState : MonoBehaviour
{
    private Rigidbody2D playerRigidbody;
    private PlayerState playerState;
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
            playerState.changeState(PlayerPossibleState.GROUND, PlayerStateSource.DASH_STATE);
        }
    }

    public void stateEnd()
    {
        isStateActive = false;
        currentTickCount = 0;
        playerRigidbody.gravityScale = cachedGravityScale;
    }

    public void stateStart(Direction direction)
    {
        isStateActive = true;
        gameObject.transform.position = gameObject.transform.position;

        cachedGravityScale = playerRigidbody.gravityScale;

        playerRigidbody.gravityScale = 0;

        currentDirection = direction;
    }
}
