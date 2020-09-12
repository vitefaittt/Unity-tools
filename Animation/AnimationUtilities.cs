using UnityEngine;

public static class AnimationUtilities
{
    public static void EnableEmission(this Renderer[] rends)
    {
        foreach (Renderer rend in rends)
            foreach (Material mat in rend.materials)
                mat.EnableKeyword("_EMISSION");
    }

    public static float SinTime01(float speed = 1, float strength = 1)
    {
        return Mathf.Sin(Time.time * speed).To01() * strength;
    }

    public static AnimationCurve GetSmoothCurve(float size = 1)
    {
        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(0, 0);
        curve.AddKey(.1f, size);
        curve.AddKey(.9f, size);
        curve.AddKey(1, 0);
        return curve;
    }
}
