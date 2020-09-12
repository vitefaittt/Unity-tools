using System.Linq;
using UnityEngine;

public class SimpleMeshCutter : MonoBehaviour
{
    public Collider cutter;
    public MeshFilter target;

    [ContextMenu("Cut")]
    public void Cut()
    {
        if (!Application.isPlaying)
            return;

        Mesh newMesh = target.mesh;
        Vector3[] vertices = newMesh.vertices;
        int[] newTriangles = newMesh.triangles;
        for (int i = 0; i < newTriangles.Length; i += 3)
        {
            if (cutter.bounds.Contains(target.transform.TransformPoint(vertices[newTriangles[i]])))
            {
                newTriangles[i] = 0;
                newTriangles[i + 1] = 0;
                newTriangles[i + 2] = 0;
            }
        }
        print("finished");
        newMesh.triangles = newTriangles.ToArray();
        target.mesh = newMesh;
    }
}
