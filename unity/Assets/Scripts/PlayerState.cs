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
    WAVE_DASH_JUMP,
    SUPER_DASH_JUMP,
    HYPER_DASH_JUMP,
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

    public PlayerObserver playerObserver;

    public PlayerMovementHandler playerMovementHandler;

    // ------------------STATES-------------------------------
    private PlayerJumpState playerJumpHandler;
    private PlayerDashState playerDashState;
    private PlayerSlideState playerSlideState;
    private PlayerSlideJumpState playerSlideJumpState;
    private PlayerDashAfterState playerDashAfterState;
    private PlayerWaveJumpState playerWaveJumpState;
    private PlayerSuperJumpState playerSuperJumpState;
    private PlayerHyperJumpState playerHyperJumpState;

    // -------------------------------------------------

    public bool isStateChangeOnCooldown;

    void Start()
    {
        playerObserver = GetComponent<PlayerObserver>();
        playerMovementHandler = GetComponent<PlayerMovementHandler>();
        playerJumpHandler = GetComponent<PlayerJumpState>();
        playerDashState = GetComponent<PlayerDashState>();
        playerSlideState = GetComponent<PlayerSlideState>();
        playerSlideJumpState = GetComponent<PlayerSlideJumpState>();
        playerDashAfterState = GetComponent<PlayerDashAfterState>();
        playerWaveJumpState = GetComponent<PlayerWaveJumpState>();
        playerSuperJumpState = GetComponent<PlayerSuperJumpState>();
        playerHyperJumpState = GetComponent<PlayerHyperJumpState>();

        currentState = PlayerPossibleState.GROUND;
    }

    public void handleJumpAction()
    {
        switch (currentState)
        {
            case PlayerPossibleState.GROUND:
                changeState(PlayerPossibleState.JUMPING);
                break;
            case PlayerPossibleState.SLIDING:
                changeState(PlayerPossibleState.SLIDE_JUMPING);
                break;
            case PlayerPossibleState.FALLING:
                if (
                    playerObserver.observedState != ObservedState.NEAR_LEFT_WALL
                    && playerObserver.observedState != ObservedState.NEAR_RIGHT_WALL
                )
                {
                    return;
                }
                changeState(PlayerPossibleState.SLIDE_JUMPING);
                break;
            case PlayerPossibleState.DASHING:
                playerDashState.handleJumpActivation();
                break;
            case PlayerPossibleState.DASHING_AFTER:
                playerDashAfterState.handleJumpActivation();
                break;
        }
    }

    public void handleDashAction()
    {
        if (!playerObserver.isDashAvailable)
        {
            return;
        }

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

        switch (currentState)
        {
            case PlayerPossibleState.JUMPING:
                playerJumpHandler.stateEnd();
                break;
            case PlayerPossibleState.DASHING:
                playerMovementHandler.handleUnDisableMovement();
                playerDashState.stateEnd();
                break;
            case PlayerPossibleState.WAVE_DASH_JUMP:
                playerMovementHandler.handleUnDisableMovement();
                playerWaveJumpState.stateEnd();
                break;
            case PlayerPossibleState.SUPER_DASH_JUMP:
                playerMovementHandler.handleUnDisableMovement();
                playerSuperJumpState.stateEnd();
                break;
            case PlayerPossibleState.HYPER_DASH_JUMP:
                playerMovementHandler.handleUnDisableMovement();
                playerHyperJumpState.stateEnd();
                break;
            case PlayerPossibleState.SLIDING:
                playerSlideState.stateEnd();
                break;
            case PlayerPossibleState.SLIDE_JUMPING:
                playerMovementHandler.handleUnDisableMovement();
                playerSlideJumpState.stateEnd();
                break;
            case PlayerPossibleState.DASHING_AFTER:
                playerMovementHandler.handleUnDisableMovement();
                playerDashAfterState.stateEnd();
                break;
        }

        switch (newState)
        {
            case PlayerPossibleState.JUMPING:
                playerJumpHandler.stateStart(playerMovementHandler.direction);
                break;
            case PlayerPossibleState.SLIDE_JUMPING:
                playerMovementHandler.handleDisableMovement();
                playerSlideJumpState.stateStart(
                    playerObserver.observedState == ObservedState.NEAR_RIGHT_WALL ? true : false
                );
                break;
            case PlayerPossibleState.DASHING:
                playerMovementHandler.handleDisableMovement();
                playerObserver.dashMark();

                Vector2 dashDirection = playerMovementHandler.direction;
                if (dashDirection == Vector2.zero)
                {
                    dashDirection =
                        playerMovementHandler.lookingDirection == LookingDirection.LEFT
                            ? Vector2.left
                            : Vector2.right;
                }

                playerDashState.stateStart(dashDirection);
                break;
            case PlayerPossibleState.WAVE_DASH_JUMP:
                playerMovementHandler.handleDisableMovement();
                playerWaveJumpState.stateStart(
                    playerMovementHandler.direction,
                    playerDashState.cachedVelocity
                );
                break;
            case PlayerPossibleState.DASHING_AFTER:
                playerDashAfterState.stateStart(playerDashState.currentDashType);
                break;
            case PlayerPossibleState.SLIDING:
                playerSlideState.stateStart();
                break;
            case PlayerPossibleState.SUPER_DASH_JUMP:
                playerMovementHandler.handleDisableMovement();
                playerSuperJumpState.stateStart(
                    playerMovementHandler.direction,
                    playerDashState.cachedVelocity
                );
                break;
            case PlayerPossibleState.HYPER_DASH_JUMP:
                playerMovementHandler.handleDisableMovement();
                playerHyperJumpState.stateStart(
                    playerMovementHandler.direction,
                    playerDashState.cachedVelocity
                );
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
}
