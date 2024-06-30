using UnityEngine;
using System.Collections;
using System.Linq;

public enum PlayerPossibleState
{
    GROUND,
    FALLING,
    DASHING,
    JUMPING,
}

public enum PlayerStateSource
{
    GROUND_OBSERVER,
    DASH_STATE,
    JUMP_STATE,
    INPUT,
}

public class PlayerState : MonoBehaviour
{
    [SerializeField]
    private PlayerPossibleState currentState;

    private Rigidbody2D playerRigidbody;

    private PlayerGroundObserver playerGroundObserver;

    private PlayerMovementHandler playerMovementHandler;

    private PlayerJumpState playerJumpHandler;

    private PlayerDashState playerDashState;

    public bool isStateChangeOnCooldown;

    public int maxYVelocity;

    void Start()
    {
        if (!!Singelton.GetPlayerState())
        {
            Destroy(this);
            return;
        }
        Singelton.SetPlayerState(this);

        playerRigidbody = GetComponent<Rigidbody2D>();
        playerGroundObserver = GetComponent<PlayerGroundObserver>();
        playerMovementHandler = GetComponent<PlayerMovementHandler>();
        playerJumpHandler = GetComponent<PlayerJumpState>();
        playerDashState = GetComponent<PlayerDashState>();

        currentState = PlayerPossibleState.GROUND;
    }

    private void OnDestroy()
    {
        Singelton.SetPlayerState(null);
    }

    public void handleJumpAction()
    {
        changeState(PlayerPossibleState.JUMPING, PlayerStateSource.INPUT);
    }

    public void handleDashAction()
    {
        changeState(PlayerPossibleState.DASHING, PlayerStateSource.INPUT);
    }

    public void handleJumpActionEnd()
    {
        playerJumpHandler.handleJumpEnd();
    }

    public void handleXMovement(float direction)
    {
        if (currentState == PlayerPossibleState.DASHING)
        {
            return;
        }
        playerMovementHandler.handlePlayerXDirection(direction);
    }

    private void FixedUpdate()
    {
        clampVelocity();
    }

    private bool isAllowedToChangeStateTo(PlayerPossibleState newState, PlayerStateSource source)
    {
        if (isStateChangeOnCooldown || newState == currentState)
        {
            return false;
        }

        switch (newState)
        {
            case PlayerPossibleState.JUMPING:
                // must be ground
                if (currentState != PlayerPossibleState.GROUND)
                {
                    return false;
                }
                break;

            case PlayerPossibleState.FALLING:
                // must be ground
                if (currentState != PlayerPossibleState.GROUND)
                {
                    return false;
                }
                break;
        }
        switch (currentState)
        {
            case PlayerPossibleState.DASHING:
                if (source != PlayerStateSource.DASH_STATE)
                {
                    return false;
                }
                break;

            case PlayerPossibleState.JUMPING:
                if (source != PlayerStateSource.JUMP_STATE)
                {
                    return false;
                }
                break;
        }
        return true;
    }

    public bool changeState(PlayerPossibleState newState, PlayerStateSource source)
    {
        bool isAllowed = isAllowedToChangeStateTo(newState, source);
        if (!isAllowed)
        {
            return false;
        }

        switch (newState)
        {
            case PlayerPossibleState.JUMPING:
                playerJumpHandler.stateStart();
                break;

            case PlayerPossibleState.DASHING:

                playerMovementHandler.handlePlayerXDirection(0);
                playerDashState.stateStart(playerMovementHandler.direction);
                break;
        }

        switch (currentState)
        {
            case PlayerPossibleState.JUMPING:
                playerJumpHandler.stateEnd();
                break;
            case PlayerPossibleState.DASHING:
                playerDashState.stateEnd();
                break;
        }

        currentState = newState;

        PlayerPossibleState[] cooldownStates = new PlayerPossibleState[]
        {
            PlayerPossibleState.DASHING,
            PlayerPossibleState.JUMPING
        };
        if (cooldownStates.Contains(newState))
        {
            StartCoroutine(handleIsStateChangeOnCooldown());
        }
        return true;
    }

    public IEnumerator handleIsStateChangeOnCooldown()
    {
        isStateChangeOnCooldown = true;
        yield return new WaitForSeconds(0.1f);
        isStateChangeOnCooldown = false;
    }

    private void clampVelocity()
    {
        float clampedVelocity = Mathf.Clamp(
            playerRigidbody.velocity.y,
            -maxYVelocity,
            maxYVelocity
        );
        playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, clampedVelocity);
    }
}
