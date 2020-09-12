using UnityEngine;

[RequireComponent(typeof(MeshCutterAsync))]
public class MeshCutter_CreateGrabbable : MonoBehaviour {

    [SerializeField]
    ScanSection scanSectionTemplate;
    public ScanSection NewScanSection { get; private set; }

    MeshCutterAsync cutter;
    MeshCutter_Selection cutterSelection;

    public delegate void CutterCreationEvent();
    public CutterCreationEvent Created;

    private void Awake()
    {
        // Create the collider when the cutter creates the mesh.
        cutter = GetComponent<MeshCutterAsync>();
        cutter.Cut += CreateGrabbbable;

        cutterSelection = GetComponent<MeshCutter_Selection>();
    }

    void CreateGrabbbable()
    {
        // Create a new scan section and send it our cut mesh.
        NewScanSection = Instantiate(scanSectionTemplate);
        NewScanSection.transform.position = cutter.SelectionBounds.center;
        NewScanSection.SetScannedMesh(cutter.CopyMesh);
        SetupCollider();
        // Call event.
        Created?.Invoke();
    }

    void SetupCollider()
    {
        cutter.CopyMesh.AddComponent<MeshCollider>();
    }

    void CreateColliderFromSelection()
    {
        // Create the collider from the last selection's size.
        GameObject newGo = new GameObject();
        newGo.transform.parent = NewScanSection.transform;
        newGo.transform.localPosition = Vector3.zero;
        BoxCollider newCol = cutter.CopyMesh.AddComponent<BoxCollider>();
        newCol.size = cutterSelection.LastBounds.size;
    }
}
