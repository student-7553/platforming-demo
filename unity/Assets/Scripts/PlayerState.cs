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
    WAVE_DASHING,
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
    private PlayerWaveDashState playerWaveDashState;

    // -------------------------------------------------

    public bool isStateChangeOnCooldown;

    void Start()
    {
        if (!!Singelton.GetPlayerState())
        {
            Destroy(this);
            return;
        }
        Singelton.SetPlayerState(this);

        playerObserver = GetComponent<PlayerObserver>();
        playerMovementHandler = GetComponent<PlayerMovementHandler>();
        playerJumpHandler = GetComponent<PlayerJumpState>();
        playerDashState = GetComponent<PlayerDashState>();
        playerSlideState = GetComponent<PlayerSlideState>();
        playerSlideJumpState = GetComponent<PlayerSlideJumpState>();
        playerDashAfterState = GetComponent<PlayerDashAfterState>();
        playerWaveDashState = GetComponent<PlayerWaveDashState>();

        currentState = PlayerPossibleState.GROUND;
    }

    private void OnDestroy()
    {
        Singelton.SetPlayerState(null);
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
            case PlayerPossibleState.DASHING:
                playerDashState.handleJumpActivation();
                break;
            case PlayerPossibleState.DASHING_AFTER:
                playerMovementHandler.handleDisableMovement();
                playerDashAfterState.handleJumpActivation();
                break;
        }
    }

    public void handleDashAction()
    {
        // Todo cap how the dash works
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
            case PlayerPossibleState.WAVE_DASHING:
                playerWaveDashState.stateStart(playerMovementHandler.direction);
                break;
            case PlayerPossibleState.DASHING_AFTER:
                playerDashAfterState.stateStart(
                    playerMovementHandler.direction,
                    playerDashState.currentDashType
                );
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
                if (newState != PlayerPossibleState.WAVE_DASHING)
                {
                    playerMovementHandler.handleUnDisableMovement();
                }
                playerDashState.stateEnd();
                break;
            case PlayerPossibleState.WAVE_DASHING:
                playerMovementHandler.handleUnDisableMovement();
                playerWaveDashState.stateEnd();
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
