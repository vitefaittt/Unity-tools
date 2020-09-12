using UnityEngine;

public class ScanSection_UI : MonoBehaviour {


    Camera cam;
    private void Awake()
    {
        cam = Camera.main;
    }


    private void OnEnable()
    {
        // Look at the main camera on enable.
        transform.LookAt(cam.transform.position);
    }
}
