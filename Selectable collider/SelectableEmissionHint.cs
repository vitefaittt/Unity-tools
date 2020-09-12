using UnityEngine;

public class SelectableEmissionHint : MonoBehaviour
{
    EmissionHint emissionHint;


    void Reset()
    {
        this.GetOrAddComponent<SelectableCollider>();
        this.GetOrAddComponent<EmissionHint>();
    }

    void Awake()
    {
        // Hint when the mouse passes over us.
        emissionHint = GetComponent<EmissionHint>();
        GetComponent<SelectableCollider>().MouseEnter.AddListener(emissionHint.Hint);
        GetComponent<SelectableCollider>().MouseExit.AddListener(emissionHint.UnHint);

    }

    void OnDisable()
    {
        emissionHint.UnHint();
    }
}
