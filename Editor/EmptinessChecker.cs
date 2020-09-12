using System.Reflection;
using UnityEditor;
using UnityEngine;

public class EmptinessChecker
{
    [MenuItem("Window/Tools/Find empty fields")]
    static void FindEmptyFields()
    {
        GameObject[] go = Selection.gameObjects;
        Transform[] transforms = UnityEngine.Object.FindObjectsOfType<Transform>();
        foreach (Transform transform in transforms)
        {
            string log = "";
            Component[] components = transform.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                if (!components[i])
                    log += "\nempty script";
                else if (components[i] is MonoBehaviour)
                {
                    BindingFlags bindingFlags = BindingFlags.Public |
                             BindingFlags.NonPublic |
                             BindingFlags.Instance |
                             BindingFlags.Static;
                    FieldInfo[] fieldInfos = components[i].GetType().GetFields(bindingFlags);
                    foreach (FieldInfo info in fieldInfos)
                        if (info.IsPublic || info.GetCustomAttribute(typeof(SerializeField)) != null)
                            if (info.GetValue(components[i]).ToString() == "null" || (info.FieldType == typeof(string) && string.IsNullOrWhiteSpace((string)info.GetValue(components[i]))))
                                log += "\nempty: " + info.Name;
                }

            }
            if (!string.IsNullOrEmpty(log))
                Debug.Log("<b>" + transform.gameObject.name + "</b>\n<i>Select to view details</i>" + log);
        }
    }
}
