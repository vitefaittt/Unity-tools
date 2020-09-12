using System.Linq;
using UnityEngine;

namespace InspectorExtensions
{
    [ExecuteInEditMode]
    public class HierarchySeparator : MonoBehaviour
    {
        static readonly char filler = '░';
        static readonly char border = 'I';
        static readonly int targetLength = 55;
        public string title;
        bool isSelected;


        void Reset()
        {
            // Reset transform, pick title then set separator name.
            transform.localPosition = transform.localEulerAngles = Vector3.zero;
            transform.localScale = Vector3.one;
            title = gameObject.name;
            gameObject.name = ComputeName(title);
        }

#if UNITY_EDITOR
        void OnEnable()
        {
            UnityEditor.Selection.selectionChanged += SelectionCallback;
        }

        void OnDisable()
        {
            UnityEditor.Selection.selectionChanged -= SelectionCallback;
        }


        void SelectionCallback()
        {
            // While selected, set gameobject name back to title.
            if (UnityEditor.Selection.activeGameObject == gameObject)
            {
                gameObject.name = title;
                isSelected = true;
            }
            else if (isSelected)
            {
                isSelected = false;
                gameObject.name = ComputeName(title);
            }
        }
#endif

        public static string ComputeName(string input)
        {
            // Return input surrounded by filler and border.
            if (string.IsNullOrEmpty(input))
                input = " ";
            return new string(new char[] { filler, border, ' ' }) + input + new string(new char[] { ' ', border }) + new string(Enumerable.Repeat(filler, targetLength - input.Length).ToArray());
        }
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(HierarchySeparator)), UnityEditor.CanEditMultipleObjects]
    class HierarchySeparatorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            // Draw preview.
            GUILayout.Label("Preview:", UnityEditor.EditorStyles.miniLabel);
            GUILayout.Space(-2);
            GUILayout.Label(HierarchySeparator.ComputeName(((HierarchySeparator)target).title));
        }
    }
#endif
}