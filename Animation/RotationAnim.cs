using UnityEngine;
using UnityEngine.Events;

public class RotationAnim : MonoBehaviour
{
    [SerializeField]
    float degreesPerSec = 60;
    [SerializeField]
    Vector3 rotAxis = Vector3.up;

    public UnityEvent OnPlay, OnPause;


    private void Start()
    {
        rotAxis.Normalize();
    }

    private void Update()
    {
        transform.Rotate(rotAxis, degreesPerSec * Time.deltaTime, Space.Self);
    }

    void OnEnable()
    {
        OnPlay.Invoke();
    }

    void OnDisable()
    {
        OnPause.Invoke();
    }
}
