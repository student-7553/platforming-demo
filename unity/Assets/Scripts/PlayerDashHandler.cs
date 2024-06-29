using UnityEngine;

public class PlayerDashHandler : MonoBehaviour
{
    Rigidbody2D playerRigidbody;

    void Start()
    {
        // if (!!Singelton.GetPlayerDashHandler())
        // {
        //     Destroy(this);
        //     return;
        // }
        // Singelton.SetPlayerDashHandler(this);
        playerRigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate() { }

    public void handleDash()
    {
        //
    }
}
