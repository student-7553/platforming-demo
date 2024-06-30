using UnityEngine;

public class PlayerGroundObserver : MonoBehaviour
{
    private BoxCollider2D playerCollider;
    private float bottomMargin;
    private PlayerState playerState;
    public LayerMask layerMask;

    private bool isOnGround;

    // int totalFramesTouchedGround;

    void Start()
    {
        playerCollider = GetComponent<BoxCollider2D>();
        bottomMargin = playerCollider.size.y / 2f;

        playerState = GetComponent<PlayerState>();
    }

    private void FixedUpdate()
    {
        isOnGround = computeIsOnGround();
        if (!isOnGround)
        {
            playerState.changeState(PlayerPossibleState.FALLING, PlayerStateSource.GROUND_OBSERVER);
        }
        else
        {
            playerState.changeState(PlayerPossibleState.GROUND, PlayerStateSource.GROUND_OBSERVER);
        }
    }

    public bool getIsOnGround()
    {
        return isOnGround;
    }

    private bool computeIsOnGround()
    {
        Vector2 originPosition = new Vector2(
            gameObject.transform.position.x,
            gameObject.transform.position.y - bottomMargin
        );

        RaycastHit2D rayCastResult = Physics2D.Raycast(
            originPosition,
            Vector2.down,
            0.25f,
            layerMask
        );

        Vector3 clonedPosition = gameObject.transform.position;
        clonedPosition.y = clonedPosition.y - bottomMargin;

        return !!rayCastResult.collider;
    }
}
