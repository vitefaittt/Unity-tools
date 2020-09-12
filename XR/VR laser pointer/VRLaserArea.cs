using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class VRLaserArea : MonoBehaviour
{
    [SerializeField]
    VRLaserPointer laser;
    [SerializeField]
    Vector3 cornerA = new Vector3(-1, 1, 0);
    [SerializeField]
    Vector3 cornerB = new Vector3(1, -1, 0);
    static List<VRLaserArea> targetedAreas = new List<VRLaserArea>();
    bool LaserIsLooking {
        get {
            Vector3 laserToA = transform.TransformPoint(cornerA) - laser.transform.position;
            Vector3 laserToB = transform.TransformPoint(cornerB) - laser.transform.position;
            Vector3 laserForward = laser.transform.forward;
            float tosAngle = Vector3.Angle(laserToA, laserToB);
            return Vector3.Angle(laserForward, laserToB) < tosAngle && Vector3.Angle(laserForward, laserToA) < tosAngle;
        }
    }
    ValueHolder<bool> laserIsLookingHolder = new ValueHolder<bool>();


    void Reset()
    {
        laser = FindObjectOfType<VRLaserPointer>();
        if (GetComponent<RectTransform>())
        {
            Bounds bounds = GetComponent<RectTransform>().GetBounds();
            cornerA = new Vector2(bounds.min.x, bounds.max.y);
            cornerB = new Vector2(bounds.max.x, bounds.min.y);
        }
    }

    void Update()
    {
        // Add ourselves to the targeted areas when the laser looks at us.
        laserIsLookingHolder.Value = LaserIsLooking;
        if (laserIsLookingHolder.ValueChanged)
        {
            if (laserIsLookingHolder.Value)
                targetedAreas.Add(this);
            else
                targetedAreas.Remove(this);
        }

        // Turn on the laser if it is looking at areas.
        if (targetedAreas.Count > 0)
            laser.TurnOn();
        else
            laser.TurnOff();
    }

    void OnDisable()
    {
        // Remove ourselves from the targeted areas when we close.
        if (!laserIsLookingHolder.Value)
            return;
        targetedAreas.Remove(this);
        laserIsLookingHolder.Value = false;
        if (targetedAreas.Count < 1)
            laser.TurnOff();
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.LowerCenter;
        Gizmos.DrawSphere(transform.TransformPoint(cornerA), .006f);
        Handles.Label(transform.TransformPoint(cornerA), "pA (Laser Area)", style);
        Gizmos.DrawSphere(transform.TransformPoint(cornerB), .006f);
        Handles.Label(transform.TransformPoint(cornerB), "pB (Laser Area)", style);
    }
#endif
}

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(VRLaserArea)), UnityEditor.CanEditMultipleObjects]
class VRLaserAreaEditor : UnityEditor.Editor
{


    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        VRLaserArea script = (VRLaserArea)target;
        GUILayout.Space(5);

        if (GUILayout.Button("Setup child buttons"))
            script.transform.ForAllDescendants(delegate (Transform child)
            {
                child.GetComponent<UnityEngine.UI.Button>()?.GetOrAddComponent<VRSelectable>();
            });
    }
}
#endif