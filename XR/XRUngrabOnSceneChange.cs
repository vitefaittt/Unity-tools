using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class XRUngrabOnSceneChange : MonoBehaviour
{
    [SerializeField]
    XRBaseInteractor interactor;


    private void Reset()
    {
        interactor = GetComponent<XRBaseInteractor>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoad;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoad;
    }


    void HandleSceneLoad(Scene scene, LoadSceneMode mode)
    {
        interactor?.DropObjectNow();
    }
}
