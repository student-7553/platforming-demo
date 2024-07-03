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
    SLIDE_JUMPING
}

public enum PlayerStateSource
{
    GROUND_OBSERVER,
    DASH_STATE,
    JUMP_STATE,
    INPUT,
}

// interface PlayerStateFunction
// {
//     public void stateStart();
//     public void stateEnd();
// }

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

    //This needs to be handled from the states themself
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
                playerSlideJumpState.stateStart(
                    playerMovementHandler.direction.x > 0 ? true : false
                );
                break;
            case PlayerPossibleState.DASHING:
                playerDashState.stateStart(playerMovementHandler.direction);
                playerMovementHandler.handleDisableMovement();
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
                playerMovementHandler.handleUnDisableMovement();
                break;
            case PlayerPossibleState.SLIDING:
                playerSlideState.stateEnd();
                break;
            case PlayerPossibleState.SLIDE_JUMPING:
                playerSlideJumpState.stateEnd();
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
