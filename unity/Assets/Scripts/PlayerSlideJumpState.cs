using UnityEngine;

public class PlayerSlideJumpState : MonoBehaviour
{
    private Rigidbody2D playerRigidbody;
    private PlayerState playerState;

    private bool isStateActive;
    private int tickCounter = 0;
    private bool isSlidingRight;

    // Set from editor
    public int maxTickCounter;
    public Vector2 jumpDirection;
    public float singleTickThrust;

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

        if (tickCounter == 0)
        {
            playerRigidbody.velocity = (
                isSlidingRight
                    ? new Vector2(-jumpInitialVelocity.x, jumpInitialVelocity.y)
                    : jumpInitialVelocity
            );
        }

        // can be between 0 - 1
        float progressScaled = tickCounter / (float)maxTickCounter;

        float curvedPercentageThrust = Mathf.Pow(progressScaled, 2);

        float thrust = singleTickThrust - (singleTickThrust * curvedPercentageThrust);

        Vector2 force =
            (isSlidingRight ? new Vector2(-jumpDirection.x, jumpDirection.y) : jumpDirection)
            * thrust;

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
        tickCounter = 0;

        // playerRigidbody.velocity = Vector2.zero;
    }

    public void handleJumpEnd()
    {
        playerState.changeState(PlayerPossibleState.NONE);
    }

    public void stateStart(bool _isSlidingRight)
    {
        isStateActive = true;
        isSlidingRight = _isSlidingRight;
    }
}
