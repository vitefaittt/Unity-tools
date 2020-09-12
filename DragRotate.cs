using UnityEngine;

public class DragRotate : MonoBehaviour
{
    [SerializeField]
    Camera cam;
    public bool useSpeedCurve;
    [SerializeField]
    float speed = .7f;
    [SerializeField]
    AnimationCurve speedCurve = new AnimationCurve(new Keyframe(1,.2f), new  Keyframe(10,.7f));
    float CurrentSpeed { get { return useSpeedCurve ? speedCurve.Evaluate(Vector3.Distance(transform.position, cam.transform.position)) : speed; } }

    Vector2 lastMousePos;
    bool isRotating;


    private void Reset()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        // Update the rotation.
        if (isRotating)
        {
            if (Input.GetMouseButtonUp(0))
            {
                OnRotationEnd();
                return;
            }
            UpdateRotation();
        }

        // Detect a click on the planete.
        if (!Input.GetMouseButtonDown(0))
            return;
        if (Utilities.ScreenRaycastAllToObject(gameObject, cam))
            OnRotationStart();
    }


    void OnRotationStart()
    {
        isRotating = true;
        lastMousePos = Input.mousePosition;
    }

    void UpdateRotation()
    {
        Vector2 mouseOffset = (Vector2)Input.mousePosition - lastMousePos;
        Vector2 rotation = new Vector2(-mouseOffset.y, -mouseOffset.x) * CurrentSpeed;
        transform.Rotate(rotation, Space.World);
        lastMousePos = Input.mousePosition;
    }

    void OnRotationEnd()
    {
        isRotating = false;
    }
}

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(DragRotate)), UnityEditor.CanEditMultipleObjects]
public class DragRotateEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        DragRotate script = target as DragRotate;
        System.Collections.Generic.List<string> excludedProperties = new System.Collections.Generic.List<string>();

        if (!script.useSpeedCurve)
            excludedProperties.Add("speedCurve");
        else
            excludedProperties.Add("speed");

        DrawPropertiesExcluding(serializedObject, excludedProperties.ToArray());
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
