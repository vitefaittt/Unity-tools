using System;
using UnityEngine;
using UnityEngine.XR;

public class Teleportation : MonoBehaviour
{
    // Player.
    [SerializeField]
    Transform playerRig;
    [SerializeField]
    Transform playerHead;
    [SerializeField]
    Transform leftController, rightController;

    // Teleporting.
    bool leftIsTeleporting, rightIsTeleporting;

    [Header("Appearance")]
    [SerializeField]
    TeleportationCurve curveTemplate;
    TeleportationCurve leftCurve, rightCurve;
    [SerializeField]
    GameObject sphereTemplate;
    Transform leftSphere, rightSphere;

    public bool couldTeleport = true;
    public static bool canTeleport = true;
    public event Action<Vector3> Teleported;

    public static Teleportation Instance { get; private set; }


    void Reset()
    {
        // Get the local player.
        playerRig = transform;
        playerHead = transform.GetComponentInChildren<Camera>()?.transform;
        leftController = transform.FindRecursive("left", "hand");
        rightController = transform.FindRecursive("right", "hand");
    }

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Create teleportation curves.
        leftCurve = Instantiate(curveTemplate, leftController);
        leftCurve.HitCondition = (hit) => { return hit.transform.CompareTag("sol"); };
        rightCurve = Instantiate(curveTemplate, rightController);
        rightCurve.HitCondition = (hit) => { return hit.transform.CompareTag("sol"); };
        leftCurve.transform.localPosition = rightCurve.transform.localPosition = Vector3.zero;
        leftCurve.transform.localRotation = rightCurve.transform.localRotation = Quaternion.identity;

        // Create teleportation spheres.
        leftSphere = Instantiate(sphereTemplate, Vector3.zero, Quaternion.identity, transform).transform;
        rightSphere = Instantiate(sphereTemplate, Vector3.zero, Quaternion.identity, transform).transform;

        StopTeleportationProcess();
    }


    void Update()
    {
        // Check if we can teleport.
        if(!canTeleport && couldTeleport)
        {
            StopTeleportationProcess();
            couldTeleport = false;
        }
        if (!canTeleport)
            return;
        couldTeleport = true;

        // Get thumbstick values.
        bool leftThumbstickDown = TryGetThumbstickDown(XRNode.LeftHand);
        bool rightThumbstickDown = TryGetThumbstickDown(XRNode.RightHand);

        // Start teleportation.
        if (!leftIsTeleporting && !rightIsTeleporting)
        {
            if (leftThumbstickDown)
            {
                leftIsTeleporting = true;

                rightCurve.ToggleDraw(false);
                rightSphere.gameObject.SetActive(false);

                leftCurve.ToggleDraw(true);
            }
            else if (rightThumbstickDown)
            {
                rightIsTeleporting = true;

                rightCurve.ToggleDraw(true);

                leftCurve.ToggleDraw(false);
                leftSphere.gameObject.SetActive(false);
            }
            return;
        }

        // Stop teleportation.
        if (leftIsTeleporting)
        {
            // Show the end point.
            leftSphere.gameObject.SetActive(leftCurve.EndPoint != null);
            if (leftCurve.EndPoint != null)
                leftSphere.position = (Vector3)leftCurve.EndPoint;

            if (!leftThumbstickDown)
            {
                // Teleport.
                if (leftCurve.EndPoint != null)
                    TeleportTo(leftSphere.position);
                StopTeleportationProcess();
            }
        }
        else if (rightIsTeleporting)
        {
            // Show the end point.
            rightSphere.gameObject.SetActive(rightCurve.EndPoint != null);
            if (rightCurve.EndPoint != null)
                rightSphere.transform.position = (Vector3)rightCurve.EndPoint;

            if (!rightThumbstickDown)
            {
                // Teleport.
                if (rightCurve.EndPoint != null)
                    TeleportTo(rightSphere.position);
                StopTeleportationProcess();
            }
        }
    }

    void OnDisable()
    {
        StopTeleportationProcess();
    }


    public void TeleportTo(Vector3 position)
    {
        if (playerHead)
        {
            // Move using the offset of the player inside the rig.
            Vector3 playerOffsetInRig = playerRig.position.SetY(0) - playerHead.position.SetY(0);
            playerRig.position = position + playerOffsetInRig;
        }
        else
            playerRig.transform.position = position;
        Teleported?.Invoke(position);
    }

    void StopTeleportationProcess()
    {
        leftIsTeleporting = false;
        rightIsTeleporting = false;
        leftCurve.ToggleDraw(false);
        leftSphere.gameObject.SetActive(false);
        rightCurve.ToggleDraw(false);
        rightSphere.gameObject.SetActive(false);
    }

    bool TryGetThumbstickDown(XRNode hand)
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(hand);
        bool state = false;
        if (device.isValid)
            device.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out state);
        return state;
    }
}

