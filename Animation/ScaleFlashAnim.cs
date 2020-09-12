using UnityEngine;

public class ScaleFlashAnim : FlashAnim
{
    Vector3 awakeScale;
    public float intensity = 1.3f;


    void Awake()
    {
        awakeScale = transform.localScale;
    }


    protected override void HandleProgression(float progression)
    {
        transform.localScale = Vector3.Lerp(awakeScale, awakeScale * intensity,progression);
    }
}
