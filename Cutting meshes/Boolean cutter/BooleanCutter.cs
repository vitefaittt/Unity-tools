using UnityEngine;

public class BooleanCutter : MonoBehaviour
{
    public MeshCollider cutterCollider;
    public MeshCollider targetCollider;

    [ContextMenu("Cut")]
    public void Cut()
    {
        // Create the cut meshes.
        Transform resultIn = CreateMeshRenderer(new BooleanMesh(targetCollider, cutterCollider).Intersection());
        Transform resultOut = CreateMeshRenderer(new BooleanMesh(targetCollider, cutterCollider).Difference());

        // Copy the transform of the target to the result GameObjects.
        resultIn.position = resultOut.position = targetCollider.transform.position;
        resultIn.rotation = resultOut.rotation = targetCollider.transform.rotation;

        // Destroy the previous target.
        Destroy(targetCollider.gameObject);
    }

    Transform CreateMeshRenderer(Mesh mesh)
    {
        GameObject newObject = new GameObject();
        newObject.AddComponent<MeshRenderer>().material = targetCollider.GetComponent<Renderer>().material;
        newObject.AddComponent<MeshFilter>().mesh = mesh;
        return newObject.transform;
    }
}
