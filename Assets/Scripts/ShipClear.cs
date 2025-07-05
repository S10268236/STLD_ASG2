using Unity.VisualScripting;
using UnityEngine;

public class ShipClear : MonoBehaviour
{
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Rock"))
        {
            
        }
    }
}
