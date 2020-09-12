using System;
using UnityEngine;

[ExecuteInEditMode]
public class BoundingBox : MonoBehaviour
{
    [SerializeField]
    MeshRenderer[] ridges;
    [SerializeField]
    MeshRenderer[] corners;
    [SerializeField]
    float width = 1;
    [SerializeField]
    Color color = Color.white;
    public Color Color => color;
    [SerializeField]
    float ridgesYTextureScale = 3;
    public bool updateContinuously;


    void Reset()
    {
        ridges = Array.FindAll(GetComponentsInChildren<MeshRenderer>(), rend => rend.name.StartsWith("Ridge", StringComparison.CurrentCultureIgnoreCase));
        corners = Array.FindAll(GetComponentsInChildren<MeshRenderer>(), rend => rend.name.StartsWith("Corner", StringComparison.CurrentCultureIgnoreCase));
    }

    void Update()
    {
        if (!updateContinuously)
            return;
        UpdateWidth();
        UpdateColor();
    }


    [ContextMenu("Update width")]
    void UpdateWidth() => SetWidth(width);
    public void SetWidth(float width)
    {
        this.width = width;
        for (int i = 0; i < ridges.Length; i++)
        {
            Utilities.MaintainScale(ridges[i].transform, width, true, true, false);
            float yTextureScale = ridgesYTextureScale * ridges[i].transform.lossyScale.z;
            (Application.isPlaying ? ridges[i].material : ridges[i].sharedMaterial).SetTextureScale("_MainTex", new Vector2(1, yTextureScale));
            (Application.isPlaying ? ridges[i].material : ridges[i].sharedMaterial).SetTextureOffset("_MainTex", new Vector2(0, -yTextureScale * .5f));
        }
        for (int i = 0; i < corners.Length; i++)
            Utilities.MaintainScale(corners[i].transform, width);
    }

    [ContextMenu("Update color")]
    void UpdateColor() => SetColor(color);
    public void SetColor(Color color)
    {
        this.color = color;
        for (int i = 0; i < ridges.Length; i++)
            (Application.isPlaying ? ridges[i].material : ridges[i].sharedMaterial).color = color;
        for (int i = 0; i < corners.Length; i++)
            (Application.isPlaying ? corners[i].material : corners[i].sharedMaterial).color = color;
    }

    public void FitBounds(Bounds bounds)
    {
        transform.position = bounds.center;
        transform.localScale = bounds.size;
        UpdateWidth();
    }
}
