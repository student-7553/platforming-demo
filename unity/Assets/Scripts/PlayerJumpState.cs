using UnityEngine;

public class PlayerJumpState : MonoBehaviour
{
    private Rigidbody2D playerRigidbody;

    private PlayerState playerState;

    private bool isStateActive;

    private int tickCounter = 0;
    public int maxJumpingFixedCounter;

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

        if (tickCounter == 0)
        {
            playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, jumpInitialVelocity);
        }

        // can be between 0 - 1
        float progressScaled = tickCounter / (float)maxJumpingFixedCounter;
        float curvedPercentageThrust = Mathf.Pow(progressScaled, 2);

        float thrust = singleTickThrust - (singleTickThrust * curvedPercentageThrust);

        Vector2 force = new Vector2(0, thrust);

        playerRigidbody.AddForce(force);

        tickCounter = tickCounter + 1;

        if (tickCounter >= maxJumpingFixedCounter)
        {
            handleJumpEnd();
        }
    }

    public void stateEnd()
    {
        isStateActive = false;

        playerRigidbody.velocity = Vector2.zero;
    }

    public void handleJumpEnd()
    {
        playerState.changeState(PlayerPossibleState.NONE);
    }

    public void stateStart()
    {
        isStateActive = true;
        tickCounter = 0;
    }
}
