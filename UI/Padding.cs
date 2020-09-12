using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class Padding : MonoBehaviour
{
    [SerializeField]
    float size = 5;
    float previousSize = 5;
    int previousChildAmount;
    List<RectTransform> paddings = new List<RectTransform>();
    readonly string paddingName = "Padding";
    LayoutGroup layoutGroup;
    [SerializeField]
    bool updateOnPlay;
    bool shouldReenableLayoutGroup;


    private void Start()
    {
        // Get paddings at startup.
        paddings = new List<RectTransform>();
        int normalChildrenCount = 0;
        foreach (Transform child in transform)
            if (child.name == paddingName)
                paddings.Add((RectTransform)child);
            else
                normalChildrenCount++;
        previousChildAmount = normalChildrenCount;
        // Get layout group.
        layoutGroup = GetComponentInParent<LayoutGroup>();

        print("start");
    }

    private void OnEnable()
    {
        // Only works on UI.
        if (!GetComponent<RectTransform>())
            enabled = false;
    }

    private void Update()
    {
        if (!updateOnPlay && Application.isPlaying)
            return;

        if (previousChildAmount != transform.childCount || size != previousSize)
        {
            UpdateCount();
            previousChildAmount = transform.childCount;
            previousSize = size;
        }

        if (shouldReenableLayoutGroup)
            layoutGroup.enabled = true;
    }


    void UpdateCount()
    {
        // Update amount.
        int amountToSpawn = -1;
        foreach (Transform child in transform)
            if (child.name != paddingName && child.gameObject.activeSelf)
                amountToSpawn++;
        print(amountToSpawn);
        if (paddings.Count < amountToSpawn)
            for (int i = paddings.Count; i < amountToSpawn; i++)
            {
                paddings.Add(new GameObject(paddingName, typeof(RectTransform)).GetComponent<RectTransform>());
                paddings[paddings.Count - 1].parent = transform;
                paddings[paddings.Count - 1].SetSiblingIndex(transform.childCount - 2);
            }
        else if (paddings.Count > amountToSpawn && amountToSpawn >= 0)
            for (int i = paddings.Count; i > amountToSpawn; i--)
            {
                DestroyImmediate(paddings[i - 1].gameObject);
                paddings.RemoveAt(i - 1);
            }
        print("new padd count " + paddings.Count);

        // Update spacing.
        foreach (var padding in paddings)
            padding.sizeDelta = Vector2.one * size;

        // Update layout group.
        if (layoutGroup)
        {
            print("layout group");
            layoutGroup.enabled = false;
            shouldReenableLayoutGroup = true;
        }
    }
}
