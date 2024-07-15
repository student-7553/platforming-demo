using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerState playerstate;

    void Start()
    {
        if (!!Singelton.GetPlayer())
        {
            Destroy(this);
            return;
        }
        Singelton.SetPlayer(this);
    }

    public void handleRestart()
    {
        Debug.Log("Handle restart.......");
    }
}
