using UnityEngine;

public class SimpleVRButtonPointer : MonoBehaviour
{
    void Reset()
    {
        this.RenameFromType();
        this.GetOrAddComponent<Rigidbody>().isKinematic = true;
    }
}
