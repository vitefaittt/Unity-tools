using System.Collections.Generic;
using UnityEngine;

public class Triangulation
{
    public List<Vertex> vertices;
    public List<Polygon> triangles;
    public float lowerAngle;


    public Triangulation()
    {        vertices = new List<Vertex>();
        triangles = new List<Polygon>();
        lowerAngle = 1;
    }

    public Triangulation(MeshCollider meshCollider)
    {
        // Setup our vertices and triangles from the meshCollider's mesh.
        lowerAngle = 1;
        vertices = new List<Vertex>();
        for (int i = 0; i < meshCollider.sharedMesh.vertices.Length; i++)
            vertices.Add(new Vertex(meshCollider.sharedMesh.vertices[i], 0, meshCollider.sharedMesh.normals[i], meshCollider.sharedMesh.uv[i]));
        triangles = new List<Polygon>();
        for (int i = 0; i < meshCollider.sharedMesh.triangles.Length; i += 3)
            triangles.Add(new Polygon(new int[3] { meshCollider.sharedMesh.triangles[i], meshCollider.sharedMesh.triangles[i + 1], meshCollider.sharedMesh.triangles[i + 2] }));
    }


    #region Update position.
    public void UpdateWorldPosition(Transform matrix)
    {
        for (int i = 0; i < vertices.Count; i++)
            vertices[i].pos = matrix.localToWorldMatrix.MultiplyPoint3x4(vertices[i].localPos);
    }

    public void UpdateLocalPosition(Transform matrix)
    {
        for (int i = 0; i < vertices.Count; i++)
            vertices[i].localPos = matrix.worldToLocalMatrix.MultiplyPoint3x4(vertices[i].pos);
    }
    #endregion

    public void Calculate()
    {        if (vertices.Count <= 0)
            return;

        float circumsphereRadius;
        Vector3 a, b, c, ac, ab, abXac, toCircumsphereCenter, ccs;
        bool allIntersections;
        List<int[]> combination = new List<int[]>();

        for (int triIndex = triangles.Count - 1; triIndex > -1; triIndex--)
        {            if (triangles[triIndex].verticesIndexes.Count <= 3)
                continue;

            allIntersections = true;

            // Delete duplicate.
            for (int i = triangles[triIndex].verticesIndexes.Count - 1; i > 0; i--)
                for (int w = 0; w < i; w++)
                {
                    if (allIntersections && vertices[triangles[triIndex].verticesIndexes[i]].type != Vertex.VertexType.Intersection)
                        allIntersections = false;
                    if (vertices[triangles[triIndex].verticesIndexes[i]].pos == vertices[triangles[triIndex].verticesIndexes[w]].pos)
                        triangles[triIndex].verticesIndexes.RemoveAt(i); break;
                }

            if (triangles[triIndex].verticesIndexes.Count <= 3)
                continue;

            // All combinations without repetition, some vertice of different type.
            for (int i = 0; i < triangles[triIndex].verticesIndexes.Count - 2; i++)
                for (int w = i + 1; w < triangles[triIndex].verticesIndexes.Count - 1; w++)
                    for (int x = w + 1; x < triangles[triIndex].verticesIndexes.Count; x++)
                    {
                        // Continue if same type.
                        if (!allIntersections)
                            if (vertices[triangles[triIndex].verticesIndexes[i]].type == vertices[triangles[triIndex].verticesIndexes[w]].type && vertices[triangles[triIndex].verticesIndexes[i]].type == vertices[triangles[triIndex].verticesIndexes[x]].type)
                                continue;
                        combination.Add(new int[3] { triangles[triIndex].verticesIndexes[i], triangles[triIndex].verticesIndexes[w], triangles[triIndex].verticesIndexes[x] });
                    }

            // Delaunay condition.
            for (int i = combination.Count - 1; i > -1; i--)
            {
                // Points.
                a = vertices[combination[i][0]].pos;
                b = vertices[combination[i][1]].pos;
                c = vertices[combination[i][2]].pos;

                // Circumcenter 3D points.
                // http://gamedev.stackexchange.com/questions/60630/how-do-i-find-the-circumcenter-of-a-triangle-in-3d
                ac = c - a;
                ab = b - a;
                abXac = Vector3.Cross(ab, ac);

                // This is the vector from a TO the circumsphere center.
                toCircumsphereCenter = (Vector3.Cross(abXac, ab) * ac.sqrMagnitude + Vector3.Cross(ac, abXac) * ab.sqrMagnitude) / (2f * abXac.sqrMagnitude);

                // The 3 space coords of the circumsphere center then:
                ccs = a + toCircumsphereCenter; // now this is the actual 3space location
                                                // The three vertices A, B, C of the triangle ABC are the same distance from the circumcenter ccs.
                circumsphereRadius = toCircumsphereCenter.magnitude;

                // As defined by the Delaunay condition, circumcircle is empty if it contains no other vertices besides the three that define.
                for (int w = 0; w < triangles[triIndex].verticesIndexes.Count; w++)
                    if (triangles[triIndex].verticesIndexes[w] != combination[i][0] && triangles[triIndex].verticesIndexes[w] != combination[i][1] && triangles[triIndex].verticesIndexes[w] != combination[i][2])
                        if (Vector3.Distance(vertices[triangles[triIndex].verticesIndexes[w]].pos, ccs) <= circumsphereRadius)
                        {
                            // If it's not empty, remove.
                            combination.RemoveAt(i);
                            break;
                        }
            }

            if (combination.Count > 0)
            {
                triangles.RemoveAt(triIndex);
                for (int i = 0; i < combination.Count; i++)
                    triangles.Add(new Polygon(combination[i]));
            }
            combination.Clear();
        }
    }

