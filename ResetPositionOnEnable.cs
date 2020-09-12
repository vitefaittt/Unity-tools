using UnityEngine;

public class ResetPositionOnEnable : MonoBehaviour
{
    Vector3? startPosition;


    void OnEnable()
    {
        if (startPosition == null)
            startPosition = transform.position;
        transform.position = (Vector3)startPosition;
    }
}
