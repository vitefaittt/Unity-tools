using UnityEngine;

public class ScrollScaleZoom : MonoBehaviour
{
    Vector3 awakeLocalScale;
    [SerializeField]
    Vector2 minMaxScale;
    bool IsAtMinScale => transform.localScale.magnitude <= minMaxScale.x;
    bool IsAtMaxScale => transform.localScale.magnitude >= minMaxScale.y;


    void Reset()
    {
        minMaxScale.x = transform.localScale.magnitude * .33f;
        minMaxScale.y = transform.localScale.magnitude * 3;
    }

    void Awake()
    {
        awakeLocalScale = transform.localScale;
    }

    void OnEnable()
    {
        transform.localScale = awakeLocalScale;
    }

    void Update()
    {
        float inputZoom = Input.mouseScrollDelta.y * awakeLocalScale.magnitude * .25f;
        if (inputZoom < 0 && IsAtMinScale)
            return;
        if (inputZoom > 0 && IsAtMaxScale)
            return;
        transform.localScale = transform.localScale + Vector3.one * (Input.mouseScrollDelta.y * awakeLocalScale.magnitude * .1f);
    }
}