    bool PositionExistsOnTriangle(Vector3 worldPosition, int triangleIndex)
    {        for (int i = 0; i < triangles[triangleIndex].verticesIndexes.Count; i++)
            if (vertices[triangles[triangleIndex].verticesIndexes[i]].pos == worldPosition)
                return true;
        return false;
    }

    #region Add world point on triangle.
    public void AddWorldPointOnTriangle(RaycastHit hit, int triangleIndex) { AddWorldPointOnTriangle(hit.point, triangleIndex); }
    public void AddWorldPointOnTriangle(Vector3 pos, int triangleIndex)
    {        if (triangleIndex < 0 || triangleIndex >= triangles.Count)
            return;

        if (!PositionExistsOnTriangle(pos, triangleIndex))
        {            vertices.Add(new Vertex(pos, Vertex.VertexType.Intersection, NormalCoords(triangleIndex), UVCoords(pos, triangleIndex)));
            triangles[triangleIndex].verticesIndexes.Add(vertices.Count - 1);
        }
    }
    public void AddWorldPointOnTriangle(RaycastHit hit)
    {        if (!PositionExistsOnTriangle(hit.point, hit.triangleIndex))
        {            vertices.Add(new Vertex(hit.point, Vertex.VertexType.Intersection, hit.normal, hit.textureCoord));
            triangles[hit.triangleIndex].verticesIndexes.Add(vertices.Count - 1);
        }
    }
    #endregion

    public void AddTriangles(Vertex[] vertices, Polygon[] polygons)
    {        int head = this.vertices.Count;
        this.vertices.AddRange(vertices);
        for (int i = 0; i < polygons.Length; i++)
            for (int w = 0; w < polygons[i].verticesIndexes.Count; w++)
                polygons[i].verticesIndexes[w] += head;

        triangles.AddRange(polygons);
    }

    Vector3 NormalCoords(int onTriangle)
    {        Vector3 a, b, c;

        a = vertices[triangles[onTriangle].verticesIndexes[0]].localPos;
        b = vertices[triangles[onTriangle].verticesIndexes[1]].localPos;
        c = vertices[triangles[onTriangle].verticesIndexes[2]].localPos;

        b = b - a;
        c = c - a;

        return Vector3.Cross(b, c).normalized;
    }

    public void InvertNormals()
    {
        for (int i = 0; i < vertices.Count; i++)
            vertices[i].normal *= -1f;
    }

