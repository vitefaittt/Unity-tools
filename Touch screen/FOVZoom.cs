using UnityEngine;

public class FOVZoom : MonoBehaviour
{
    [SerializeField]
    Camera cam;

    [SerializeField]
    float perspectiveZoomSpeed = 0.5f;
    [SerializeField]
    Vector2 minMaxPerspective = new Vector2(50, 170);


    private void Start()
    {
        if (!cam)
            enabled = false;
    }

    private void Update()
    {
        cam.fieldOfView += (GetTouchDelta() + Input.mouseScrollDelta.y * -40) * perspectiveZoomSpeed;
        cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minMaxPerspective.x, minMaxPerspective.y);
    }


    float GetTouchDelta()
    {
        if (Input.touchCount != 2)
            return 0;

        Touch touchZero = Input.GetTouch(0);
        Touch touchOne = Input.GetTouch(1);

        Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
        Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

        float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
        float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

        return prevTouchDeltaMag - touchDeltaMag;
    }
}
