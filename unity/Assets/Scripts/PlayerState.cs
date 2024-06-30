using UnityEngine;
using System.Collections;

public enum PlayerPossibleState
{
    GROUND,
    DASHING,
    JUMPING,
}

// public interface PlayerStateCommon
// {
//     public void stateStart();
//     public void stateEnd();
// }

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
        changeState(PlayerPossibleState.JUMPING);
    }

    public void handleJumpActionEnd()
    {
        playerJumpHandler.handleJumpEnd();
    }

    public void handleDashAction()
    {
        changeState(PlayerPossibleState.DASHING);
    }

    public void handleXMovement(float direction)
    {
        if (currentState == PlayerPossibleState.DASHING)
        {
            Debug.Log("rejected...");
            return;
        }
        playerMovementHandler.handlePlayerXDirection(direction);
    }

    private void FixedUpdate()
    {
        clampVelocity();
    }

    public bool changeState(PlayerPossibleState newState)
    {
        if (isStateChangeOnCooldown || newState == currentState)
        {
            return false;
        }

        Debug.Log(newState);

        switch (newState)
        {
            case PlayerPossibleState.JUMPING:
                // must be ground
                if (currentState != PlayerPossibleState.GROUND)
                {
                    // not possible
                    return false;
                }
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
        StartCoroutine(handleIsStateChangeOnCooldown());
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
