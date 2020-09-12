using System.Collections.Generic;
using UnityEngine;
using System.Threading;


public class MeshCutterNotAsync : MonoBehaviour {


    public GameObject TestInputMesh;
    GameObject m_copyMesh;

    private Bounds m_boundings;
    public Renderer m_box;

    List<Vector3> m_currentVertexSelection;
    Color[] m_currentVertexColorsSelection;
    List<int> m_currentVertexIndexSelection;
    int m_minIndexOffset;
    int[] m_currentSelectedTriangles;

    List<int> m_trianglesNewCut;
    List<int> m_trianglesToKeep;

    Dictionary<int, int> m_SelectedIndexes;

    Mesh m_inputMeshToCut;
    Mesh m_outputCutMesh;
    Vector3[] m_inputVertices;
    Color[] m_inputColors;
    int[] m_inputTriangles;

    bool m_checkThreadRunning = false;


    void Start()
    {
        m_trianglesNewCut = new List<int>();
        m_trianglesToKeep = new List<int>();

        m_SelectedIndexes = new Dictionary<int, int>();
    }


    void Update()
    {
        m_boundings = m_box.bounds;
        if (Input.GetKeyUp(KeyCode.C))
        {
            CutAsync(TestInputMesh.GetComponent<MeshFilter>().mesh, m_boundings);

        }

        if (m_checkThreadRunning)
        {
            if (!_threadRunning)
            {
                m_checkThreadRunning = false;
                //newGameMeshFromCut("Test Cut Async");
                SetNewTrianglesToCut();
            }
        }
    }


    void Cut(Mesh inputMesh, Bounds bounds)
    {
        m_currentVertexSelection = SelectVertices(inputMesh, bounds);
        SelectTriangles(inputMesh, m_currentVertexIndexSelection);

        GameObject newCut = newGameMeshFromCut("Test Cut");
    }
    

    void CutAsync(Mesh inputMesh, Bounds bounds)
    {
        // Begin our heavy work on a new thread.
        //m_inputMeshToCut = inputMesh;
        m_inputVertices = inputMesh.vertices;
        m_inputTriangles = inputMesh.triangles;
        //m_inputColors = inputMesh.colors;


        m_copyMesh = GameObject.Instantiate(TestInputMesh);

        _thread = new Thread(AsyncCut);
        _thread.Start();
    }


    List<Vector3> SelectVertices(Mesh inputMesh, Bounds bounds)
    {
        List<Vector3> selection = new List<Vector3>();
        m_currentVertexIndexSelection = new List<int>();
        List<Color> selectedVertexColors = new List<Color>();
        bool hasColors = inputMesh.colors.Length > 0;

        m_SelectedIndexes.Clear();
        int vSelectionIndex = 0;
        for (int vIndex = 0; vIndex < inputMesh.vertices.Length; vIndex++)
        {
            if (bounds.Contains(inputMesh.vertices[vIndex]))
            {
                selection.Add(inputMesh.vertices[vIndex]);
                m_currentVertexIndexSelection.Add(vIndex);
                if (hasColors)
                    selectedVertexColors.Add(inputMesh.colors[vIndex]);
                m_SelectedIndexes.Add(vIndex, vSelectionIndex);
                vSelectionIndex++;
            }
        }

        // Store vertex colors
        m_currentVertexColorsSelection = selectedVertexColors.ToArray();



        return selection;
    }

    List<Vector3> SelectVertices(Vector3[] inputVertices, int[] inputTriangles, Color[] inputColors, Bounds bounds)
    {
        List<Vector3> selection = new List<Vector3>();
        m_currentVertexIndexSelection = new List<int>();
        List<Color> selectedVertexColors = new List<Color>();
        bool hasColors = inputColors.Length > 0;
        m_SelectedIndexes.Clear();
        int vSelectionIndex = 0;
        for (int vIndex = 0; vIndex < inputVertices.Length; vIndex++)
        {
            if (bounds.Contains(inputVertices[vIndex]))
            {
                selection.Add(inputVertices[vIndex]);
                m_currentVertexIndexSelection.Add(vIndex);
                if (hasColors)
                    selectedVertexColors.Add(inputColors[vIndex]);
                m_SelectedIndexes.Add(vIndex, vSelectionIndex);
                vSelectionIndex++;
            }
        }

        // Store vertex colors
        m_currentVertexColorsSelection = selectedVertexColors.ToArray();



        return selection;
    }

    void SelectTriangles(Mesh inputMesh, List<int> selectedVertexIndices)
    {

        m_trianglesNewCut.Clear();
        List<int> trianglesToKeep = new List<int>();

        for (int triStartIndex = 0; triStartIndex < inputMesh.triangles.Length - 3; triStartIndex += 3)
        {
            if (selectedVertexIndices.Contains(inputMesh.triangles[triStartIndex])
                && selectedVertexIndices.Contains(inputMesh.triangles[triStartIndex + 1])
                && selectedVertexIndices.Contains(inputMesh.triangles[triStartIndex + 2]))
            {
                m_trianglesNewCut.Add(m_SelectedIndexes[inputMesh.triangles[triStartIndex]]);
                m_trianglesNewCut.Add(m_SelectedIndexes[inputMesh.triangles[triStartIndex + 1]]);
                m_trianglesNewCut.Add(m_SelectedIndexes[inputMesh.triangles[triStartIndex + 2]]);
            }
            else
            {
                trianglesToKeep.Add(inputMesh.triangles[triStartIndex]);
                trianglesToKeep.Add(inputMesh.triangles[triStartIndex + 1]);
                trianglesToKeep.Add(inputMesh.triangles[triStartIndex + 2]);
            }
        }

        inputMesh.SetTriangles(trianglesToKeep, 0, true);
        TestInputMesh.GetComponent<MeshFilter>().mesh = inputMesh;

    }

