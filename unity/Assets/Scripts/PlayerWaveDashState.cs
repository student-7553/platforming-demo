using UnityEngine;

public enum PlayerWaveDashDirection
{
    RIGHT,
    LEFT,
}

public class PlayerWaveDashState : MonoBehaviour
{
    private Rigidbody2D playerRigidbody;
    private PlayerState playerState;

    private bool isStateActive;
    private PlayerWaveDashDirection direction;

    private int currentTickCount;

    public int maxTickCounter;

    public Vector2 jumpInitialVelocity;
    public Vector2 jumpTickThrust;

    void Start()
    {
        playerState = GetComponent<PlayerState>();
        playerRigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (!isStateActive)
        {
            return;
        }
        if (currentTickCount == 0)
        {
            playerRigidbody.velocity =
                direction == PlayerWaveDashDirection.RIGHT
                    ? jumpInitialVelocity
                    : new Vector2(-jumpInitialVelocity.x, jumpInitialVelocity.y);
        }

        // can be between 0 - 1
        float progressScaled = currentTickCount / (float)maxTickCounter;
        float curvedPercentageThrust = Mathf.Pow(progressScaled, 2);

        Vector2 thrust = jumpTickThrust - (jumpTickThrust * curvedPercentageThrust);

        Vector2 force =
            direction == PlayerWaveDashDirection.RIGHT ? thrust : new Vector2(-thrust.x, thrust.y);

        playerRigidbody.AddForce(force);

        currentTickCount = currentTickCount + 1;

        if (currentTickCount >= maxTickCounter)
        {
            playerState.changeState(PlayerPossibleState.NONE);
        }
    }

    public void stateEnd()
    {
        isStateActive = false;
        currentTickCount = 0;
    }

    public void stateStart(Vector2 _direction)
    {
        isStateActive = true;
        direction = _direction.x > 0 ? PlayerWaveDashDirection.RIGHT : PlayerWaveDashDirection.LEFT;
    }
}
