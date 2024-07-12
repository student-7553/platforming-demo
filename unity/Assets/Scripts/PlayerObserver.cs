using UnityEngine;

public enum ObservedState
{
    NEAR_LEFT_WALL,
    NEAR_RIGHT_WALL,
    GROUND,
    AIR
}

public class PlayerObserver : MonoBehaviour
{
    private BoxCollider2D playerCollider;

    private float yMargin;
    private float xMargin;

    private PlayerState playerState;

    public ObservedState observedState;
    public bool isDashAvailable;

    public LayerMask layerMask;

    void Start()
    {
        playerCollider = GetComponent<BoxCollider2D>();
        yMargin = (playerCollider.size.y / 2f) - 0.1f;
        xMargin = (playerCollider.size.x / 2f) - 0.1f;

        playerState = GetComponent<PlayerState>();
    }

    private void FixedUpdate()
    {
        observedState = getObservedState();
        handleDashCooldown();
        handleSelfStates();
    }

    public void handleDashCooldown()
    {
        if (observedState != ObservedState.GROUND)
        {
            return;
        }

        if (playerState.currentState == PlayerPossibleState.DASHING)
        {
            return;
        }

        isDashAvailable = true;
    }

    public void dashMark()
    {
        isDashAvailable = false;
    }

    private void handleSelfStates()
    {
        if (
            playerState.isStateChangeOnCooldown
            || (
                playerState.currentState != PlayerPossibleState.NONE
                && playerState.currentState != PlayerPossibleState.FALLING
                && playerState.currentState != PlayerPossibleState.GROUND
                && playerState.currentState != PlayerPossibleState.SLIDING
            )
        )
        {
            return;
        }

        if (observedState == ObservedState.GROUND)
        {
            playerState.changeState(PlayerPossibleState.GROUND);
            return;
        }
        if (
            observedState == ObservedState.NEAR_LEFT_WALL
            && playerState.playerMovementHandler.direction.x < 0
        )
        {
            playerState.changeState(PlayerPossibleState.SLIDING);
            return;
        }
        if (
            observedState == ObservedState.NEAR_RIGHT_WALL
            && playerState.playerMovementHandler.direction.x > 0
        )
        {
            playerState.changeState(PlayerPossibleState.SLIDING);
            return;
        }
        //
        playerState.changeState(PlayerPossibleState.FALLING);
    }

    private ObservedState getObservedState()
    {
        bool isOnGround = computeIsOnGround();
        if (isOnGround)
        {
            return ObservedState.GROUND;
        }
        bool isSlidingLeft = computeIsSlidingLeft();
        if (isSlidingLeft)
        {
            return ObservedState.NEAR_LEFT_WALL;
        }
        bool isSlidingRight = computeIsSlidingRight();
        if (isSlidingRight)
        {
            return ObservedState.NEAR_RIGHT_WALL;
        }

        return ObservedState.AIR;
    }

    private bool computeIsOnGround()
    {
        Vector2 originPosition = new Vector2(
            gameObject.transform.position.x,
            gameObject.transform.position.y - yMargin
        );

        RaycastHit2D rayCastResult = Physics2D.Raycast(
            originPosition,
            Vector2.down,
            0.15f,
            layerMask
        );

        // Debug.DrawRay(originPosition, Vector2.down * 0.25f, Color.red);

        return !!rayCastResult.collider;
    }

    private bool computeIsSlidingRight()
    {
        Vector2 originPosition = new Vector2(
            gameObject.transform.position.x + xMargin,
            gameObject.transform.position.y
        );

        RaycastHit2D rayCastResult = Physics2D.Raycast(
            originPosition,
            Vector2.right,
            0.15f,
            layerMask
        );

        return !!rayCastResult.collider;
    }

    private bool computeIsSlidingLeft()
    {
        Vector2 originPosition = new Vector2(
            gameObject.transform.position.x - xMargin,
            gameObject.transform.position.y
        );

        RaycastHit2D rayCastResult = Physics2D.Raycast(
            originPosition,
            Vector2.left,
            0.15f,
            layerMask
        );

        return !!rayCastResult.collider;
    }
}
