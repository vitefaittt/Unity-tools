using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class ConnectedStatus : MonoBehaviour
{
    Text display;


    void Reset()
    {
        this.RenameFromType();
    }

    private void Start()
    {
        display = GetComponent<Text>();
    }

    private void Update()
    {
        if (display)
            display.text = PhotonNetwork.NetworkClientState.ToString();
    }


    public void SetupText()
    {
        this.GetOrAddComponent<Text>().text = "Connection state";
    }
}

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(ConnectedStatus)), UnityEditor.CanEditMultipleObjects]
class ConnectedStatusEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GUILayout.Space(5);

        ConnectedStatus script = (ConnectedStatus)target;

        GUILayout.Label("Connection state: " + PhotonNetwork.NetworkClientState.ToString());

        if (GUILayout.Button(new GUIContent("Setup text", "Create a text component with default message")))
            script.SetupText();
    }
}
#endif
