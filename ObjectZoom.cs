using UnityEngine;

public class ObjectZoom : MonoBehaviour
{
    [SerializeField]
    Camera cam;
    [SerializeField]
    float speed = 1;

    [Header("Clamping")]
    [SerializeField]
    [Tooltip("The near and far distance at which zooming will stop. Set to -1 to disable stopping.")]
    Vector2 distanceClamp = Vector3.one * -1;
    float CurrentDistance { get { return Vector3.Distance(transform.position, cam.transform.position); } }
    [SerializeField]
    bool getCloseClampAtStart, getFarClampAtStart;


    private void Start()
    {
        if (getCloseClampAtStart)
            distanceClamp = distanceClamp.SetX(CurrentDistance);
        if (getFarClampAtStart)
            distanceClamp = distanceClamp.SetY(CurrentDistance);
    }

    private void Update()
    {
        if (Input.mouseScrollDelta.y != 0 && Utilities.ScreenRaycastAllToObject(gameObject, cam))
            Zoom(Input.mouseScrollDelta.y > 0);
    }


    void Zoom(bool closer)
    {
        float increment = speed;
        if (distanceClamp.x >= 0 && closer && CurrentDistance - increment <= distanceClamp.x)
            increment = CurrentDistance - distanceClamp.x;
        if (distanceClamp.y >= 0 && !closer && CurrentDistance + increment >= distanceClamp.y)
            increment = distanceClamp.y - CurrentDistance;
        // Move the object closer to the camera.
        Vector3 direction = (transform.position - cam.transform.position).normalized;
        if (direction == Vector3.zero)
            direction = cam.transform.forward;
        transform.Translate(direction * increment * (closer ? -1 : 1), Space.World);
    }
}
