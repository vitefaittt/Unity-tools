using UnityEngine;

public class StateAtStart : MonoBehaviour
{
    [SerializeField]
    bool activeOnStart;


    void Start()
    {
        gameObject.SetActive(activeOnStart);
    }
}
