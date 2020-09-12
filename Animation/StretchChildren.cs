using System.Collections.Generic;
using UnityEngine;

public class StretchChildren : MonoBehaviour
{
    [SerializeField]
    Transform parent;
    List<Vector3> startLocalPositions = new List<Vector3>();
    List<Vector3> targetLocalPositions = new List<Vector3>();

    bool isStretching = false;
    Coroutine animRoutine;

    [SerializeField]
    float strength = 1;


    void Reset()
    {
        parent = transform;
    }

    private void Start()
    {
        foreach (Transform child in parent)
        {
            startLocalPositions.Add(child.localPosition);
            targetLocalPositions.Add(child.localPosition + child.localPosition.normalized * strength);
        }
    }


    public void Stretch()
    {
        if (!isStretching)
            ToggleStretch();
    }

    public void Unstretch()
    {
        if (isStretching)
            ToggleStretch();
    }

    public void ToggleStretch()
    {
        if (animRoutine.IsRunning())
            StopCoroutine(animRoutine);
        isStretching = !isStretching;
        animRoutine = this.ProgressionAnim(3, delegate (float progression)
        {
            for (int i = 0; i < parent.childCount; i++)
                parent.GetChild(i).localPosition = Vector3.Lerp(parent.GetChild(i).localPosition, isStretching ? targetLocalPositions[i] : startLocalPositions[i], progression);
        }, delegate { animRoutine = null; });
    }
}