using System.Collections;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class ScanSection : MonoBehaviour {

    Interactable interactable;
    Vector3 posOnGrab;
    Quaternion rotOnGrab;
    [SerializeField]
    GameObject menu;
    Vector3 menuStartScale;
    float menuCountdown = .2f;

    Hand lastHand;

    private void Awake()
    {
        interactable = GetComponent<Interactable>();
        interactable.onAttachedToHand += OnGrab;
        menu.SetActive(false);
        menuStartScale = menu.transform.localScale;
    }

    void OnGrab(Hand hand)
    {
        lastHand = hand;
        posOnGrab = transform.position;
        rotOnGrab = transform.rotation;
        StartCoroutine(ListenToGrab());
    }

    IEnumerator ListenToGrab()
    {
        // Only show the menu if we are not grabbed for long.
        float grabInstant = Time.time;
        yield return null;
        StartMenuPreview();
        while (interactable.attachedToHand != null && Time.time - grabInstant < menuCountdown)
        {
            yield return null;
        }
        if (Time.time - grabInstant < menuCountdown)
            CancelGrabToMenu();
    }

    void CancelGrabToMenu()
    {
        // Reset the position and show the menu.
        transform.position = posOnGrab;
        transform.rotation = rotOnGrab;
        ShowMenu();
        StartCoroutine(ListenForDeselect());
    }

    IEnumerator ListenForDeselect()
    {
        // If we click somewhere and we are not hovering anything, deselect.
        while (true)
        {
            if (Utilities.IsHandClickingNowhere(lastHand) || Utilities.IsHandClickingNowhere(lastHand.otherHand))
                OnDeselect();
            yield return null;
        }
    }

    void OnDeselect()
    {
        // Stop listening for deselect and hide the menu.
        StopAllCoroutines();
        HideMenu();
    }

    public void Destroy()
    {
        RemoveHighlight();
        Destroy(gameObject);
    }

    public void SetScannedMesh(GameObject mesh)
    {
        // Get the previous mesh, set the new mesh the same parent, then destroy the previous mesh.
        GameObject meshPlaceHolder = GetComponentInChildren<Renderer>().gameObject;
        mesh.transform.parent = meshPlaceHolder.transform.parent;
        Destroy(meshPlaceHolder);
    }


    void RemoveHighlight()
    {
        // Check if any of our interactables are hovered by a hand. If they are, tell the hand to forget about them.
        Interactable[] interactables = GetComponentsInChildren<Interactable>(true);
        foreach (Interactable interactabl in interactables)
        {
            if (lastHand.hoveringInteractable == interactabl)
                lastHand.hoveringInteractable = null;
            if (lastHand.otherHand.hoveringInteractable == interactabl)
                lastHand.otherHand.hoveringInteractable = null;
        }
    }

    #region Show/hide menu --------------
    IEnumerator playingMenu;
    void StartMenuPreview()
    {
        ShowMenu();
        // Hide the menu over time.
        playingMenu = this.MakeProgressionAnim(.3f, .5f, 0,
        delegate (float progression)
        {
            menu.transform.localScale = Vector3.Lerp(menuStartScale, Vector3.zero, progression);
        },
        delegate
        {
            HideMenu();
        });
    }

    void ShowMenu()
    {
        // Stop the menu animation, enable the menu and face it to the player.
        if (playingMenu != null)
            StopCoroutine(playingMenu);
        menu.SetActive(true);
        menu.transform.localScale = menuStartScale;
        menu.transform.LookAt(Player.instance.headCollider.transform.position);
    }

    void HideMenu()
    {
        // Stop the animation and disable the menu.
        if (playingMenu != null)
            StopCoroutine(playingMenu);
        menu.transform.localScale = menuStartScale;
        menu.SetActive(false);
    }
    #endregion ----------------------------
}