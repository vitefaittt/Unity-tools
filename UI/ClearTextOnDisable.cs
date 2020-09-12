using UnityEngine;
using UnityEngine.UI;

public class ClearTextOnDisable : MonoBehaviour
{
    [SerializeField]
    Text textToClear;


    void Reset()
    {
        textToClear = GetComponentInChildren<Text>();
    }

    void OnDisable()
    {
        textToClear.text = "";
    }
}
