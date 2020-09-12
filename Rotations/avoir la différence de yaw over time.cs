
// trying to get the yaw...
Vector3 flattenedPrevForward = Vector3.ProjectOnPlane(prevForward, head.up).normalized;
Vector3 flattenedForward = Vector3.ProjectOnPlane(head.forward, head.up).normalized;
Debug.Log("yaw: " +Vector3.Angle(flattenedPrevForward, flattenedForward));
        prevForward = head.forward;
        prevUp = head.up;