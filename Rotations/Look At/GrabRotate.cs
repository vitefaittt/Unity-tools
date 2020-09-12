using System.Collections;
using UnityEngine;

public class GrabRotate : MonoBehaviour
{
    Vector3 previousGrabPos;
    Grabber grabber;
    Transform grabPoint;
    Quaternion grabRotationOffset;
    Quaternion LookRotation { get { return Quaternion.LookRotation((grabPoint.position - transform.position).normalized, transform.up); } }


    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Drawer>() && other.GetComponentInParent<Seb_XR_Input>()
            && other.GetComponentInParent<Grabber>().Gripping && !runningGrabRoutine.IsRunning())
        {
            grabber = other.GetComponentInParent<Grabber>();
            grabPoint = other.transform;
            grabRotationOffset = Quaternion.Inverse(transform.rotation) * LookRotation;
            runningGrabRoutine = GrabRoutine();
            StartCoroutine(runningGrabRoutine);
        }
    }


    IEnumerator runningGrabRoutine;
    IEnumerator GrabRoutine()
    {
        while (grabber.GripSqueeze > 0)
        {
            transform.rotation = LookRotation * Quaternion.Inverse(grabRotationOffset);
            yield return null;
        }
        runningGrabRoutine = null;
    }
}
