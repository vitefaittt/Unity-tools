using UnityEngine;

public class CrepuscularLight : MonoBehaviour
{
    [SerializeField]
    [TextArea]
    string note = "Turns on the crepuscular effect on an incoming crepuscular behaviour camera.";


    private void Start()
    {
        if (CrepuscularSceneBehaviour.Instance)
            CrepuscularSceneBehaviour.Instance.AddCrepuscularLight(GetComponent<Light>());
    }
}
