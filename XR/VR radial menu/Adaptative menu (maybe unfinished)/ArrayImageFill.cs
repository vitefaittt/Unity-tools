using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ArrayImageFill : MonoBehaviour
{
    Arrays.Array array;
    [SerializeField]
    float spacing = .1f;
    ValueHolder<int> childCountHolder = new ValueHolder<int>();
    ValueHolder<float> spacingHolder = new ValueHolder<float>();


    void Awake()
    {
        array = GetComponent<Arrays.Array>();
        array.TransformsUpdated += UpdateImageFills;
    }

    void Update()
    {
        // Update when our childCount or our spacing change.
        childCountHolder.Value = transform.childCount;
        spacingHolder.Value = spacing;
        if (childCountHolder.ValueChanged || spacingHolder.ValueChanged)
            UpdateImageFills();
    }


    void UpdateImageFills()
    {
        float fill = 1 / (float)transform.childCount - spacing;
        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).GetComponent<Image>().fillAmount = fill;
    }
}
