using UnityEngine;

public class PlayerVelocityHandler : MonoBehaviour
{
    private Rigidbody2D playerRigidbody;
    public int maxYVelocity;
    public int minYVelocity;

    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        clampVelocity();
    }

    private void clampVelocity()
    {
        float clampedYVelocity = Mathf.Clamp(
            playerRigidbody.velocity.y,
            minYVelocity,
            maxYVelocity
        );

        playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, clampedYVelocity);
    }
}
