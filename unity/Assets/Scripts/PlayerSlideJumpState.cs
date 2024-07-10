using UnityEngine;

public class PlayerSlideJumpState : MonoBehaviour
{
    private Rigidbody2D playerRigidbody;
    private PlayerState playerState;

    private bool isStateActive;
    private int tickCounter = 0;
    private bool isSlidingRight;

    public int maxTickCounter;
    public Vector2 jumpForceTick;

    public Vector2 jumpInitialVelocity;

    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerState = GetComponent<PlayerState>();
    }

    private void FixedUpdate()
    {
        handleTick();
    }

    private void handleTick()
    {
        if (!isStateActive)
        {
            return;
        }

        // can be between 0 - 1
        float progressScaled = tickCounter / (float)maxTickCounter;
        // float curvedPercentageThrust = Mathf.Pow(progressScaled, 2);

        Vector2 thrust = jumpForceTick - (jumpForceTick * progressScaled);

        Vector2 force = isSlidingRight ? new Vector2(-thrust.x, thrust.y) : thrust;

        playerRigidbody.AddForce(force);

        tickCounter = tickCounter + 1;

        if (tickCounter >= maxTickCounter)
        {
            handleJumpEnd();
        }
    }

    public void stateEnd()
    {
        isStateActive = false;
        // playerRigidbody.velocity = Vector2.zero;
    }

    public void handleJumpEnd()
    {
        playerState.changeState(PlayerPossibleState.NONE);
    }

    public void stateStart(bool _isSlidingRight)
    {
        isStateActive = true;
        tickCounter = 0;
        isSlidingRight = _isSlidingRight;

        playerRigidbody.velocity = (
            isSlidingRight
                ? new Vector2(-jumpInitialVelocity.x, jumpInitialVelocity.y)
                : jumpInitialVelocity
        );
    }
}
