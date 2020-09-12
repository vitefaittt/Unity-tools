using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MathUtilities
{
    public static float StandardDeviation(this List<float> values)
    {
        if (values.Count < 1)
            return 0;
        float average = values.Average();
        return Mathf.Sqrt(values.Average(v => (v - average) * (v - average)));
    }

    public static Vector3 Divide(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
    }
}
