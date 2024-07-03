using UnityEngine;
using System.Collections;
using System.Linq;

public enum PlayerPossibleState
{
    NONE,
    GROUND,
    FALLING,
    DASHING,
    JUMPING,
    SLIDING,
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

    private PlayerObserver playerObserver;

    public PlayerMovementHandler playerMovementHandler;

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
        playerObserver = GetComponent<PlayerObserver>();
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
        changeState(PlayerPossibleState.JUMPING, PlayerStateSource.INPUT, false);
    }

    public void handleDashAction()
    {
        changeState(PlayerPossibleState.DASHING, PlayerStateSource.INPUT, true);
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

    private bool isAllowedToChangeStateTo(
        PlayerPossibleState newState,
        PlayerStateSource source,
        bool isHighPriority
    )
    {
        if (newState == currentState)
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
        }
        if (isHighPriority)
        {
            return true;
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

    public bool changeState(
        PlayerPossibleState newState,
        PlayerStateSource source,
        bool isHighPriority
    )
    {
        bool isAllowed = isAllowedToChangeStateTo(newState, source, isHighPriority);

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
                playerDashState.stateStart(playerMovementHandler.direction);
                playerMovementHandler.handleDisableMovement();
                break;
        }

        switch (currentState)
        {
            case PlayerPossibleState.JUMPING:
                playerJumpHandler.stateEnd();
                break;
            case PlayerPossibleState.DASHING:
                playerDashState.stateEnd();
                playerMovementHandler.handleUnDisableMovement();
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
        //This function should prob be in jump handler tbh
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
