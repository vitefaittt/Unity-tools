using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class QRCodeReaderOutput : MonoBehaviour
{
    RawImage image;
    Texture WebcamTexture { get { return QRCodeReader.Instance.WebcamTexture; } }

    private void Awake()
    {
        image = GetComponent<RawImage>();
    }

    private IEnumerator Start()
    {
        if (!QRCodeReader.Instance)
            yield break;
        // Get the webcam texture.
        while (!WebcamTexture)
            yield return null;
            image.material = new Material(Shader.Find("Unlit/Texture"));
        image.material.mainTexture = WebcamTexture;
    }
}
