using System.Collections.Generic;
using UnityEngine;

public class SplineProgressionFinder : MonoBehaviour
{
    Spline savedSpline;
    Spline Spline { get {
            if (!savedSpline)
                savedSpline = GetComponent<Spline>();
            return savedSpline;
        } }
    [SerializeField]
    int slicing = 20;
    List<Vector3> savedEstimationPoints = new List<Vector3>();
    List<Vector3> EstimationPoints {
        get {
            // Split the curve into a number of points.
            if (savedEstimationPoints.Count == 0)
                for (int i = 0; i < slicing; i++)
                    savedEstimationPoints.Add(Spline.GetPoint(i / (float)slicing));
            return savedEstimationPoints;
        }
    }

    struct Line
    {
        public Vector3 pointA, pointB;
        public Line(Vector3 pA, Vector3 pb)
        {
            pointA = pA;
            pointB = pb;
            pointB = pb;
            Length = Vector3.Distance(pointA, pointB);
            Direction = (pointB - pointA).normalized;
        }
        public float Length { get; private set; }
        public Vector3 Direction { get; private set; }
    }


    private void OnDrawGizmosSelected()
    {
        savedEstimationPoints.Clear();
        for (int i = 0; i < EstimationPoints.Count - 1; i++)
            Debug.DrawLine(EstimationPoints[i], EstimationPoints[i + 1], Color.white * .8f);
        if (Spline.Loop)
            Debug.DrawLine(EstimationPoints[EstimationPoints.Count - 1], EstimationPoints[0], Color.white * .8f);
    }


    public float GetProgression(Transform target)
    {
        // Get point A.
        int estimationPointA = 0;
        for (int i = 1; i < EstimationPoints.Count; i++)
            if (Vector3.Distance(EstimationPoints[i], target.position) < Vector3.Distance(EstimationPoints[estimationPointA], target.position))
                estimationPointA = i;

        // Get point B.
        int estimationPointB = 0;
        if (Vector3.Distance(EstimationPoints[(estimationPointA == EstimationPoints.Count - 1) ? 0 : (estimationPointA + 1)], target.position) < Vector3.Distance(EstimationPoints[(estimationPointA == 0) ? (EstimationPoints.Count - 1) : (estimationPointA - 1)], target.position))
            estimationPointB = (estimationPointA == EstimationPoints.Count - 1) ? 0 : (estimationPointA + 1);
        else
            estimationPointB = (estimationPointA == 0) ? (EstimationPoints.Count - 1) : (estimationPointA - 1);

        // Make it so that a comes before b.
        if (estimationPointB < estimationPointA)
            Permute(ref estimationPointA, ref estimationPointB);
        if (estimationPointB == EstimationPoints.Count - 1 && estimationPointA == 0)
            Permute(ref estimationPointA, ref estimationPointB);

        // Correct result by looking at the lines formed by the points.
        Line resultLine = new Line(EstimationPoints[estimationPointA], EstimationPoints[estimationPointB]);
        if (Vector3.Dot(resultLine.Direction, (target.position - resultLine.pointA).normalized) < 0)
        {
            // The target is behind the line, pick the previous points if possible.
            if (!Spline.Loop && estimationPointB == 0)
                return 0;
            estimationPointA = estimationPointA <= 0 ? (EstimationPoints.Count - 1) : (estimationPointA - 1);
            estimationPointB = estimationPointB <= 0 ? (EstimationPoints.Count - 1) : (estimationPointB - 1);
        }
        else if (Vector3.Dot(-resultLine.Direction, (target.position-resultLine.pointB).normalized) < 0)
        {
            // The target is ahead of the line, pick the next points if possible.
            if (!Spline.Loop && estimationPointB == 0)
                return 1;
            estimationPointA = estimationPointA >= EstimationPoints.Count - 1 ? 0 : (estimationPointA + 1);
            estimationPointB = estimationPointB >= EstimationPoints.Count - 1 ? 0 : (estimationPointB + 1);
        }

        // Get progression on closest line.
        Line closestLine = new Line(EstimationPoints[estimationPointA], EstimationPoints[estimationPointB]);
        float progressionOnLine = GetProgressionOnLine(closestLine, target.position);

        // Return progression between the two points.
        float firstPointProgression = estimationPointA / (float)EstimationPoints.Count;
        float secondPointProgression = estimationPointB == 0 ? 1 : (estimationPointB / (float)EstimationPoints.Count);
        return Mathf.Lerp(firstPointProgression, secondPointProgression, progressionOnLine);
    }

    float GetProgressionOnLine(Line line, Vector3 targetPosition)
    {
        float distance = Vector3.Distance(GetProjectedLinePoint(targetPosition, line), line.pointA);
        float progression = distance / line.Length;
        return progression;
    }

    Vector3 GetProjectedLinePoint(Vector3 point, Line line)
    {
        // Source: https://math.stackexchange.com/questions/1905533/find-perpendicular-distance-from-point-to-line-in-3d
        Vector3 direction = (line.pointB - line.pointA) / line.Length;
        Vector3 lineOriginToPoint = point - line.pointA;
        float projection = Vector3.Dot(lineOriginToPoint, direction);
        Vector3 projectedPoint = line.pointA + projection * direction;
        return projectedPoint;
    }

    static void Permute(ref int a, ref int b)
    {
        int holder = a;
        a = b;
        b = holder;
    }
}
