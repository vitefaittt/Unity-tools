using UnityEngine;

public class VRLaserPointer : MonoBehaviour
{
    [SerializeField]
    LineRenderer laser;
    VRSelectable currentButton;

    public static VRLaserPointer Instance { get; private set; }


    void Reset()
    {
        this.RenameFromType();
        if (!GetComponent<LineRenderer>())
            gameObject.AddComponent<LineRenderer>();
        else
        {
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(GetComponent<LineRenderer>(), "Setup laser line renderer");
#endif
        }
        laser = GetComponent<LineRenderer>();
        laser.startWidth = laser.endWidth = .005f;
        laser.useWorldSpace = false;
        ColorUtility.TryParseHtmlString("#AEFFFF", out Color laserColor);
        laser.startColor = laser.endColor = laserColor;
    }

    private void Awake()
    {
        Instance = this;
        laser = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (!laser.enabled)
            return;

        // Set laser tip position from hit.
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit))
            SetLaserTipLocalPosition(transform.InverseTransformPoint(hit.point));
        else
            SetLaserTipLocalPosition(Vector3.forward * 5);

        // Update our current button.
        VRSelectable newButton = hit.collider == null ? null : hit.collider.GetComponent<VRSelectable>();
        if (newButton != currentButton)
            UpdateCurrentButton(newButton);
    }


    void UpdateCurrentButton(VRSelectable newButton)
    {
        if (currentButton)
            currentButton.OnHoverEnd();
        currentButton = newButton;
        if (currentButton)
            currentButton.OnHoverStart();
    }

    public void TurnOn()
    {
        laser.enabled = true;
    }

    public void TurnOff()
    {
        laser.enabled = false;
    }

    public void OnClickDown()
    {
        currentButton?.OnClickDown();
    }

    public void OnClickUp()
    {
        currentButton?.OnClickUp();
    }

    void SetLaserTipLocalPosition(Vector3 localPosition)
    {
        laser.SetPosition(1, localPosition);
    }
}

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(VRLaserPointer)), UnityEditor.CanEditMultipleObjects]
class VRLaserPointerEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GUILayout.Space(5);

        VRLaserPointer script = (VRLaserPointer)target;

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Turn on"))
            script.TurnOn();
        if (GUILayout.Button("Turn off"))
            script.TurnOff();
        GUILayout.EndHorizontal();
    }
}
#endif