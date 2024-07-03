using UnityEngine;

public class PlayerSlideState : MonoBehaviour
{
    private Rigidbody2D playerRigidbody;
    private PlayerState playerState;
    private bool isStateActive;
    public float maxYSlidingVelocity;

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

        clampVelocity();
    }

    public void stateEnd()
    {
        isStateActive = false;
    }

    public void stateStart()
    {
        isStateActive = true;
    }

    private void clampVelocity()
    {
        float clampedVelocity = Mathf.Clamp(
            playerRigidbody.velocity.y,
            -maxYSlidingVelocity,
            maxYSlidingVelocity
        );
        playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, clampedVelocity);
    }
}
