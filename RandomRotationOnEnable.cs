using UnityEngine;

public class RandomRotationOnEnable : MonoBehaviour
{
    [SerializeField]
    BoolVector3 axis = new BoolVector3(false, true, false);
    [SerializeField]
    float[] rotations = new float[] { 0, 90, 180 };
    int rotationIndex = -1;


    void OnEnable()
    {
        rotationIndex = Utilities.ExclusiveRandomRange(0, rotations.Length, rotationIndex);
        Vector3 newRotation = axis.ToVector3() * rotations[rotationIndex];
        transform.localEulerAngles = new Vector3(axis.x ? newRotation.x : transform.localEulerAngles.x, axis.y ? newRotation.y : transform.localEulerAngles.y, axis.z ? newRotation.z : transform.localEulerAngles.z);
    }
}
