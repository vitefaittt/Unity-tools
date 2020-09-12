using UnityEngine;
using UnityEngine.SceneManagement;

public class XRCenteringArea : MonoBehaviour
{
    public Vector2 size = new Vector2(3, 3);


    void Reset()
    {
        this.RenameFromType();
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
        Vector3 rigPosition = XRRigInstance.Position;
        rigPosition.x = Mathf.Clamp(rigPosition.x, -size.x * .5f, size.x * .5f);
        rigPosition.z = Mathf.Clamp(rigPosition.z, -size.y * .5f, size.y * .5f);
        XRRigInstance.Position = rigPosition;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(size.x, 0, size.y));
    }
}
