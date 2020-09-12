using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeshSlicer : MonoBehaviour
{
    [SerializeField]
    BoxCollider collider;
    [SerializeField]
    MeshFilter meshToCut;


    private void Reset()
    {
        collider = GetComponent<BoxCollider>();
    }


    [ContextMenu("Cut")]
    public void Cut()
    {
        // Get original mesh.
        Mesh originalMesh = meshToCut.mesh;
        originalMesh.RecalculateBounds();
        List<MeshPart> parts = new List<MeshPart>();

        // Get the main part of the original mesh.
        MeshPart mainPart = new MeshPart(meshToCut.transform)
        {
            uVs = originalMesh.uv.ToList(),
            vertices = originalMesh.vertices.ToList(),
            normals = originalMesh.normals.ToList(),
            triangles = new List<List<int>> { new int[originalMesh.subMeshCount].ToList() },
            bounds = originalMesh.bounds
        };
        for (int i = 0; i < originalMesh.subMeshCount; i++)
            mainPart.triangles[i] = originalMesh.GetTriangles(i).ToList();

        // Send our part to GenerateMesh two times, for each side of the plane.
        parts.Add(GenerateMesh(mainPart, collider.bounds, true));
        parts.Add(GenerateMesh(mainPart, collider.bounds, false));

        // Setup a gameobject with our result and apply a force to its parts.
        for (int i = 0; i < parts.Count; i++)
            parts[i].ToGameobject(meshToCut.gameObject);

        Destroy(meshToCut.gameObject);
    }

    MeshPart GenerateMesh(MeshPart mesh, Bounds bounds, bool inside)
    {
        MeshPart result = new MeshPart(mesh.sourceTransform);

        // Create new mesh with the triangles that are inside of the bounds.
        for (int i = 0; i < mesh.triangles.Count; i++)
        {
            int[] triangle = mesh.triangles[i].ToArray();

            for (int j = 0; j < triangle.Length; j += 3)
            {
                int amountOfPointsInside = 0;
                if (bounds.Contains(mesh.sourceTransform.TransformPoint(mesh.vertices[triangle[j]])))
                    amountOfPointsInside++;
                if (bounds.Contains(mesh.sourceTransform.TransformPoint(mesh.vertices[triangle[j + 1]])))
                    amountOfPointsInside++;
                if (bounds.Contains(mesh.sourceTransform.TransformPoint(mesh.vertices[triangle[j + 2]])))
                    amountOfPointsInside++;

                // Most of the points are inside of the bounds, keep this triangle.
                if (amountOfPointsInside >= 2 == inside)
                    result.AddTriangle(i,
                        mesh.vertices[triangle[j]], mesh.vertices[triangle[j + 1]], mesh.vertices[triangle[j + 2]],
                        mesh.normals[triangle[j]], mesh.normals[triangle[j + 1]], mesh.normals[triangle[j + 2]],
                        mesh.uVs[triangle[j]], mesh.uVs[triangle[j + 1]], mesh.uVs[triangle[j + 2]]);
            }
        }

        return result;
    }

    public class MeshPart
    {
        public Transform sourceTransform;
        public List<Vector3> vertices = new List<Vector3>();
        public List<Vector3> normals = new List<Vector3>();
        public List<List<int>> triangles = new List<List<int>>();
        public List<Vector2> uVs = new List<Vector2>();
        public Bounds bounds = new Bounds();

        public MeshPart(Transform sourceTransform)
        {
            this.sourceTransform = sourceTransform;
        }

        public void AddTriangle(int submesh, Vector3 v1, Vector3 v2, Vector3 v3, Vector3 normal1, Vector3 normal2, Vector3 normal3, Vector2 uv1, Vector2 uv2, Vector2 uv3)
        {
            if (triangles.Count - 1 < submesh)
                triangles.Add(new List<int>());

            // Add vertices, triangles, normals, UVs.
            triangles[submesh].Add(vertices.Count);
            vertices.Add(v1);
            triangles[submesh].Add(vertices.Count);
            vertices.Add(v2);
            triangles[submesh].Add(vertices.Count);
            vertices.Add(v3);
            normals.Add(normal1);
            normals.Add(normal2);
            normals.Add(normal3);
            uVs.Add(uv1);
            uVs.Add(uv2);
            uVs.Add(uv3);

            // Recalculate bounds.
            bounds.min = Vector3.Min(bounds.min, v1);
            bounds.min = Vector3.Min(bounds.min, v2);
            bounds.min = Vector3.Min(bounds.min, v3);
            bounds.max = Vector3.Min(bounds.max, v1);
            bounds.max = Vector3.Min(bounds.max, v2);
            bounds.max = Vector3.Min(bounds.max, v3);
        }

        public void ToGameobject(GameObject original)
        {
            GameObject gameObject = new GameObject(original.name);
            gameObject.transform.position = original.transform.position;
            gameObject.transform.rotation = original.transform.rotation;
            gameObject.transform.localScale = original.transform.localScale;

            Mesh mesh = new Mesh();
            mesh.name = original.GetComponent<MeshFilter>().mesh.name;

            mesh.vertices = vertices.ToArray();
            mesh.normals = normals.ToArray();
            mesh.uv = uVs.ToArray();
            for (int i = 0; i < triangles.Count; i++)
                mesh.SetTriangles(triangles[i], i, true);
            bounds = mesh.bounds;

            MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
            renderer.materials = original.GetComponent<MeshRenderer>().materials;

            MeshFilter filter = gameObject.AddComponent<MeshFilter>();
            filter.mesh = mesh;

            MeshCollider collider = gameObject.AddComponent<MeshCollider>();
            collider.convex = true;
        }
    }
}

