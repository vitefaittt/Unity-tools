using UnityEngine;

public class MeshCutterManager : MonoBehaviour
{
    [SerializeField]
    MeshCutter meshCutterTemplate;
    MeshCutter meshCutter;
    bool rightHander = true;

    public MeshFilter targetMesh;

    public static MeshCutterManager Instance { get; private set; }


    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        // Stop if we are not in sync with the AppManager.
        if (AppManager.currentMode != "MeshCutter")
            return;

        // Create mesh cutter.
        if (!meshCutter)
        {
            meshCutter = Instantiate(meshCutterTemplate);
            MoveCutterToHand(rightHander);
            meshCutter.TargetMesh = targetMesh;
        }
        meshCutter.gameObject.SetActive(true);

    }

    private void OnDisable()
    {
        if (meshCutter)
            meshCutter.gameObject.SetActive(false);
    }


    public void OnTriggerDown(bool rightHander)
    {
        // Start cutting or move the cutter to the other hand.
        if (rightHander == this.rightHander)
            meshCutter.StartCut();
        else
            MoveCutterToHand(rightHander);

        // Update right hander.
        this.rightHander = rightHander;
    }

    public void OnTriggerUp(bool rightHander)
    {
        if (rightHander == this.rightHander)
            meshCutter.StopCut();
    }

    void MoveCutterToHand(bool rightHander)
    {
        // Move to the left or right default attach point.
        meshCutter.transform.parent = rightHander ? DefaultAttachPoint.RightHand : DefaultAttachPoint.LeftHand;
        meshCutter.transform.localPosition = Vector3.zero;
        meshCutter.transform.localRotation = Quaternion.identity;
    }
}
