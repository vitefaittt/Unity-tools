using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    Vector3 onClickOffset;
    bool shouldStop;
    bool MouseOutOfBounds { get { return Input.mousePosition.x < 5 || Input.mousePosition.x > (Screen.width - 5) || Input.mousePosition.y < 5 || Input.mousePosition.y > Screen.height - 5; } }
    public event System.Action OnClick, HoverStart, HoverEnd, DragStarted, DragEnded;

    [SerializeField]
    bool unparentOnDrag;
    int startSiblingIndex;
    Transform startParent;


    private void Start()
    {
        startParent = transform.parent;
        startSiblingIndex = transform.GetSiblingIndex();
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        // Call event.
        if (OnClick != null)
            OnClick();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        onClickOffset = (Vector2)transform.position - new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        if (unparentOnDrag)
            transform.parent = GetComponentInParent<Canvas>().transform;
        // Call event.
        if (DragStarted != null)
            DragStarted();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (shouldStop || MouseOutOfBounds)
            return;
        transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y) + (Vector2)onClickOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        shouldStop = false;
        if (unparentOnDrag)
        {
            transform.parent = startParent;
            transform.SetSiblingIndex(startSiblingIndex);
        }
        // Call event.
        if (DragEnded != null)
            DragEnded();
    }

    public void CancelDrag()
    {
        shouldStop = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Call event.
        if (HoverStart != null)
            HoverStart();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Call event.
        if (HoverEnd != null)
            HoverEnd();
    }
}