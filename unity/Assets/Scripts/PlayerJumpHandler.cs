using UnityEngine;

public class PlayerJumpHandler : MonoBehaviour
{
    private Rigidbody2D playerRigidbody;
    private PlayerGroundObserver playerGroundObserver;
    private PlayerState playerState;

    private bool isStateActive;

    private bool isThrustingUpward;

    private int jumpingFixedCounter = 0;
    public int maxJumpingFixedCounter;
    public int toThePowerOf;

    public int singleTickThrust;

    public int jumpInitialVelocity;

    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerGroundObserver = GetComponent<PlayerGroundObserver>();
        playerState = GetComponent<PlayerState>();
    }

    private void updateJumpCounter(int newCounter)
    {
        jumpingFixedCounter = newCounter;

        if (jumpingFixedCounter >= maxJumpingFixedCounter)
        {
            handleJumpEnd();
        }
    }

    private void FixedUpdate()
    {
        handleJumpTick();

        handleFixedUpdateStateTransition();
    }

    private void handleJumpTick()
    {
        if (!isThrustingUpward)
        {
            return;
        }

        if (jumpingFixedCounter == 0)
        {
            playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, jumpInitialVelocity);
        }

        float scaled = jumpingFixedCounter / (float)maxJumpingFixedCounter;
        float curvedPercentageThrust = Mathf.Pow(scaled, toThePowerOf);

        float thrust = singleTickThrust - (singleTickThrust * curvedPercentageThrust);

        Vector2 force = Vector2.up * thrust;

        playerRigidbody.AddForce(force);

        updateJumpCounter(jumpingFixedCounter + 1);
    }

    private void handleFixedUpdateStateTransition()
    {
        if (!isStateActive || playerState.isStateChangeOnCooldown)
        {
            return;
        }

        bool isGrounded = playerGroundObserver.isOnGround();
        if (isGrounded)
        {
            isStateActive = false;
            handleJumpEnd();
            playerState.changeState(PlayerPossibleState.GROUND);
            return;
        }
    }

    public void handleJumpEnd()
    {
        isThrustingUpward = false;
        updateJumpCounter(0);
    }

    public void handlePlayerJumpStateStart()
    {
        isStateActive = true;
        isThrustingUpward = true;
    }
}
