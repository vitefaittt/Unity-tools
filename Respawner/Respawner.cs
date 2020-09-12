using UnityEngine;

public class Respawner : MonoBehaviour
{
    Collider trigger;
    public Bounds Bounds => trigger.bounds;
    float minYPosition = -5;
    public Transform player;
    public Vector3 PlayerPosition => player ? player.position : Vector3.zero;
    public static Respawner Instance { get; private set; }


    private void Awake()
    {
        trigger = GetComponent<Collider>();
        Instance = this;
    }

    private void Update()
    {
        // Respawn objects in the area if they are under the player.
        if (Respawnable.Objects != null)
            foreach (Respawnable respawnable in Respawnable.Objects)
                if (respawnable.transform.position.y < PlayerPosition.y + minYPosition)
                    Respawn(respawnable, PlayerPosition + Vector3.up * .5f);
    }

    private void OnTriggerExit(Collider other)
    {
        // Send a respawn request to an object that leaves the area.
        Respawnable respawnable = other.GetComponent<Respawnable>();
        if (!respawnable)
            respawnable = other.GetComponentInParent<Respawnable>();

        respawnable?.ReceiveRespawnRequest();
    }

    void OnDestroy()
    {
        Instance = null;
    }


    public static void Respawn(Respawnable respawnable, Vector3 respawnPosition)
    {
        // Reset the velocity and place the respawnable at the center of the room.
        if (respawnable.Rigidbody)
        {
            respawnable.Rigidbody.velocity = Vector3.zero;
            respawnable.Rigidbody.angularVelocity = Vector3.zero;
        }
        respawnable.transform.position = respawnPosition;
    }
}
