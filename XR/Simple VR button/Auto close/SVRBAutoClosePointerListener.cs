using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SimpleVRButtonPointer))]
public class SVRBAutoClosePointerListener : MonoBehaviour
{
    static List<GameObject> pointers = new List<GameObject>();


    private void Awake()
    {
        pointers.Add(gameObject);
    }

    private void OnDestroy()
    {
        pointers.Remove(gameObject);
    }


    public static void ClosePointers()
    {
        foreach (var pointer in pointers)
            pointer.gameObject.SetActive(false);
    }

    public static void OpenPointers()
    {
        foreach (var pointer in pointers)
            pointer.gameObject.SetActive(true);
    }
}
