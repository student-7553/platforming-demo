using UnityEngine;

public class PlayerGroundObserver : MonoBehaviour
{
    private BoxCollider2D playerCollider;
    private float bottomMargin;
    public LayerMask layerMask;

    bool isOnGround;
    int totalFramesTouchedGround;

    void Start()
    {
        playerCollider = GetComponent<BoxCollider2D>();
        bottomMargin = playerCollider.size.y / 2f;
    }

    private void FixedUpdate()
    {
        computeIsOnGround();
    }

    public bool getIsOnGround()
    {
        return isOnGround;
    }

    private void computeIsOnGround()
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

        // Debug.DrawRay(clonedPosition, Vector3.down * 0.2f, Color.red);

        isOnGround = !!rayCastResult.collider;
    }
}
