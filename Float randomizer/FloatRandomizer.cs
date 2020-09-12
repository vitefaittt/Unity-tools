using UnityEngine;

public class FloatRandomizer : MonoBehaviour
{
    [SerializeField]
    RemoteFloatField target;
    [SerializeField]
    Vector2 range = new Vector2(-1, 1);
    float defaultValue;


    void Reset()
    {
        target.targetGameObject = gameObject;
        target.targetComponent = GetComponent<MonoBehaviour>();
    }

    void Start()
    {
        defaultValue = target.GetValue();
    }

    void Update()
    {
        target.SetValue(defaultValue * Random.Range(range.x, range.y));
    }
}
