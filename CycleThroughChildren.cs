using UnityEngine;

public class CycleThroughChildren : MonoBehaviour
{
    int currentChild;


    private void Reset()
    {
        GoToChild(0);
    }


    public void Next()
    {
        Utilities.LimitedIncrement(ref currentChild, transform.childCount);
        GoToChild(currentChild);
    }

    public void Previous()
    {
        Utilities.LimitedDecrement(ref currentChild, transform.childCount);
        GoToChild(currentChild);
    }

    public void GoToChild(int index)
    {
        if (!index.IsBetween(0, transform.childCount - 1) || transform.childCount < 1)
            return;
        currentChild = index;
        transform.HideChildren();
        transform.GetChild(currentChild).gameObject.SetActive(true);
    }
}

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(CycleThroughChildren))]
class CycleThroughChildrenEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        CycleThroughChildren script = (CycleThroughChildren)target;
        GUILayout.Space(5);

        if (script.transform.childCount < 1)
            GUILayout.Label("No children");

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("← Previous"))
            script.Previous();
        if (GUILayout.Button("Next →"))
            script.Next();
        GUILayout.EndHorizontal();

        GUILayout.Space(5);
        GUILayout.Label("Per child", UnityEditor.EditorStyles.boldLabel);

        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        for (int i = 0; i < script.transform.childCount; i++)
        {
            buttonStyle.normal.textColor = buttonStyle.hover.textColor = script.transform.GetChild(i).gameObject.activeInHierarchy ? Color.black : Color.grey;
            if (GUILayout.Button(script.transform.GetChild(i).name, buttonStyle))
                script.GoToChild(i);
        }

        GUILayout.Space(5);
        if (GUILayout.Button("Show all"))
            script.gameObject.ShowChildren();
        if (GUILayout.Button("Show none"))
            script.gameObject.HideChildren();
    }
}
#endif
