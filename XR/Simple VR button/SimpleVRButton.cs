using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleVRButton : MonoBehaviour
{
    [SerializeField]
    ProgressionCircle circleTemplate;
    ProgressionCircle circle;
    List<SimpleVRButtonPointer> pointers = new List<SimpleVRButtonPointer>();
    [SerializeField]
    float clickDelay = 2;


    private void Reset()
    {
        if (GetComponentInChildren<Collider>())
        {
            GetComponentInChildren<Collider>().isTrigger = true;
            print("Set " + GetComponentInChildren<Collider>() + " as trigger.");
        }
    }

    private void Awake()
    {
        circle = Instantiate(circleTemplate, transform, false);
        circle.transform.localPosition = Vector3.forward * .3f;
        circle.transform.localScale = Vector3.one;
    }

    private void OnTriggerStay(Collider other)
    {
        SimpleVRButtonPointer otherPointer = other.GetComponent<SimpleVRButtonPointer>();
        if (otherPointer && !pointers.Contains(otherPointer))
            pointers.Add(otherPointer);

        // Update the circle progression and click when progression complete.
        if (pointers.Count > 0)
        {
            circle.progression += 1 / clickDelay * Time.deltaTime;
            if (circle.progression >= .99f)
                Click();
        }
        else
            circle.progression = 0;
    }


    public void Click()
    {
        GetComponent<Button>().onClick.Invoke();
        circle.progression = 0;
    }
}
