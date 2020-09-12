using UnityEngine;

public class BooleanMesh
{
    MeshCollider target;
    Triangulation targetTriangulation;
    MeshCollider cutter;
    Triangulation cutterTriangulation;

    float distance, customDistance;

    readonly bool legacyKeepCutterGeometry;


    public BooleanMesh(MeshCollider target, MeshCollider cutter)
    {
        this.target = target;
        this.cutter = cutter;
        cutterTriangulation = new Triangulation(cutter);
        targetTriangulation = new Triangulation(target);
        distance = 100;
    }


    public Mesh Intersection()
    {
        GetIntersections();
        ClearVertices(targetTriangulation, Vertex.VertexType.OutOrLocal);
        ClearVertices(cutterTriangulation, Vertex.VertexType.OutOrLocal);
        return GetTriangulationMesh();
    }

    public Mesh Difference()
    {
        GetIntersections();
        ClearVertices(targetTriangulation, Vertex.VertexType.In);
        ClearVertices(cutterTriangulation, Vertex.VertexType.OutOrLocal);
        cutterTriangulation.InvertNormals();
        return GetTriangulationMesh();
    }

    void GetIntersections()
    {
        // Update world position vertices.
        targetTriangulation.UpdateWorldPosition(target.transform);
        cutterTriangulation.UpdateWorldPosition(cutter.transform);

        // Intersection data.
        IntersectionData targetToCutterIntersection = new IntersectionData(targetTriangulation, cutterTriangulation, cutter);
        IntersectionData cutterToTargetIntersection = new IntersectionData(cutterTriangulation, targetTriangulation, target);

        // In/out points.
        SetupTargetVerticesType(targetToCutterIntersection);
        SetupTargetVerticesType(cutterToTargetIntersection);

        // Intersections.
        IntersectionsAToB(targetToCutterIntersection);
        IntersectionsAToB(cutterToTargetIntersection);
    }

    void SetupTargetVerticesType(IntersectionData intersection)
    {
        // Get which vertices from A are in B.
        for (int i = 0; i < intersection.target.vertices.Count; i++)
            if (IsIn(intersection.cutterCollider, intersection.target.vertices[i].pos))
                intersection.target.vertices[i].type = Vertex.VertexType.In;
            else
                intersection.target.vertices[i].type = Vertex.VertexType.OutOrLocal;
    }

    #region Intersection A to B.
    void IntersectionsAToB(IntersectionData intersection)
    {
        for (int i = 0; i < intersection.target.triangles.Count; i++)
        {
            intersection.targetTriangle = i;
            RaycastIntersection(0, 1, intersection);
            RaycastIntersection(0, 2, intersection);
            RaycastIntersection(1, 2, intersection);
        }
    }

    void RaycastIntersection(int originVertice, int toVertice, IntersectionData intersection)
    {
        intersection.ray1.origin = intersection.target.vertices[intersection.target.triangles[intersection.targetTriangle].verticesIndexes[originVertice]].pos;
        intersection.ray2.origin = intersection.target.vertices[intersection.target.triangles[intersection.targetTriangle].verticesIndexes[toVertice]].pos;
        intersection.ray1.direction = (intersection.ray2.origin - intersection.ray1.origin).normalized;
        intersection.ray2.direction = (intersection.ray1.origin - intersection.ray2.origin).normalized;

        intersection.customDistance = Vector3.Distance(intersection.ray1.origin, intersection.ray2.origin);

        if (intersection.target.vertices[intersection.target.triangles[intersection.targetTriangle].verticesIndexes[originVertice]].type == Vertex.VertexType.OutOrLocal)
            if (intersection.cutterCollider.Raycast(intersection.ray1, out intersection.hit, intersection.customDistance))
                SetIntersectionPointOnIntersection(intersection);
        if (intersection.target.vertices[intersection.target.triangles[intersection.targetTriangle].verticesIndexes[toVertice]].type == Vertex.VertexType.OutOrLocal)
            if (intersection.cutterCollider.Raycast(intersection.ray2, out intersection.hit, intersection.customDistance))
                SetIntersectionPointOnIntersection(intersection);
    }

    void SetIntersectionPointOnIntersection(IntersectionData intersection)
    {
        intersection.target.AddWorldPointOnTriangle(intersection.hit.point, intersection.targetTriangle);
        intersection.cutter.AddWorldPointOnTriangle(intersection.hit);
    }
    #endregion

    void ClearVertices(Triangulation triangulation, Vertex.VertexType vertexType)
    {
        for (int i = triangulation.triangles.Count - 1; i > -1; i--)
        {
            for (int w = triangulation.triangles[i].verticesIndexes.Count - 1; w > -1; w--)
                if (triangulation.vertices[triangulation.triangles[i].verticesIndexes[w]].type == vertexType)
                    triangulation.triangles[i].verticesIndexes.RemoveAt(w);

            if (triangulation.triangles[i].verticesIndexes.Count < 3)
                triangulation.triangles.RemoveAt(i);
        }
    }

