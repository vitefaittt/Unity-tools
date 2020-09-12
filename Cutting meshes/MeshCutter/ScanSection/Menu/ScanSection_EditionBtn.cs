using UnityEngine.UI;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class ScanSection_EditionBtn : MonoBehaviour {

    ScanSection section;
    [SerializeField]
    GameObject activationCue;

    private void Awake()
    {
        // Get parent section and listen to button component.
        section = GetComponentInParent<ScanSection>();
        GetComponent<UIElement>().onHandClick.AddListener(delegate { ToggleEditMode(); });
        UpdateActivationCue();
    }

    void ToggleEditMode()
    {
        // Tell the parent section not to wait for deselect, and show its state through activationCue.
        //section.SetEditionActive(!section.editionActive);
        UpdateActivationCue();
    }

    void UpdateActivationCue()
    {
        //activationCue.SetActive(section.editionActive);
    }
}
