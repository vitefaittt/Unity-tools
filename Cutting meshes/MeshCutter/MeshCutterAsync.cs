using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Linq;

/// <summary>
/// Class used to cut GameObjects vertices for rendering
/// It copies the origin mesh and changes the triangles to render only he cut
/// </summary>
public class MeshCutterAsync : MonoBehaviour {

    // Input and resulting GameObjects.
    GameObject previousGoToEdit;
    public GameObject GameObjectToEdit { get { return ScannedObject.GameObjectToEdit; } }
    public GameObject CopyMesh { get; private set; }
    private Matrix4x4 meshToWorldMatrix;

    // Renderer used to define a selection bouding box.
    public Bounds SelectionBounds { get; private set; }
    Renderer boundingBoxRend;

    // List of triangles to modify depending on the cuts.
    List<int> trianglesToCut = new List<int>();
    List<int> trianglesToKeep = new List<int>();

    // Used to check if the thread has finished.
    bool checkThreadRunning = false;

    // Thread variables.
    public Vector3[] InputVertices { get; private set; }
    int[] inputTriangles;
    bool threadRunning;
    Thread thread;

    public delegate void CutterEvent();
    public CutterEvent Cut;

    bool inputsInitialized = false;


    private void Awake()
    {
        boundingBoxRend = GetComponent<Renderer>();
    }


    void Update()
    {
        // If thread was running and now isn't, update the meshes.
        if (checkThreadRunning)
            if (!threadRunning)
            {
                checkThreadRunning = false;
                CreateCutMeshes();
            }
    }

    public void TryCutMesh(Bounds bounds)
    {
        SelectionBounds = bounds;
        StartCutAsync();
    }

    public bool InitInputs()
    {
        // Initialize input vertices and triangles.
        if (!GameObjectToEdit)
        {
            Debug.Log("No gameobject to edit");
            return false;
        }
        else
        {
            meshToWorldMatrix = GameObjectToEdit.transform.localToWorldMatrix;
            MeshFilter mf = GameObjectToEdit.GetComponent<MeshFilter>();
            if (!mf)
            {
                Debug.Log("The gameobject to edit has no mesh filter !");
                return false;
            }
            else
            {
                InputVertices = LocalToWorldMeshVertices(mf.mesh.vertices);
                inputTriangles = mf.mesh.triangles;
            }
        }
        inputsInitialized = true;
        return true;
    }

    void StartCutAsync()
    {
        if (!inputsInitialized)
            InitInputs();
        // Start the cutting thread.
        thread = new Thread(AsyncCut);
        thread.Start();
    }

    void DistributeTriangles(int[] inputTriangles, Vector3[] inputVertices)
    {
        // Redistribute the triangles into two lists.
        trianglesToCut.Clear();
        trianglesToKeep.Clear();
        for (int triStartIndex = 0; triStartIndex < inputTriangles.Length - 3; triStartIndex += 3)
        {
            if (SelectionBounds.Contains(inputVertices[inputTriangles[triStartIndex]])
                || SelectionBounds.Contains(inputVertices[inputTriangles[triStartIndex + 1]])
                || SelectionBounds.Contains(inputVertices[inputTriangles[triStartIndex + 2]]))
            {
                trianglesToCut.Add(inputTriangles[triStartIndex]);
                trianglesToCut.Add(inputTriangles[triStartIndex + 1]);
                trianglesToCut.Add(inputTriangles[triStartIndex + 2]);
            }
            else
            {
                trianglesToKeep.Add(inputTriangles[triStartIndex]);
                trianglesToKeep.Add(inputTriangles[triStartIndex + 1]);
                trianglesToKeep.Add(inputTriangles[triStartIndex + 2]);
            }
        }
    }

