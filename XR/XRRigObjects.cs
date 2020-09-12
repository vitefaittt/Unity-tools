using UnityEngine;

public class XRRigObjects : MonoBehaviour
{
    [SerializeField]
    Transform head;
    public static Transform Head => Instance.head;
    [SerializeField]
    Transform leftController;
    public static Transform LeftController => Instance.leftController;
    [SerializeField]
    Transform rightController;
    public static Transform RightController => Instance.rightController;

    public static XRRigObjects Instance { get; private set; }


    private void Awake()
    {
        Instance = this;
    }
}
