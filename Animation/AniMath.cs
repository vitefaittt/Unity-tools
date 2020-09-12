using UnityEngine;

public static class AniMath
{
    /// <summary>
    /// Smoothen the start and the end of a value going from 0 to 1.
    /// </summary>
    /// <param name="value">Value to smoothen.</param>
    /// <returns>Smoothed value.</returns>
    public static float Smooth(float value)
    {
        return 1 - (Mathf.Cos(value * 3.1416f) + 1) * .5f;
    }

    /// <summary>
    /// Smoothen the start of a value going from 0 to 1.
    /// </summary>
    /// <param name="value">Value to smoothen.</param>
    /// <returns>Smoothed value.</returns>
    public static float SmoothStart(float value)
    {
        return 1 - Mathf.Cos(value * 1.5708f);
    }

    /// <summary>
    /// Smoothen the end of a value going from 0 to 1.
    /// </summary>
    /// <param name="value">Value to smoothen.</param>
    /// <returns>Smoothed value.</returns>
    public static float SmoothEnd(float value)
    {
        return Mathf.Sin(value * 1.5708f);
    }

    /// <summary>
    /// Map a value going from 0 to 1 to a bell curve.
    /// </summary>
    /// <param name="value">Value to map on the bell curve.</param>
    /// <returns>Value from the bell curve.</returns>
    public static float Bell(float value)
    {
        return (Mathf.Sin(2 * Mathf.PI * (value - .25f)) + 1) * .5f;
    }
}