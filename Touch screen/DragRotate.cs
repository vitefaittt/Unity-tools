using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragRotate : MonoBehaviour
{
    public Transform rotatee;
    [SerializeField]
    float speed = 5;
    enum Axis { X, Y, XY}
    [SerializeField]
    Axis axis = Axis.XY;

    public static DragRotate Instance { get; private set; }


    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (!rotatee)
            return;
        if (Input.touchCount == 1 && !rotationTouchRoutine.IsRunning())
        {
            StopAllCoroutines();
            // Start rotation routine.
            rotationTouchRoutine = RotatingTouch();
            StartCoroutine(rotationTouchRoutine);
        }
    }


    IEnumerator rotationTouchRoutine;
    IEnumerator RotatingTouch()
    {
        Quaternion rotateeStartRot = rotatee.rotation;
        List<Vector2> lastFingerDeltaPos = new List<Vector2>();
        while (Input.touchCount == 1)
        {
            // While we are touching, update the rotation.
            lastFingerDeltaPos.Add(Input.GetTouch(0).deltaPosition);
            if (lastFingerDeltaPos.Count > 3)
                lastFingerDeltaPos.RemoveAt(0);
            Rotate(Average(lastFingerDeltaPos));
            yield return null;
        }
        // If nothing is touching the screen, throw, else reset the rotation, because other fingers are touching the screen.
        if (Input.touchCount < 1)
            StartCoroutine(ThrowCoolDown(Average(lastFingerDeltaPos)));
        else
            rotatee.rotation = rotateeStartRot;
        rotationTouchRoutine = null;
    }

    IEnumerator ThrowCoolDown(Vector2 velocity)
    {
        // While our velocity is higher than what we can decrease from it, decrease our velocity.
        while (velocity.magnitude > (velocity.normalized * Time.deltaTime * 200).magnitude)
        {
            Rotate(velocity);
            velocity -= velocity.normalized * Time.deltaTime * 100;
            yield return null;
        }
    }

    void Rotate(Vector2 dragVelocity)
    {
        // Rotate the rotatee on the axes on which it can rotate.
        switch (axis)
        {
            case Axis.X:
                dragVelocity = dragVelocity.SetY(0);
                break;
            case Axis.Y:
                dragVelocity = dragVelocity.SetX(0);
                break;
        }
        rotatee.Rotate(Time.deltaTime * 5 * new Vector3(dragVelocity.y, -dragVelocity.x), Space.World);
    }

    Vector2 Average(List<Vector2> values)
    {
        Vector2 result = new Vector2();
        foreach (Vector2 value in values)
            result += value;
        return result / values.Count;
    }
}
