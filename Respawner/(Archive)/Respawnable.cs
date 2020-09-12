using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class Respawnable : MonoBehaviour
{
    public static List<Respawnable> Objects { get; private set; }

    [SerializeField]
    bool waitForFirstGrab = false;
    public bool WaitingForFirstGrab { get; private set; }
    public bool IsGrabbed { get { return interactable.IsGrabbed(); } }
    [SerializeField]
    Interactable interactable;
    public Interactable Interactable { get { return interactable; } }
    
    public Vector3 plannedRespawnPosition = new Vector3(0, .5f);


    private void Awake()
    {
        if (Objects == null)
            Objects = new List<Respawnable>();
        Objects.Add(this);

        if (!interactable)
            interactable = GetComponent<Interactable>();

        WaitingForFirstGrab = waitForFirstGrab;
    }
        
    private void Start()
    {
        if (waitForFirstGrab)
            interactable.onAttachedToHand += delegate { WaitingForFirstGrab = false; };
        interactable.onDetachedFromHand += delegate { CheckOutOfBounds(); } ;
    }

    private void OnDestroy()
    {
        Objects.Remove(this);
    }


    void CheckOutOfBounds()
    {
        if (!Respawner.Instance)
            return;
        if (!Respawner.Instance.Bounds.Contains(transform.position))
            Respawner.Instance.Respawn(this, plannedRespawnPosition);
    }
}
