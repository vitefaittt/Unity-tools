using UnityEngine;

public class AddForceOnKeyPress : MonoBehaviour
{
    public Vector3 force = Vector3.forward * 350;
    public KeyCode key = KeyCode.Space;


    void Update()
    {
        if (Input.GetKeyDown(key))
            if (GetComponent<Rigidbody>())
                GetComponent<Rigidbody>().AddForce(force);
    }
}
