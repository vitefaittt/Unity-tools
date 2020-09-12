using UnityEngine;
using UnityEngine.UI;

public class UIFlashAnim : FlashAnim
{
    Image image;
    [SerializeField]
    Color startColor, targetColor;


    void Reset()
    {
        startColor = GetComponent<Image>().color;
        targetColor = startColor * .7f;
    }

    void Awake()
    {
        image = GetComponent<Image>();
    }


    protected override void HandleProgression(float progression)
    {
        image.color = Color.Lerp(startColor, targetColor, progression);
    }
}
