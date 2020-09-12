using UnityEngine;

[ExecuteInEditMode]
public class LookAtConstrained : MonoBehaviour
{
    [SerializeField]
    Transform target;


    private void Update()
    {
        if (!target)
            return;
        transform.rotation = Quaternion.LookRotation(Vector3.up, target.position);
    }
}