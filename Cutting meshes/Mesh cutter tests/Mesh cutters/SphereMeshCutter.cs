using System.Threading;
using UnityEngine;

public class SphereMeshCutter : MeshCutter
{
    public Transform sphere;
    float radius;
    Vector3 spherePosition;
    bool threadWasRunning;
    bool threadIsRunning;
    bool didCut;
    public event System.Action<bool> CutFinished;


    void Update()
    {
        if (threadWasRunning)
        {
            targetMesh.mesh.triangles = triangles;
            threadIsRunning = false;
            CutFinished?.Invoke(didCut);
        }

        if (!IsCutting)
            return;

        if (threadIsRunning)
            return;

        radius = sphere.lossyScale.x * .5f;
        spherePosition = sphere.position;
        new Thread(ThreadedCut).Start();
    }


    void ThreadedCut()
    {
        threadWasRunning = threadIsRunning = true;
        didCut = false;
        for (int i = 0; i < triangles.Length; i += 3)
            if (Vector3.Distance(spherePosition, worldVertices[triangles[i]]) < radius)
            {
                didCut = true;
                triangles[i] = 0;
                triangles[i + 1] = 0;
                triangles[i + 2] = 0;
            }
        threadIsRunning = false;
    }

    public override void StartCut()
    {
        IsCutting = true;
    }

    public override void StopCut()
    {
        IsCutting = false;
    }
}
