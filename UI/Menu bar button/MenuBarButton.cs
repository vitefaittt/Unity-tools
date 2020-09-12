using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MenuBarButton : MonoBehaviour
{
    [SerializeField]
    RectTransform dropdown;
    Camera cam;


    private void Reset()
    {
        GetComponent<Button>().AddPersistentEvent(this, OnClick);
    }

    private void Start()
    {
        if (dropdown)
            Close();
        cam = CameraFromHeadPosition.Instance.GetComponentInChildren<Camera>();
    }


    public void OnClick()
    {
        if (!dropdown)
            return;
        if (!dropdown.gameObject.activeSelf)
        {
            Open();
            StartCoroutine(ListenToOutsideClick());
        }
        else
        {
            Close();
            StopAllCoroutines();
        }
    }

    void Open()
    {
        dropdown.gameObject.SetActive(true);
    }

    void Close()
    {
        dropdown.gameObject.SetActive(false);
    }

    IEnumerator ListenToOutsideClick()
    {
        while (true)
        {
            yield return null;
            if (Input.GetMouseButtonUp(0) && !RectTransformUtility.RectangleContainsScreenPoint(dropdown, Input.mousePosition, cam))
            {
                Close();
                break;
            }
        }
    }
}