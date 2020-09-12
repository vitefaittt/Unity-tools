using UnityEngine;

[ExecuteInEditMode]
public class FittedRect : MonoBehaviour
{
    RectTransform RectTransform { get { return (RectTransform)transform; } }
    [SerializeField]
    float margin = 30;
    DeviceOrientation lastOrientation;
    DeviceOrientation currentOrientation { get { return lastOrientation = Screen.width < Screen.height ? DeviceOrientation.Portrait : DeviceOrientation.LandscapeLeft; } }


    private void Start()
    {
        UpdateRect();
    }

    private void LateUpdate()
    {
        if (lastOrientation != currentOrientation)
            UpdateRect();
    }


    void UpdateRect()
    {
        switch (currentOrientation)
        {
            case DeviceOrientation.Portrait:
            case DeviceOrientation.PortraitUpsideDown:
                RectTransform.anchorMin = new Vector2(0, .5f);
                RectTransform.anchorMax = new Vector2(1, .5f);
                RectTransform.sizeDelta = RectTransform.sizeDelta.SetX(-margin*2);
                RectTransform.sizeDelta = RectTransform.sizeDelta.SetY(RectTransform.rect.width);
                break;
            case DeviceOrientation.LandscapeLeft:
            case DeviceOrientation.LandscapeRight:
                RectTransform.anchorMin = new Vector2(.5f, 0);
                RectTransform.anchorMax = new Vector2(.5f, 1);
                RectTransform.sizeDelta = RectTransform.sizeDelta.SetY(-margin*2);
                RectTransform.sizeDelta = RectTransform.sizeDelta.SetX(RectTransform.rect.height);
                break;
            default:
                break;
        }
        lastOrientation = currentOrientation;
    }
}
