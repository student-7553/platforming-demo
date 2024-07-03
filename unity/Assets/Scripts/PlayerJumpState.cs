using UnityEngine;

public class PlayerJumpState : MonoBehaviour
{
    private Rigidbody2D playerRigidbody;

    private PlayerState playerState;

    private bool isStateActive;

    private int jumpingFixedCounter = 0;
    public int maxJumpingFixedCounter;
    public int toThePowerOf;

    public int singleTickThrust;

    public int jumpInitialVelocity;

    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
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
        isStateActive = false;

        playerRigidbody.velocity = Vector2.zero;
        jumpingFixedCounter = 0;
    }

    public void handleJumpEnd()
    {
        //Todo handle this qq
        playerState.changeState(PlayerPossibleState.NONE, PlayerStateSource.JUMP_STATE, false);
    }

    public void stateStart()
    {
        isStateActive = true;
    }
}
