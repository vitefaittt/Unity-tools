using UnityEngine;

public class ScannedObject : MonoBehaviour {

    public static GameObject GameObjectToEdit { get; private set; }

    private void Awake()
    {
        GameObjectToEdit = GetComponentInChildren<Renderer>().gameObject;
    }
}