    void RecalculateTriangles(Vector3[] vertices, Vector3[] normals, int[] triangles)
    {
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int vertex1Index = triangles[i];
            int vertex2Index = triangles[i + 1];
            int vertex3Index = triangles[i + 2];

            Vector3 vertex1Position = vertices[vertex1Index];
            Vector3 vertex2Position = vertices[vertex2Index];
            Vector3 vertex3Position = vertices[vertex3Index];

            if (Vector3.Dot(normals[vertex1Index] + normals[vertex2Index] + normals[vertex3Index], Vector3.Cross(vertex2Position - vertex1Position, vertex3Position - vertex1Position)) < 0)
            {
                triangles[i + 2] = vertex1Index;
                triangles[i] = vertex3Index;
            }
        }
    }

    Mesh GetTriangulationMesh()
    {
        targetTriangulation.Calculate();
        cutterTriangulation.Calculate();

        Mesh mesh = new Mesh
        {
            subMeshCount = 2
        };

        int cutterTriangleCount = targetTriangulation.triangles.Count;
        int targetTriangleCount = cutterTriangulation.triangles.Count;

        int[] verticesA = new int[cutterTriangleCount * 3];
        int[] verticesB = new int[targetTriangleCount * 3];

        targetTriangulation.AddTriangles(cutterTriangulation.vertices.ToArray(), cutterTriangulation.triangles.ToArray());
        targetTriangulation.UpdateLocalPosition(target.transform);

        Vector3[] vertices = new Vector3[targetTriangulation.vertices.Count];
        Vector3[] normals = new Vector3[targetTriangulation.vertices.Count];
        Vector2[] uv = new Vector2[targetTriangulation.vertices.Count];

        for (int i = 0; i < targetTriangulation.vertices.Count; i++)
        {
            vertices[i] = targetTriangulation.vertices[i].localPos;
            normals[i] = targetTriangulation.vertices[i].normal.normalized;
            uv[i] = targetTriangulation.vertices[i].uv;
        }

        for (int i = 0; i < cutterTriangleCount; i++)
        {
            verticesA[i * 3] = targetTriangulation.triangles[i].verticesIndexes[0];
            verticesA[i * 3 + 1] = targetTriangulation.triangles[i].verticesIndexes[1];
            verticesA[i * 3 + 2] = targetTriangulation.triangles[i].verticesIndexes[2];
        }

        for (int i = 0; i < targetTriangleCount; i++)
        {
            verticesB[i * 3] = targetTriangulation.triangles[cutterTriangleCount + i].verticesIndexes[0];
            verticesB[i * 3 + 1] = targetTriangulation.triangles[cutterTriangleCount + i].verticesIndexes[1];
            verticesB[i * 3 + 2] = targetTriangulation.triangles[cutterTriangleCount + i].verticesIndexes[2];
        }

        RecalculateTriangles(vertices, normals, verticesA);
        if (legacyKeepCutterGeometry)
            RecalculateTriangles(vertices, normals, verticesB);

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uv;
        mesh.SetTriangles(verticesA, 0);
        if (legacyKeepCutterGeometry)
            mesh.SetTriangles(verticesB, 1);

        return mesh;
    }

    #region Concave hull & IsIn.
    RaycastHit leftHit;
    RaycastHit rightHit;
    RaycastHit upHit;
    RaycastHit downHit;
    RaycastHit forwardHit;
    RaycastHit backHit;
    RaycastHit tempHit;

    Ray leftRay = new Ray(Vector3.zero, -Vector3.left);
    Ray rightRay = new Ray(Vector3.zero, -Vector3.right);
    Ray upRay = new Ray(Vector3.zero, -Vector3.up);
    Ray downRay = new Ray(Vector3.zero, -Vector3.down);
    Ray forwardRay = new Ray(Vector3.zero, -Vector3.forward);
    Ray backRay = new Ray(Vector3.zero, -Vector3.back);
    Ray tempRay = new Ray();

    bool IsIn(MeshCollider meshCollider, Vector3 position)
    {
        leftRay.origin = -leftRay.direction * distance + position;
        rightRay.origin = -rightRay.direction * distance + position;
        upRay.origin = -upRay.direction * distance + position;
        downRay.origin = -downRay.direction * distance + position;
        forwardRay.origin = -forwardRay.direction * distance + position;
        backRay.origin = -backRay.direction * distance + position;

        return meshCollider.Raycast(leftRay, out leftHit, distance) &&
            meshCollider.Raycast(rightRay, out rightHit, distance) &&
            meshCollider.Raycast(upRay, out upHit, distance) &&
            meshCollider.Raycast(downRay, out downHit, distance) &&
            meshCollider.Raycast(forwardRay, out forwardHit, distance) &&
            meshCollider.Raycast(backRay, out backHit, distance) &&
            !ConcaveHull(meshCollider, position, rightRay, rightHit) &&
            !ConcaveHull(meshCollider, position, leftRay, leftHit) &&
            !ConcaveHull(meshCollider, position, upRay, upHit) &&
            !ConcaveHull(meshCollider, position, downRay, downHit) &&
            !ConcaveHull(meshCollider, position, forwardRay, forwardHit) &&
            !ConcaveHull(meshCollider, position, backRay, backHit);
    }

    bool ConcaveHull(MeshCollider meshCollider, Vector3 position, Ray ray, RaycastHit hit)
    {
        tempRay.origin = position;
        tempRay.direction = -ray.direction;
        customDistance = distance - hit.distance;

        while (meshCollider.Raycast(tempRay, out tempHit, customDistance))
        {
            if (tempHit.triangleIndex == hit.triangleIndex)
                break;

            ray.origin = -ray.direction * customDistance + position;

            if (!meshCollider.Raycast(ray, out hit, customDistance))
                return true;

            if (tempHit.triangleIndex == hit.triangleIndex)
                break;

            customDistance -= hit.distance;
        }

        return false;
    }
    #endregion
}

class IntersectionData
{
    public Triangulation target, cutter;
    public MeshCollider cutterCollider;
    public int targetTriangle;
    public float customDistance;
    public Ray ray1 = new Ray();
    public Ray ray2 = new Ray();
    public RaycastHit hit = new RaycastHit();

    public IntersectionData(Triangulation target, Triangulation cutter, MeshCollider cutterCollider)
    {
        this.target = target;
        this.cutter = cutter;
        this.cutterCollider = cutterCollider;
    }
}