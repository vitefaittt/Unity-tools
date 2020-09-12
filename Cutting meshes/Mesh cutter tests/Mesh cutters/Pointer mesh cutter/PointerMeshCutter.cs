using UnityEngine;

public class PointerMeshCutter : MonoBehaviour
{
    public RaycastHit hit;
    public Vector3 DefaultPosition => transform.TransformPoint(Vector3.forward);
    public MeshFilter targetMesh;
    public MeshCollider targetCollider;
    public bool IsInRange { get; private set; }
    public bool IsCutting { get; private set; }


    void Update()
    {
        IsInRange = TryRaycast();
        if (IsInRange && IsCutting)
        {
            Mesh newMesh = targetMesh.mesh;
            int[] newTriangles = newMesh.triangles;

            newTriangles[hit.triangleIndex * 3] = 0;
            newTriangles[hit.triangleIndex * 3 + 1] = 0;
            newTriangles[hit.triangleIndex * 3 + 2] = 0;
            newMesh.triangles = newTriangles;
            targetMesh.mesh = newMesh;
        }
    }

    void OnDrawGizmos()
    {
        IsInRange = TryRaycast();


        if (!IsInRange)
        {
            Gizmos.DrawLine(transform.position, DefaultPosition);
            return;
        }

        // Draw a sphere at the impact and highlight the result triangle.
        Gizmos.DrawLine(transform.position, hit.point);
        Gizmos.DrawSphere(hit.point, .025f);
        Mesh mesh = Application.isPlaying ? targetMesh.mesh : targetMesh.sharedMesh;
        Vector3 p0 = targetMesh.transform.TransformPoint(mesh.vertices[mesh.triangles[hit.triangleIndex * 3 + 0]]);
        Vector3 p1 = targetMesh.transform.TransformPoint(mesh.vertices[mesh.triangles[hit.triangleIndex * 3 + 1]]);
        Vector3 p2 = targetMesh.transform.TransformPoint(mesh.vertices[mesh.triangles[hit.triangleIndex * 3 + 2]]);
        Gizmos.DrawLine(p0, p1);
        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p0);
    }


    bool TryRaycast()
    {
        if (!targetCollider)
            return false;
        return Physics.Raycast(new Ray(transform.position, transform.forward), out hit) && hit.collider == targetCollider;
    }

    public void StartCut()
    {
        IsCutting = true;
    }

    public void StopCut()
    {
        IsCutting = false;
    }
}
