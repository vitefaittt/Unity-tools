using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceTabs : MonoBehaviour
{
    [SerializeField]
    Transform panelsParent;
    public Transform PanelsParent => panelsParent;
    Button[] childrenButtons;
    [SerializeField]
    bool highlightSelection = true;
    Dictionary<Button, Color> defaultColors = new Dictionary<Button, Color>();
    [SerializeField]
    [Tooltip("Set to -1 to not select anything on enable.")]
    int defaultIndexOnEnable = 0;


    void Awake()
    {
        // Set each button to open its panel by index.
        childrenButtons = GetComponentsInChildren<Button>();
        for (int i = 0; i < childrenButtons.Length; i++)
        {
            int index = i;
            childrenButtons[i].onClick.AddListener(() =>
            {
                OpenPanel(index);
            });
        }
    }

    void OnEnable()
    {
        if (defaultIndexOnEnable >= 0)
            OpenPanel(defaultIndexOnEnable);
    }


    void OpenPanel(int index)
    {
        panelsParent.HideChildren();
        panelsParent.GetChild(index).gameObject.SetActive(true);

        // Highlight selection.
        if (highlightSelection)
        {
            for (int i = 0; i < childrenButtons.Length; i++)
            {
                if (!defaultColors.ContainsKey(childrenButtons[i]))
                    defaultColors.Add(childrenButtons[i], childrenButtons[i].GetComponent<Image>().color);
                childrenButtons[i].GetComponent<Image>().color = (i == index) ? ((Color.white - defaultColors[childrenButtons[i]]) * .5f) : defaultColors[childrenButtons[i]];
            }
        }
    }
}


#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(InterfaceTabs)), UnityEditor.CanEditMultipleObjects]
class InterfaceTabsEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GUILayout.Space(5);

        InterfaceTabs script = (InterfaceTabs)target;

        GUIStyle messageStyle = new GUIStyle(GUI.skin.label);
        messageStyle.fontStyle = FontStyle.Italic;
        GUILayout.Label(script.GetComponentsInChildren<Button>().Length + " buttons matched to " + (!script.PanelsParent ? "no" : script.PanelsParent.childCount.ToString()) + " panels.", messageStyle);
    }
}
#endif
