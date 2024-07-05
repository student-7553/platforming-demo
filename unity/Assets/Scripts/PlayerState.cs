using UnityEngine;
using System.Collections;
using System.Linq;

public enum PlayerPossibleState
{
    NONE,
    GROUND,
    FALLING,
    DASHING,
    DASHING_AFTER,
    JUMPING,
    SLIDING,
    SLIDE_JUMPING
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
    public PlayerPossibleState currentState;

    private Rigidbody2D playerRigidbody;

    private PlayerObserver playerObserver;

    public PlayerMovementHandler playerMovementHandler;

    private PlayerJumpState playerJumpHandler;

    private PlayerDashState playerDashState;
    private PlayerSlideState playerSlideState;
    private PlayerSlideJumpState playerSlideJumpState;
    private PlayerDashAfterState playerDashAfterState;

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
        playerObserver = GetComponent<PlayerObserver>();
        playerMovementHandler = GetComponent<PlayerMovementHandler>();
        playerJumpHandler = GetComponent<PlayerJumpState>();
        playerDashState = GetComponent<PlayerDashState>();
        playerSlideState = GetComponent<PlayerSlideState>();
        playerSlideJumpState = GetComponent<PlayerSlideJumpState>();
        playerDashAfterState = GetComponent<PlayerDashAfterState>();

        currentState = PlayerPossibleState.GROUND;
    }

    private void OnDestroy()
    {
        Singelton.SetPlayerState(null);
    }

    public void handleJumpAction()
    {
        if (currentState == PlayerPossibleState.GROUND)
        {
            changeState(PlayerPossibleState.JUMPING);
            return;
        }
        if (currentState == PlayerPossibleState.SLIDING)
        {
            changeState(PlayerPossibleState.SLIDE_JUMPING);
        }
        if (currentState == PlayerPossibleState.DASHING_AFTER)
        {
            playerDashAfterState.handleJumpActivation();
        }
    }

    public void handleDashAction()
    {
        //ok
        changeState(PlayerPossibleState.DASHING);
    }

    public void handleJumpActionEnd()
    {
        if (currentState != PlayerPossibleState.JUMPING)
        {
            return;
        }
        playerJumpHandler.handleJumpEnd();
    }

    private void FixedUpdate()
    {
        clampVelocity();
    }

    private bool isAllowedToChangeStateTo(PlayerPossibleState newState)
    {
        if (newState == currentState)
        {
            return false;
        }

        return true;
    }

    public bool changeState(PlayerPossibleState newState)
    {
        bool isAllowed = isAllowedToChangeStateTo(newState);

        if (!isAllowed)
        {
            return false;
        }

        switch (newState)
        {
            case PlayerPossibleState.JUMPING:
                playerJumpHandler.stateStart();
                break;
            case PlayerPossibleState.SLIDE_JUMPING:
                playerMovementHandler.handleDisableMovement();
                playerSlideJumpState.stateStart(
                    playerMovementHandler.direction.x > 0 ? true : false
                );
                break;
            case PlayerPossibleState.DASHING:
                playerMovementHandler.handleDisableMovement();
                playerDashState.stateStart(playerMovementHandler.direction);
                break;
            case PlayerPossibleState.DASHING_AFTER:
                playerDashAfterState.stateStart(playerMovementHandler.direction);
                break;
            case PlayerPossibleState.SLIDING:
                playerSlideState.stateStart();
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
            case PlayerPossibleState.SLIDING:
                playerSlideState.stateEnd();
                break;
            case PlayerPossibleState.SLIDE_JUMPING:
                playerMovementHandler.handleDisableMovement();
                playerMovementHandler.handleUnDisableMovement();
                playerSlideJumpState.stateEnd();
                break;
            case PlayerPossibleState.DASHING_AFTER:
                playerMovementHandler.handleUnDisableMovement();
                playerDashAfterState.stateEnd();
                break;
        }

        currentState = newState;

        PlayerPossibleState[] cooldownStates = new PlayerPossibleState[]
        {
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
        // This function should prob be in jump handler tbh
        isStateChangeOnCooldown = true;
        yield return new WaitForSeconds(0.05f);
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
