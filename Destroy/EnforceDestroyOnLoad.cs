using UnityEngine;
using UnityEngine.SceneManagement;

public class EnforceDestroyOnLoad : MonoBehaviour
{
    bool nativeSceneLoaded;


    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    private void Start()
    {
        nativeSceneLoaded = true;
    }


    void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (nativeSceneLoaded)
            Destroy(gameObject);
    }
}