    Vector2 UVCoords(Vector3 point, int onTriangle)
    {        // http://answers.unity3d.com/questions/383804/calculate-uv-coordinates-of-3d-point-on-plane-of-m.html
        // ... interpolate (extrapolate?) points outside the triangle, a more general approach must be used: the "sign" of each
        // area must be taken into account, which produces correct results for points inside or outside the triangle. In order 
        // to calculate the area "signs", we can use (guess what?) dot products - like this:


        // Triangle points.
        Vector3 pos1 = vertices[triangles[onTriangle].verticesIndexes[0]].pos;
        Vector3 pos2 = vertices[triangles[onTriangle].verticesIndexes[1]].pos;
        Vector3 pos3 = vertices[triangles[onTriangle].verticesIndexes[2]].pos;

        // Calculate vectors from point f to vertices p1, p2 and p3.
        Vector3 f1 = pos1 - point;
        Vector3 f2 = pos2 - point;
        Vector3 f3 = pos3 - point;

        // Calculate the areas (parameters order is essential in this case).
        Vector3 va = Vector3.Cross(pos1 - pos2, pos1 - pos3); // main triangle cross product
        Vector3 va1 = Vector3.Cross(f2, f3); // p1's triangle cross product
        Vector3 va2 = Vector3.Cross(f3, f1); // p2's triangle cross product
        Vector3 va3 = Vector3.Cross(f1, f2); // p3's triangle cross product
        float area = va.magnitude; // main triangle area

        // Calculate barycentric coordinates with sign.
        float a1 = va1.magnitude / area * Mathf.Sign(Vector3.Dot(va, va1));
        float a2 = va2.magnitude / area * Mathf.Sign(Vector3.Dot(va, va2));
        float a3 = va3.magnitude / area * Mathf.Sign(Vector3.Dot(va, va3));

        // Find the uv corresponding to point f (uv1/uv2/uv3 are associated to p1/p2/p3).
        Vector2 uv1 = vertices[triangles[onTriangle].verticesIndexes[0]].uv;
        Vector2 uv2 = vertices[triangles[onTriangle].verticesIndexes[1]].uv;
        Vector2 uv3 = vertices[triangles[onTriangle].verticesIndexes[2]].uv;

        return uv1 * a1 + uv2 * a2 + uv3 * a3;
    }

    public Triangulation Clone()
    {        Triangulation triangulation = new Triangulation();
        for (int i = 0; i < vertices.Count; i++)
            triangulation.vertices.Add(vertices[i].Clone());
        for (int i = 0; i < triangles.Count; i++)
            triangulation.triangles.Add(triangles[i].Clone());
        return triangulation;
    }
}

public class Vertex
{
    // Mesh-Local position.
    public Vector3 localPos;
    // World position.
    public Vector3 pos;
    public Vector3 normal;
    public Vector2 uv;
    // -1: in, 0: out/local, 1: intersection.
    public enum VertexType { In = -1, OutOrLocal = 0, Intersection = 1 }
    public VertexType type;

    public Vertex(Vector3 pos, VertexType type, Vector3 normal, Vector2 uv)
    {
        this.type = type;
        if (type == VertexType.OutOrLocal)
        {
            // Local position.
            localPos = pos;
            this.pos = new Vector3();
        }
        else
        {
            // World position.
            this.pos = pos;
            localPos = new Vector3();
        }
        this.normal = normal;
        this.uv = uv;
    }

    public Vertex(Vector3 localPos, Vector3 pos, VertexType type, Vector3 normal, Vector2 uv)
    {
        this.localPos = localPos;
        this.pos = pos;
        this.normal = normal;
        this.uv = uv;
        this.type = type;
    }

    public Vertex Clone()
    {
        return new Vertex(localPos, pos, type, normal, uv);
    }
}

public class Polygon
{
    public List<int> verticesIndexes = new List<int>();

    public Polygon(int[] indexVertices)
    {
        this.verticesIndexes.AddRange(indexVertices);
    }

    public Polygon Clone()
    {
        int[] index = new int[verticesIndexes.Count];
        for (int i = 0; i < verticesIndexes.Count; i++)
            index[i] = verticesIndexes[i];
        Polygon polygon = new Polygon(index);
        return polygon;
    }
}
