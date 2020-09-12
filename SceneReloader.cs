using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneReloader : MonoBehaviour
{
    void Reset()
    {
        if (GetComponent<Button>())
            GetComponent<Button>().onClick.AddPersistentEvent(this, Reload);
    }


    public void Reload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