    void CreateCutMeshes()
    {
        // If we have cut something, setup two new meshes.
        if (trianglesToCut.Count > 0)
        {
            GameObjectToEdit.GetComponent<MeshFilter>().mesh.SetTriangles(trianglesToKeep, 0, true);
            CreateCutMesh();
        }
        else
        // Inform the user that nothing was cut
            Debug.Log("Nothing to cut buddy ;)");
    }

    void CreateCutMesh()
    {
        // Copy the current mesh in a new game object instance
        CopyMesh = Instantiate(GameObjectToEdit);
        CopyMesh.name = GameObjectToEdit.name + "_CUT";
        CopyMesh.transform.position = GameObjectToEdit.transform.position;
        CopyMesh.transform.rotation = GameObjectToEdit.transform.rotation;
        CopyMesh.transform.localScale = GameObjectToEdit.transform.localScale;

        // Set the triangles to its mesh
        CopyMesh.GetComponent<MeshFilter>().mesh.SetTriangles(trianglesToCut, 0, true);

        // Call event.
        Cut?.Invoke();

        inputsInitialized = false;
    }


    Mesh ClearBlanks(Mesh mesh)
    {
        // Remove all unnecessary vertices (caution: expensive).
        int[] triangles = mesh.triangles;
        Vector3[] vertices = mesh.vertices;
        Vector2[] uv = mesh.uv;
        Vector3[] normals = mesh.normals;
        List<Vector3> vertList = vertices.ToList();
        List<Vector2> uvList = uv.ToList();
        List<Vector3> normalsList = normals.ToList();
        List<int> trianglesList = triangles.ToList();

        int testVertex = 0;

        while (testVertex < vertList.Count)
        {
            if (trianglesList.Contains(testVertex))
                testVertex++;
            else
            {
                vertList.RemoveAt(testVertex);
                uvList.RemoveAt(testVertex);
                normalsList.RemoveAt(testVertex);

                for (int i = 0; i < trianglesList.Count; i++)
                {
                    if (trianglesList[i] > testVertex)
                        trianglesList[i]--;
                }
            }
        }

        triangles = trianglesList.ToArray();
        vertices = vertList.ToArray();
        uv = uvList.ToArray();
        normals = normalsList.ToArray();

        mesh.triangles = triangles;
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.normals = normals;
        return mesh;
    }


    #region Threading --------------
    void AsyncCut()
    {
        threadRunning = true;
        bool workDone = false;
        checkThreadRunning = true;
        // This pattern lets us interrupt the work at a safe point if neeeded.
        while (threadRunning && !workDone)
        {
            // Do Work...
            //currentVertexSelection = SelectVertices(inputVertices, inputTriangles, inputColors, boundings);
            //SelectTriangles(inputTriangles, currentVertexIndexSelection);
            DistributeTriangles(inputTriangles, InputVertices);
            workDone = true;
            threadRunning = false;
        }
        threadRunning = false;
    }

    void OnDisable()
    {
        // If the thread is still running, we should shut it down,
        // otherwise it can prevent the game from exiting correctly.
        if (threadRunning)
        {
            // This forces the while loop in the ThreadedWork function to abort.
            threadRunning = false;

            // This waits until the thread exits,
            // ensuring any cleanup we do after this is safe. 
            thread.Join();
        }

        // Thread is guaranteed no longer running. Do other cleanup tasks.
    }
    #endregion ----------------------------


    #region Utilies --------------
    /// <summary>
    /// Transforms a mesh vertex position in world position using the gameobject to edit matrix
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    Vector3 MeshToWorldVertex(Vector3 v)
    {
        return meshToWorldMatrix.MultiplyPoint3x4(v);
    }

    /// <summary>
    /// Converts the array of vertices from local to world coordinates
    /// </summary>
    /// <param name="vT"></param>
    /// <returns></returns>
    Vector3[] LocalToWorldMeshVertices(Vector3[] vT)
    {
        for (int i = 0; i < vT.Length; i++)
        {
            vT[i] = MeshToWorldVertex(vT[i]);
        }

        return vT;
    }
    #endregion ---------------------
}
