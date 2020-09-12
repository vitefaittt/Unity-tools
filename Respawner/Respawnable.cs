using System.Collections.Generic;
using UnityEngine;

public class Respawnable : MonoBehaviour
{
    public static List<Respawnable> Objects { get; private set; } = new List<Respawnable>();

    Rigidbody savedRigidbody;
    public Rigidbody Rigidbody {
        get {
            if (!savedRigidbody)
                savedRigidbody = GetComponent<Rigidbody>();
            return savedRigidbody;
        }
    }
    [HideInInspector]
    public Vector3 plannedRespawnPosition = new Vector3(0, .5f);
    public System.Func<bool> RespawnCondition;


    private void Awake()
    {
        // Add this object to the existing respawnables.
        if (Objects == null)
            Objects = new List<Respawnable>();
        Objects.Add(this);
    }

    private void OnDestroy()
    {
        Objects.Remove(this);
    }


    public void ReceiveRespawnRequest()
    {
        // Get the planned respawn position and respawn to it if it is allowed to.
        Vector3 directionToCenter = (transform.position - Respawner.Instance.PlayerPosition).normalized;
        Vector3 respawnPosition = transform.position - directionToCenter * .5f;
        plannedRespawnPosition = respawnPosition;
        if (RespawnCondition == null || RespawnCondition())
            Respawner.Respawn(this, respawnPosition);
    }
}
