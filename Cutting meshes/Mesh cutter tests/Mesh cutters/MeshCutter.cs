using UnityEngine;

public abstract class MeshCutter : MonoBehaviour
{
    [SerializeField]
    protected MeshFilter targetMesh;
    public MeshFilter TargetMesh
    {
        get { return targetMesh; }
        set
        {
            targetMesh = value;
            ComputeMesh();
        }
    }
    protected Vector3[] vertices;
    protected Vector3[] worldVertices;
    protected int[] triangles;
    public bool IsCutting { get; protected set; }

    void ComputeMesh()
    {
        vertices = targetMesh.mesh.vertices;
        worldVertices = new Vector3[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
            worldVertices[i] = targetMesh.transform.TransformPoint(vertices[i]);
        triangles = targetMesh.mesh.triangles;
    }

    public abstract void StartCut();
    public abstract void StopCut();
}
