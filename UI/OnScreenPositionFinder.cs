using UnityEngine;

/// <summary>
/// Gets the position of a target object on the screen, translated to local position for the child of a canvas.
/// </summary>
public class OnScreenPositionFinder
{
    Camera camera;
    Vector2 xLocalPosMinMax;
    Vector2 yLocalPosMinMax;

    /// <summary>
    /// [Hint: should be created on Start.]
    /// </summary>
    public OnScreenPositionFinder(RectTransform canvas, Camera camera)
    {
        this.camera = camera;
        UpdateScreenSize(canvas);
    }

    public void UpdateScreenSize(RectTransform canvas)
    {
        Vector2 parentWidthHeight = new Vector2(canvas.rect.width, canvas.rect.height);
        xLocalPosMinMax = new Vector2(-parentWidthHeight.x * .5f, parentWidthHeight.x * .5f);
        yLocalPosMinMax = new Vector2(-parentWidthHeight.y * .5f, parentWidthHeight.y * .5f);
    }

    /// <summary>
    /// Get the position of the target object on the screen, translated to localPos.
    /// </summary>
    public Vector2 GetPosition(Transform target)
    {
        if (!target)
            return Vector2.zero;

        // Get the pogression on screen of the target, then use it to get our localPosition in the canvas.
        Vector2 targetScreenPos = camera.WorldToScreenPoint(target.position);
        Vector2 screenProgression = new Vector2(targetScreenPos.x / Screen.width, targetScreenPos.y / Screen.height);
        return new Vector2(
            Mathf.Lerp(xLocalPosMinMax.x, xLocalPosMinMax.y, screenProgression.x),
            Mathf.Lerp(yLocalPosMinMax.x, yLocalPosMinMax.y, screenProgression.y));
    }
}