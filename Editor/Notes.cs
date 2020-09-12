using UnityEngine;

public class Notes : MonoBehaviour
{
    [SerializeField]
    [TextArea]
    string note;


    void Reset()
    {
        gameObject.name = "[Notes]";
        transform.position = transform.localEulerAngles = new Vector3();
    }
}
