using UnityEngine;

public class PointerMeshCutterRenderer : MonoBehaviour
{
    LineRenderer line;
    PointerMeshCutter cutter;


    private void Reset()
    {
        line = this.GetOrAddComponent<LineRenderer>();
        line.useWorldSpace = true;
    }

    private void Awake()
    {
        cutter = GetComponent<PointerMeshCutter>();
        line = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        line.SetPosition(0, transform.position);
        if (cutter.IsInRange)
            line.SetPosition(1, cutter.hit.point);
    }
}
