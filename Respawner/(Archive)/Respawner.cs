using UnityEngine;
using Valve.VR.InteractionSystem;

public class Respawner : MonoBehaviour {

    float minYPosition = -5;

    Collider col;
    public Bounds Bounds { get { return col.bounds; } }
    public static Respawner Instance { get; private set; }
    Vector3 PlayerPosition{ get{ return Player.instance.transform.position; } }


    private void Awake()
    {
        col = GetComponent<Collider>();
        Instance = this;
    }

    private void Update()
    {
        if (Respawnable.Objects != null)
            // Respawn objects in the play area if they are under the player.
            foreach (Respawnable respawnable in Respawnable.Objects)
                if (respawnable.transform.position.y < PlayerPosition.y + minYPosition)
                    Respawn(respawnable, PlayerPosition + Vector3.up * .5f);
    }

    private void OnTriggerExit(Collider other)
    {
        // Respawn an object that comes in contact.
        Respawnable respawnable = other.GetComponent<Respawnable>();
        if (!respawnable)
            respawnable = other.GetComponentInParent<Respawnable>();

        if (respawnable && !respawnable.WaitingForFirstGrab)
        {
            Vector3 directionToCenter = (other.transform.position - PlayerPosition).normalized;
            Vector3 respawnPosition = other.transform.position - directionToCenter * .5f;
            if (!respawnable.IsGrabbed)
                Respawn(respawnable, respawnPosition);
            else
                respawnable.plannedRespawnPosition = respawnPosition;
        }
    }

    void OnDestroy()
    {
        Instance = null;
    }


    public void Respawn(Respawnable respawnable, Vector3 respawnPosition)
    {
        // Reset the velocity and place the respawnable at the center of the room.
        respawnable.GetComponent<Rigidbody>().velocity = Vector3.zero;
        respawnable.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        respawnable.transform.position = respawnPosition;
    }
}
