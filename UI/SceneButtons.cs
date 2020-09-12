using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneButtons : MonoBehaviour
{
    public Button[] buttons;


    private void Reset()
    {
        GetButtons();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }


    void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        for (int i = 0; i < buttons.Length; i++)
            if (buttons[i])
                buttons[i].interactable = i != scene.buildIndex;
    }

    [ContextMenu("Get buttons")]
    void GetButtons()
    {
        buttons = GetComponentsInChildren<Button>();
    }

    [ContextMenu("Get all buttons")]
    void GetAllButtons()
    {
        buttons = GetComponentsInChildren<Button>(true);
    }
}
