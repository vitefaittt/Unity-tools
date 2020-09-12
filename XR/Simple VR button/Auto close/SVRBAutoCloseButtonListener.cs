using System.Collections.Generic;
using UnityEngine;

public class SVRBAutoCloseButtonListener : MonoBehaviour
{
    static List<GameObject> buttons = new List<GameObject>();


    private void Awake()
    {
        buttons.Add(gameObject);
    }

    private void OnDestroy()
    {
        buttons.Remove(gameObject);
    }

    private void OnEnable()
    {
        SVRBAutoClosePointerListener.OpenPointers();
    }

    private void OnDisable()
    {
        foreach (var button in buttons)
            if (button.activeInHierarchy)
                return;
        SVRBAutoClosePointerListener.ClosePointers();
    }
}
