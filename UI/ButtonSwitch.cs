using UnityEngine;
using UnityEngine.UI;

public class ButtonSwitch : MonoBehaviour
{
    [SerializeField]
    Button buttonA, buttonB;


    private void Reset()
    {
        try
        {
            buttonA = GetComponentInChildren<Button>();
            buttonB = GetComponentsInChildren<Button>()[1];
        }
        catch { }
    }

    private void Awake()
    {
        ToggleA();
        buttonA.onClick.AddListener(ToggleB);
        buttonB.onClick.AddListener(ToggleA);
    }


    void ToggleB()
    {
        buttonA.interactable = true;
        buttonB.interactable = false;
    }

    public void ToggleA()
    {
        buttonA.interactable = false;
        buttonB.interactable = true;
    }
}
