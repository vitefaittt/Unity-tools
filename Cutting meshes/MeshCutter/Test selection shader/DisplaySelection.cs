using UnityEngine;

public class DisplaySelection : MonoBehaviour {

    [SerializeField]
    GameObject objToEdit;
    Mesh objMesh;
    Vector3[] objVertices;
    [SerializeField]
    Color initHighlightColor = Color.white;
    Color invertedHighlightColor;

    Collider col;
    private void Awake()
    {
        col = GetComponent<Collider>();
        objMesh = objToEdit.GetComponent<MeshFilter>().mesh;
        objVertices = objMesh.vertices;
        invertedHighlightColor = new Color(1 - initHighlightColor.r, 1 - initHighlightColor.g, 1 - initHighlightColor.b);
    }


    private void Update()
    {
        Color[] colors = new Color[objVertices.Length];
        for (int i = 0; i < objVertices.Length; i++)
            colors[i] = col.bounds.Contains(objToEdit.transform.TransformPoint(objVertices[i])) ? invertedHighlightColor : Color.white;
        objMesh.colors = colors;
    }
}