    void SelectTriangles(int[] inputTriangles, List<int> selectedVertexIndices)
    {

        m_trianglesNewCut.Clear();
        m_trianglesToKeep.Clear();

        for (int triStartIndex = 0; triStartIndex < inputTriangles.Length - 3; triStartIndex += 3)
        {
            if (selectedVertexIndices.Contains(inputTriangles[triStartIndex])
                && selectedVertexIndices.Contains(inputTriangles[triStartIndex + 1])
                && selectedVertexIndices.Contains(inputTriangles[triStartIndex + 2]))
            {
                m_trianglesNewCut.Add(m_SelectedIndexes[inputTriangles[triStartIndex]]);
                m_trianglesNewCut.Add(m_SelectedIndexes[inputTriangles[triStartIndex + 1]]);
                m_trianglesNewCut.Add(m_SelectedIndexes[inputTriangles[triStartIndex + 2]]);
            }
            else
            {
                m_trianglesToKeep.Add(inputTriangles[triStartIndex]);
                m_trianglesToKeep.Add(inputTriangles[triStartIndex + 1]);
                m_trianglesToKeep.Add(inputTriangles[triStartIndex + 2]);
            }
        }



    }

    void CutAndReTriangulate(int[] inputTriangles, Vector3[] inputVertices)
    {
        m_trianglesNewCut.Clear();
        m_trianglesToKeep.Clear();

        for (int triStartIndex = 0; triStartIndex < inputTriangles.Length - 3; triStartIndex += 3)
        {
            if (m_boundings.Contains(inputVertices[inputTriangles[triStartIndex]])
                || m_boundings.Contains(inputVertices[inputTriangles[triStartIndex + 1]])
                || m_boundings.Contains(inputVertices[inputTriangles[triStartIndex + 2]]))
            {
                m_trianglesNewCut.Add(inputTriangles[triStartIndex]);
                m_trianglesNewCut.Add(inputTriangles[triStartIndex + 1]);
                m_trianglesNewCut.Add(inputTriangles[triStartIndex + 2]);
            }
            else
            {
                m_trianglesToKeep.Add(inputTriangles[triStartIndex]);
                m_trianglesToKeep.Add(inputTriangles[triStartIndex + 1]);
                m_trianglesToKeep.Add(inputTriangles[triStartIndex + 2]);
            }
        }
    }

    void SetNewTrianglesToCut()
    {
        TestInputMesh.GetComponent<MeshFilter>().mesh.SetTriangles(m_trianglesToKeep, 0, true);
        m_copyMesh.GetComponent<MeshFilter>().mesh.SetTriangles(m_trianglesNewCut, 0, true);

    }

    GameObject newGameMeshFromCut(string objectName)
    {

        m_inputMeshToCut.SetTriangles(m_trianglesToKeep, 0, true);
        TestInputMesh.GetComponent<MeshFilter>().mesh = m_inputMeshToCut;



        Mesh m = new Mesh();
        m.SetVertices(m_currentVertexSelection);
        m.SetTriangles(m_trianglesNewCut, 0, true);
        m.colors = m_currentVertexColorsSelection;

        GameObject go = new GameObject();
        MeshFilter mf = go.AddComponent<MeshFilter>();

        go.AddComponent<MeshRenderer>().material = TestInputMesh.GetComponent<MeshRenderer>().material;
        mf.mesh = m;
        go.name = objectName;

        return go;
    }


    //////////// THREADING ///////////

    bool _threadRunning;
    Thread _thread;




    void AsyncCut()
    {
        _threadRunning = true;
        bool workDone = false;
        m_checkThreadRunning = true;
        // This pattern lets us interrupt the work at a safe point if neeeded.
        while (_threadRunning && !workDone)
        {
            // Do Work...
            //m_currentVertexSelection = SelectVertices(m_inputVertices, m_inputTriangles, m_inputColors, m_boundings);
            //SelectTriangles(m_inputTriangles, m_currentVertexIndexSelection);
            CutAndReTriangulate(m_inputTriangles, m_inputVertices);
            workDone = true;
            _threadRunning = false;
        }
        _threadRunning = false;
    }

    void OnDisable()
    {
        // If the thread is still running, we should shut it down,
        // otherwise it can prevent the game from exiting correctly.
        if (_threadRunning)
        {
            // This forces the while loop in the ThreadedWork function to abort.
            _threadRunning = false;

            // This waits until the thread exits,
            // ensuring any cleanup we do after this is safe. 
            _thread.Join();
        }

        // Thread is guaranteed no longer running. Do other cleanup tasks.
    }
}
