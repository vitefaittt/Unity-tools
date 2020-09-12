using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    [SerializeField]
    float delay = 3;


    void Start()
    {
        Invoke("DoDestroy", delay);
    }


    void DoDestroy()
    {
        Destroy(gameObject);
    }
}
