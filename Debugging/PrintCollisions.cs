using UnityEngine;

public class PrintCollisions : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        print(collision.collider);
    }
}
