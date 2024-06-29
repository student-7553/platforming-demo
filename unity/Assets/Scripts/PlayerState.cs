using UnityEngine;
using System.Collections;

public enum PlayerPossibleState
{
    GROUND,
    DASHING,
    JUMPING,
}

public class PlayerState : MonoBehaviour
{
    [SerializeField]
    private PlayerPossibleState possibleState;

    private Rigidbody2D playerRigidbody;

    private PlayerGroundObserver playerGroundObserver;

    private PlayerMovementHandler playerMovementHandler;

    private PlayerJumpHandler playerJumpHandler;

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
        playerJumpHandler = GetComponent<PlayerJumpHandler>();

        possibleState = PlayerPossibleState.GROUND;
    }

    private void OnDestroy()
    {
        Singelton.SetPlayerState(null);
    }

    public void handleJumpAction()
    {
        if (possibleState != PlayerPossibleState.GROUND)
        {
            return;
        }

        changeState(PlayerPossibleState.JUMPING);
    }

    public void handleJumpActionEnd()
    {
        playerJumpHandler.handleJumpEnd();
    }

    public void handleXMovement(float direction)
    {
        playerMovementHandler.handlePlayerXDirection(direction);
    }

    private void FixedUpdate()
    {
        clampVelocity();
    }

    public void changeState(PlayerPossibleState newState)
    {
        if (isStateChangeOnCooldown)
        {
            return;
        }
        Debug.Log(newState);

        possibleState = newState;

        switch (newState)
        {
            case PlayerPossibleState.JUMPING:
                playerJumpHandler.handlePlayerJumpStateStart();
                break;
        }
        StartCoroutine(handleIsStateChangeOnCooldown());
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
