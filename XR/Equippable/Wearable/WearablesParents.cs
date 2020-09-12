using System;
using UnityEngine;

public class WearablesParents : MonoBehaviour
{
    public enum BodyPlacement { Head }
    [SerializeField]
    WearableTransform[] bodyPlacements;

    public static WearablesParents Instance { get; private set; }


    private void Awake()
    {
        Instance = this;
    }


    public Transform GetPlacement(WearablesParents.BodyPlacement placement)
    {
        return Array.Find(bodyPlacements, p => p.Placement == placement)?.Transform;
    }
}

[Serializable]
class WearableTransform
{
    [SerializeField]
    WearablesParents.BodyPlacement placement;
    public WearablesParents.BodyPlacement Placement => placement;
    [SerializeField]
    Transform transform;
    public Transform Transform => transform;
}