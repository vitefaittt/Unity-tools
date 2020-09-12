using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arrays
{
    [ExecuteInEditMode]
    public class RadialArray : Array
    {
        [Header("RadialArray")]
        [SerializeField]
        [Tooltip("Minimum amount of imaginary balls to spawn, determines the space between each spawned object.")]
        int minIncrements = 8;
        [SerializeField]
        float radius = 1;
        public enum RotationType { LookAtCenter, RotationAxis, None}
        [Header("Rotation")]
        [SerializeField]
        public RotationType rotationType;
        [SerializeField]
        BoolVector3 rotationAxis;
        [Header("Navigation")]
        [SerializeField]
        bool smoothTransition = true;
        [SerializeField]
        float transitionSpeed = 400;
        [SerializeField]
        bool reverseAtTheEnd = true;
        int currentIndex = 0;
        Quaternion startRotation;
        float IncrementAngle => 360f / Mathf.Max(amount, minIncrements);


        private void Awake()
        {
            startRotation = transform.rotation;
        }


        public override void UpdateItemsTransform()
        {
            // Update each child's position.
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).localPosition = PointOnCircle(radius, i / (float)Mathf.Max(amount, minIncrements) * 360);
                // Rotate our elemets.
                switch (rotationType)
                {
                    case RotationType.LookAtCenter:
                        transform.GetChild(i).rotation = Quaternion.LookRotation((transform.GetChild(i).transform.position - transform.position).normalized, transform.forward);
                        break;
                    case RotationType.RotationAxis:
                        float angle = (i/(float)transform.childCount) * -360;
                        transform.GetChild(i).localEulerAngles = new Vector3(rotationAxis.x?angle: 0, rotationAxis.y ? angle : 0, rotationAxis.z ? angle : 0);
                        break;
                    case RotationType.None:
                        break;
                    default:
                        break;
                }
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
                if (currentIndex < amount)
                    StartCoroutine(Rotate(IncrementAngle, transitionSpeed));
                else
                {
                    currentIndex = 0;
                    StartCoroutine(Rotate(reverseAtTheEnd ? -IncrementAngle * (amount - 1) : IncrementAngle * (amount < minIncrements ? minIncrements - amount + 1 : 1), transitionSpeed * 2));
                }
                return;
            }

            // Rotate the array instantly.
            Utilities.LimitedIncrement(ref currentIndex, transform.childCount);
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
                    currentIndex = amount - 1;
                    StartCoroutine(Rotate(reverseAtTheEnd ? IncrementAngle * (amount - 1) : -IncrementAngle * (amount < minIncrements ? minIncrements - amount + 1 : 1), transitionSpeed * 2));
                }
                return;
            }

            // Rotate the array instantly.
            Utilities.LimitedDecrement(ref currentIndex, transform.childCount);
            if (currentIndex == transform.childCount - 1)
                transform.Rotate(Vector3.forward, IncrementAngle * (transform.childCount - 1), Space.Self);
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
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(RadialArray))]
    class RadialArrayEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            RadialArray script = (RadialArray)target;

            // Show the rotation axis only when we are using it.
            List<string> excludedProperties = new List<string>();
            if (script.rotationType != RadialArray.RotationType.RotationAxis)
                excludedProperties.Add("rotationAxis");
            DrawPropertiesExcluding(serializedObject, excludedProperties.ToArray());

            GUILayout.Space(5);

            if (GUILayout.Button("Respawn items"))
                script.Respawn();

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}