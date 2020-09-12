using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class ObjectMenu : MonoBehaviour {

    Interactable interactable;
    private void Awake()
    {
        interactable = GetComponentInParent<Interactable>();
        gameObject.SetActive(false);
    }


}
