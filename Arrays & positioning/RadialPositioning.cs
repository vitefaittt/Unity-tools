using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RadialPositioning : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Minimum ItemsAmount of imaginary balls to spawn, determines the space between each spawned object.")]
    int minIncrements = 8;
    [SerializeField]
    float radius = 1;
    [SerializeField]
    bool rotateElements = true;
    [Header("Navigation")]
    [SerializeField]
    bool smoothTransition = true;
    [SerializeField]
    float transitionSpeed = 400;
    [SerializeField]
    bool reverseAtTheEnd = true;
    int currentIndex = 0;
    Quaternion startRotation;
    float IncrementAngle => 360f / Mathf.Max(ItemsAmount, minIncrements);
    int ItemsAmount => transform.childCount;


    private void Awake()
    {
        startRotation = transform.rotation;
    }

    private void Update()
    {
        UpdateItemsTransform();
    }


    public void UpdateItemsTransform()
    {
        // Update each child's position.
        for (int i = 0; i < ItemsAmount; i++)
        {
            transform.GetChild(i).localPosition = PointOnCircle(radius, i / (float)Mathf.Max(ItemsAmount, minIncrements) * 360);
            if (rotateElements)
                transform.GetChild(i).rotation = Quaternion.LookRotation((transform.GetChild(i).transform.position - transform.position).normalized, transform.forward);
        }
    }

    Vector2 PointOnCircle(float radius, float angle)
    {
        Vector2 pos;
        pos.x = radius * Mathf.Sin(angle * Mathf.Deg2Rad);
        pos.y = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
        return pos;
    }

    public void Next()
    {
        if (smoothTransition)
        {
            // Rotate the array over time.
            currentIndex++;
            if (currentIndex < ItemsAmount)
                StartCoroutine(Rotate(IncrementAngle, transitionSpeed));
            else
            {
                currentIndex = 0;
                StartCoroutine(Rotate(reverseAtTheEnd ? -IncrementAngle * (ItemsAmount - 1) : IncrementAngle * (ItemsAmount < minIncrements ? minIncrements - ItemsAmount + 1 : 1), transitionSpeed * 2));
            }
            return;
        }

        // Rotate the array instantly.
        Utilities.LimitedIncrement(ref currentIndex, ItemsAmount);
        if (currentIndex == 0)
            transform.rotation = startRotation;
        else
            transform.Rotate(Vector3.forward, IncrementAngle, Space.Self);
    }

    public void Previous()
    {
        if (smoothTransition)
        {
            // Rotate the array over time.
            currentIndex--;
            if (currentIndex >= 0)
                StartCoroutine(Rotate(-IncrementAngle, transitionSpeed));
            else
            {
                currentIndex = ItemsAmount - 1;
                StartCoroutine(Rotate(reverseAtTheEnd ? IncrementAngle * (ItemsAmount - 1) : -IncrementAngle * (ItemsAmount < minIncrements ? minIncrements - ItemsAmount + 1 : 1), transitionSpeed * 2));
            }
            return;
        }

        // Rotate the array instantly.
        Utilities.LimitedDecrement(ref currentIndex, ItemsAmount);
        if (currentIndex == ItemsAmount - 1)
            transform.Rotate(Vector3.forward, IncrementAngle * (ItemsAmount - 1), Space.Self);
        else
            transform.Rotate(Vector3.forward, -IncrementAngle, Space.Self);
    }

    IEnumerator Rotate(float angle, float speed)
    {
        float rotatedAmount = 0;
        while ((angle > 0 && rotatedAmount < angle) || (angle < 0 && rotatedAmount > angle))
        {
            float newRotAngle = (angle > 0 ? 1 : -1) * Time.deltaTime * speed;
            // Clamp the rotation angle to not rotate too much.
            if (angle > 0)
            {
                if (angle - newRotAngle < rotatedAmount)
                    newRotAngle = angle - rotatedAmount;
            }
            else if (angle - newRotAngle > rotatedAmount)
                newRotAngle = angle - rotatedAmount;
            // Rotate.
            transform.Rotate(Vector3.forward, newRotAngle, Space.Self);
            rotatedAmount += newRotAngle;
            yield return null;
        }
    }

    public void SetupEmptyParents()
    {
        for (int i = 0; i < ItemsAmount; i++)
        {
            Transform newParent = new GameObject(transform.GetChild(i).name + " parent").transform;
            newParent.parent = transform.GetChild(i);
            newParent.Reset();
            newParent.parent = transform;
            newParent.SetSiblingIndex(i);
            transform.GetChild(i+1).parent = newParent;
        }
    }
}

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(RadialPositioning))]
class RadialPositioningEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        RadialPositioning script = (RadialPositioning)target;
        GUILayout.Space(5);

        if (GUILayout.Button("Setup empty parents"))
            script.SetupEmptyParents();

        if (GUILayout.Button("Select empty parent children"))
        {
            List<GameObject> emptyParentChilren = new List<GameObject>();
            foreach (Transform emptyParent in script.transform)
                emptyParentChilren.Add(emptyParent.GetChild(0).gameObject);
            UnityEditor.Selection.objects = emptyParentChilren.ToArray();
        }
    }
}
#endif