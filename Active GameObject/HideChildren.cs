using UnityEngine;

public class HideChildren : MonoBehaviour
{
    public void Hide()
    {
        gameObject.HideChildren();
    }

    public void Show()
    {
        gameObject.ShowChildren();
    }
}
