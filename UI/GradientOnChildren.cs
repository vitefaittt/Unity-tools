using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class GradientOnChildren : MonoBehaviour
{
    [SerializeField]
    Color colorStart = Color.white, colorEnd = Color.clear;


    private void Update()
    {
        if (Application.isPlaying)
            return;
        Image[] images = GetComponentsInChildren<Image>();
        for (int i = 0; i < transform.childCount; i++)
            images[i].color = Color.Lerp(colorStart, colorEnd, i / (float)transform.childCount);
    }
}
