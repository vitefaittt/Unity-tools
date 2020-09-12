using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    void Awake()
    {
        SetDontDestroyOnLoad();
    }


    [ContextMenu("Don't destroy on load")]
    public void SetDontDestroyOnLoad()
    {
        if (!this.GetComponentUpwards<DontDestroyOnLoad>())
            transform.parent = null;
        DontDestroyOnLoad(gameObject);
    }

    [ContextMenu("Don't destroy on load", true)]
    public bool SetDontDestroyOnLoadValidation()
    {
        return Application.isPlaying;
    }
}
