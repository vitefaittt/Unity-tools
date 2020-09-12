using UnityEngine;

public class PointerMeshCutter : MonoBehaviour
{
    RaycastHit hit;
    public MeshFilter targetMesh;
    public MeshCollider targetCollider;


    void Update()
    {
        if (TryRaycast())
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
        if (!TryRaycast())
            Gizmos.DrawLine(transform.position, transform.position + transform.forward);
        else
        {
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
    }


    bool TryRaycast()
    {
        if (!targetCollider)
            return false;
        return Physics.Raycast(new Ray(transform.position, transform.forward), out hit) && hit.collider == targetCollider;
    }
}
