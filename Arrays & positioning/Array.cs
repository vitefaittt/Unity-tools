using UnityEngine;

namespace Arrays
{
    [ExecuteInEditMode]
    public abstract class Array : MonoBehaviour
    {
        [SerializeField]
        GameObject template;
        [SerializeField]
        public int amount = 2;
        [SerializeField]
        bool updateInPlayMode = false;
        [SerializeField]
        bool disableNonPrefabTemplate = true;

        public event System.Action TransformsUpdated;


        private void Reset()
        {
            if (name == "GameObject")
                this.RenameFromType();
            transform.DestroyChildren();
        }

        protected virtual void Update()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
#endif
                if (!updateInPlayMode)
                    return;

            if (!template)
                return;

            // Update array items.
            UpdateItemsCount();
            UpdateItemsTransform();
            TransformsUpdated?.Invoke();
        }


        [ContextMenu("Respawn")]
        public void Respawn()
        {
            transform.DestroyChildren();
        }

        public void UpdateItemsCount()
        {
            // Update count.
            if (transform.childCount < TotalAmount())
            {
                if (disableNonPrefabTemplate)
                    template.SetActive(true);
                for (int i = transform.childCount; i < TotalAmount(); i++)
                {
#if UNITY_EDITOR
                    // Instantiate the template as a prefab if it is a prefab.
                    if (UnityEditor.PrefabUtility.GetPrefabInstanceStatus(template) != UnityEditor.PrefabInstanceStatus.NotAPrefab)
                    {
                        if (disableNonPrefabTemplate)
                            UnityEditor.PrefabUtility.InstantiatePrefab(template, transform);
                        continue;
                    }
#endif
                    Instantiate(template, transform);
                }
                if (disableNonPrefabTemplate)
                    template.SetActive(false);
            }
            else if (transform.childCount > TotalAmount() && TotalAmount() >= 0)
                for (int i = transform.childCount; i > TotalAmount(); i--)
                    DestroyImmediate(transform.GetChild(0).gameObject);
        }

        protected virtual int TotalAmount()
        {
            return amount;
        }

        public abstract void UpdateItemsTransform();
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(Array), true)]
    class ArrayEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            Array script = (Array)target;
            GUILayout.Space(5);

            if (GUILayout.Button("Respawn"))
                script.Respawn();
        }
    }
#endif
}