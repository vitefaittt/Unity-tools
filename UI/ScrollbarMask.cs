using UnityEngine;
using UnityEngine.UI;

public class ScrollbarMask : MonoBehaviour
{
    [SerializeField]
    ScrollRect scrollRect;
    RectTransform scrollRectTransform;
    Image[] images;
    bool shown = true;
    bool ShouldBeShown { get
        {
            return ((scrollbar.direction == Scrollbar.Direction.LeftToRight || scrollbar.direction == Scrollbar.Direction.RightToLeft) && scrollRect.content.rect.width > scrollRectTransform.rect.width) ||
                ((scrollbar.direction == Scrollbar.Direction.BottomToTop || scrollbar.direction == Scrollbar.Direction.TopToBottom) && scrollRect.content.rect.height > scrollRectTransform.rect.height);
        } }
    Scrollbar scrollbar;


    private void Reset()
    {
        scrollRect = GetComponentInParent<ScrollRect>();
    }

    private void Awake()
    {
        scrollbar = GetComponent<Scrollbar>();
    }

    private void Start()
    {
        images = GetComponentsInChildren<Image>();
        scrollRectTransform = (RectTransform)scrollRect.transform;
    }

    private void Update()
    {
        // Show when we can scroll.
        if (ShouldBeShown)
        {
            if (!shown)
                SetShown(true);
        }
        else
        {
            if (shown)
                SetShown(false);
        }
    }


    void SetShown(bool state)
    {
        foreach (Image image in images)
            image.enabled = state;
        shown = state;
    }
}
