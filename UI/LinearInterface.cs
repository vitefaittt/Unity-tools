using UnityEngine;

public class LinearInterface : MonoBehaviour
{
    int currentChild = 0;


    void Start()
    {
        transform.HideChildren();
        SetChildActive(currentChild, true);
    }

    void Update()
    {
        // Change our current child if a child before us is now active.
        int firstActiveChild = -1;
        for (int i = 0; i < transform.childCount; i++)
            if (GetChildActive(i))
            {
                firstActiveChild = i;
                break;
            }
        if (firstActiveChild < currentChild)
        {
            SetChildActive(currentChild, false);
            currentChild = firstActiveChild;
            SetChildActive(currentChild, true);
        }

        // Change our current child if it is now inactive.
        if (!GetChildActive(currentChild))
        {
            currentChild++;
            SetChildActive(currentChild, true);
        }
    }


    void SetChildActive(int i, bool state)
    {
        transform.GetChild(i).gameObject.SetActive(state);
    }

    bool GetChildActive(int i)
    {
        return transform.GetChild(i).gameObject.activeSelf;
    }
}
