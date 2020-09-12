using UnityEngine;
using UnityEngine.UI;

public class TransitionMask : MonoBehaviour
{
    Image image;


    private void Awake()
    {
        image = GetComponent<Image>();
        image.color = Color.black;
        image.enabled = false;
    }


    public static TransitionMask Create(RectTransform parent, Color color = new Color())
    {
        GameObject maskObject = new GameObject("Transition mask", typeof(RectTransform), typeof(Image));
        maskObject.transform.parent = parent;
        ((RectTransform)maskObject.transform).anchorMin = Vector2.zero;
        ((RectTransform)maskObject.transform).anchorMax = Vector2.one;
        ((RectTransform)maskObject.transform).anchoredPosition = Vector2.zero;
        maskObject.transform.localScale = Vector3.one;
        maskObject.GetComponent<Image>().color = color;
        return maskObject.AddComponent<TransitionMask>();
    }

    public void Play(float duration, System.Action MidWayAction, System.Action EndAction = null)
    {
        image.enabled = true;
        transform.SetSiblingIndex(transform.parent.childCount - 1);
        bool midwayReached = false;
        this.ProgressionAnim(duration, delegate (float progression)
        {
            image.color = image.color.SetA(AniMath.Bell(progression));
            if (progression >= .5f && !midwayReached)
            {
                if (MidWayAction != null)
                    MidWayAction();
                midwayReached = true;
            }
        }, delegate
        {
            if (EndAction != null)
                EndAction();
            image.enabled = false;
        });
    }
}
