using UnityEngine;

[RequireComponent(typeof(Respawnable))]
public class RespawnableGrabBehaviour : MonoBehaviour
{
    Respawnable respawnable;
    public bool IsGrabbed { get; private set; }
    public bool WasGrabbedOnce { get; private set; }


    void Awake()
    {
        respawnable = GetComponent<Respawnable>();

        // Can only respawn when it is not grabbed.
        respawnable.RespawnCondition += () => !IsGrabbed;
    }


    void RespawnIfOutOfBounds()
    {
        if (!Respawner.Instance)
            return;
        if (!Respawner.Instance.Bounds.Contains(transform.position))
            Respawner.Respawn(respawnable, respawnable.plannedRespawnPosition);
    }

    public void SetIsGrabbed(bool state)
    {
        // Set is grabbed.
        IsGrabbed = state;
        if (IsGrabbed)
            WasGrabbedOnce = true;

        // Respawn if ungrabbed and out of bounds.
        if(!IsGrabbed)
            RespawnIfOutOfBounds();
    }
}
