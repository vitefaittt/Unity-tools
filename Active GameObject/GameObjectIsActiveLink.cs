using UnityEngine;

public class GameObjectIsActiveLink : MonoBehaviour
{
    public GameObject linkedGameObject;
    [HideInInspector]
    public bool isWatched = false;
    [SerializeField]
    [Tooltip("If checked, our state will be inverted from the linkedGameobject.")]
    bool invert;


    private void Start()
    {
        if (!linkedGameObject || isWatched)
            return;

        // Send another component to the object that we are watching.
        GameObjectIsActiveLink otherLink = linkedGameObject.AddComponent<GameObjectIsActiveLink>();
        otherLink.isWatched = true;
        otherLink.linkedGameObject = gameObject;

        UpdateState(linkedGameObject.activeInHierarchy);
    }

    private void OnEnable()
    {
        if (isWatched)
            linkedGameObject.GetComponent<GameObjectIsActiveLink>().UpdateState(true);
    }

    private void OnDisable()
    {
        if (isWatched && linkedGameObject != null)
            linkedGameObject.GetComponent<GameObjectIsActiveLink>().UpdateState(false);
    }


    void UpdateState(bool incomingState)
    {
        gameObject.SetActive(invert ? !incomingState : incomingState);
    }
}

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(GameObjectIsActiveLink))]
class IsActiveLinkEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        GameObjectIsActiveLink script = (GameObjectIsActiveLink)target;
        if (!script.isWatched)
        {
            DrawDefaultInspector();
            return;
        }

        GUILayout.Label("This object is syncing its state with Linked Game Object.");

        System.Collections.Generic.List<string> excludedProperties = new System.Collections.Generic.List<string>();
        excludedProperties.Add("invert");

        DrawPropertiesExcluding(serializedObject, excludedProperties.ToArray());
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
