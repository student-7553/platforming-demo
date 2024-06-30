using UnityEngine;

public class PlayerJumpState : MonoBehaviour
{
    private Rigidbody2D playerRigidbody;
    private PlayerGroundObserver playerGroundObserver;
    private PlayerState playerState;

    private bool isStateActive;

    // private bool isThrustingUpward;

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

    private void FixedUpdate()
    {
        handleJumpTick();
    }

    private void handleJumpTick()
    {
        if (!isStateActive)
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

        jumpingFixedCounter = jumpingFixedCounter + 1;

        if (jumpingFixedCounter >= maxJumpingFixedCounter)
        {
            handleJumpEnd();
        }
    }

    public void stateEnd()
    {
        // handleJumpEnd();
        // isStateActive = false;
    }

    public void handleJumpEnd()
    {
        isStateActive = false;
        jumpingFixedCounter = 0;
        playerState.changeState(PlayerPossibleState.GROUND, PlayerStateSource.JUMP_STATE);
    }

    public void stateStart()
    {
        isStateActive = true;
    }
}
