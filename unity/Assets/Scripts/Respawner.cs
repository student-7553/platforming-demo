using UnityEngine;

public class Respawner : MonoBehaviour
{
    void OnTriggerExit2D(Collider2D col)
    {
        Singelton.GetPlayer().handleRestart();
    }
}
