using System;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TeleportationCurve : MonoBehaviour
{
    LineRenderer lineRenderer;
    Vector3[] controlPoints = new Vector3[3];
    float extendStep = 5;
    const int SEGMENT_COUNT = 10;
    public float extensionFactor;

    public Vector3? EndPoint { get; private set; }
    public Func<RaycastHit, bool> HitCondition = null;


    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
    }

    void Update()
    {
        UpdateControlPoints();
        HandleExtension();
        DrawCurve();
    }


    public void ToggleDraw(bool draw)
    {
        if (lineRenderer != null)
            lineRenderer.enabled = draw;
    }

    void HandleExtension()
    {
        if (extensionFactor == 0)
            return;

        float finalExtension = extendStep + Time.deltaTime * extensionFactor * 2;
        extendStep = Mathf.Clamp(finalExtension, 2.5f, 7.5f);
    }

    void UpdateControlPoints()
    {
        // The first control is the remote. The second is a forward projection. The third is a forward and downward projection.
        controlPoints[0] = transform.position;
        controlPoints[1] = controlPoints[0] + (transform.forward * extendStep * 2 / 5f);
        controlPoints[2] = new Vector3(
            controlPoints[1].x + (transform.forward.x * extendStep * 3 / 5f),
            0,
            controlPoints[1].z + (transform.forward.z * extendStep * 3 / 5f));
    }

    void DrawCurve()
    {
        if (!lineRenderer.enabled)
            return;

        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, controlPoints[0]);

        Vector3 prevPosition = controlPoints[0];
        Vector3 nextPosition;
        for (int i = 1; i <= SEGMENT_COUNT; i++)
        {
            float progression = i / (float)SEGMENT_COUNT;
            lineRenderer.positionCount = i + 1;

            if (i == SEGMENT_COUNT)
            {
                // For the last point, project out the curve two more meters.
                Vector3 endDirection = Vector3.Normalize(prevPosition - lineRenderer.GetPosition(i - 2));
                nextPosition = prevPosition + endDirection * 2;
            }
            else
                nextPosition = GetBezierPoint(progression, controlPoints[0], controlPoints[1], controlPoints[2]);

            if (GetEndPoint(prevPosition, nextPosition))
            {
                // If the segment intersects a surface, draw the point and return.
                lineRenderer.SetPosition(i, (Vector3)EndPoint);
                return;
            }
            else
            {
                // If the point does not intersect, continue to draw the curve.
                lineRenderer.SetPosition(i, nextPosition);
                prevPosition = nextPosition;
            }
        }
    }

    bool GetEndPoint(Vector3 start, Vector3 end)
    {
        Ray ray = new Ray(start, end - start);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Vector3.Distance(start, end)) && (HitCondition == null || HitCondition(hit)))
        {
            EndPoint = hit.point;
            return true;
        }
        EndPoint = null;
        return false;
    }

    Vector3 GetBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        return
            Mathf.Pow((1 - t), 2) * p0 +
            2f * (1 - t) * t * p1 +
            Mathf.Pow(t, 2) * p2;
    }
}
