using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerMovementMonitor
{
    public Transform head;

    Vector3 prevHeadPosition;
    public PlayerMovementMetrics Metrics { get; private set; }

    Vector3 prevRight, prevForward, prevUp;


    public PlayerMovementMonitor(Transform head)
    {
        this.head = head;
        prevHeadPosition = head.position;
    }


    public void Reset()
    {
        Metrics = new PlayerMovementMetrics();

        prevRight = head.right;
        prevForward = head.forward;
        prevUp = head.up;
    }

    public void Update()
    {
        // Logging the movement.
        Vector3 currentMovement = head.position - prevHeadPosition;
        prevHeadPosition = head.position;
        // (Don't log the movement when the headset is resting.)
        if (currentMovement.magnitude >= .0005f)
        {
            Metrics.horizontalMov += new Vector2(currentMovement.x, currentMovement.z).magnitude;
            Metrics.verticalMov += Mathf.Abs(currentMovement.z);
        }

        // Logging the rotation.
        // Yaw.
        Vector3 flattenedPrevForward = Vector3.ProjectOnPlane(prevForward, head.up).normalized;
        Vector3 flattenedForward = Vector3.ProjectOnPlane(head.forward, head.up).normalized;
        Metrics.totalYaw += Vector3.Angle(flattenedPrevForward, flattenedForward);

        // Pitch.
        Vector3 flattenedPrevUp = Vector3.ProjectOnPlane(prevUp, head.right).normalized;
        Vector3 flattenedUp = Vector3.ProjectOnPlane(head.up, head.right).normalized;
        Metrics.totalPitch += Vector3.Angle(flattenedPrevUp, flattenedUp);

        // Roll.
        Vector3 flattenedPrevRight = Vector3.ProjectOnPlane(prevRight, head.forward).normalized;
        Vector3 flattenedRight = Vector3.ProjectOnPlane(head.right, head.forward).normalized;
        Metrics.totalRoll += Vector3.Angle(flattenedPrevRight, flattenedRight);

        prevForward = head.forward;
        prevUp = head.up;
        prevRight = head.right;
    }
}

public class PlayerMovementMetrics
{
    public float horizontalMov, verticalMov;
    public float totalYaw, totalPitch, totalRoll;

    public static string GetCSVHeaders()
    {
        return new CSVBuilder("Dépl. horizontaux (m)", "Dépl. verticaux (m)",
            "Yaw tête", "Pitch tête", "Roll tête").Content;
    }

    public string ToCSV()
    {
        return new CSVBuilder(horizontalMov.ToString("0.000"), verticalMov.ToString("0.000"),
                totalYaw.ToString("0.000"), totalPitch.ToString("0.000"), totalRoll.ToString("0.000")).Content;
    }

    public static PlayerMovementMetrics operator +(PlayerMovementMetrics a, PlayerMovementMetrics b)
    {
        PlayerMovementMetrics c = new PlayerMovementMetrics();
        if (a == null)
            return b;
        if (b == null)
            return a;
        c.horizontalMov = a.horizontalMov + b.horizontalMov;
        c.verticalMov = a.verticalMov + b.verticalMov;
        c.totalYaw = a.totalYaw + b.totalYaw;
        c.totalPitch = a.totalPitch + b.totalPitch;
        c.totalRoll = a.totalRoll + b.totalRoll;
        return c;
    }

    public static PlayerMovementMetrics Sum<T>(IEnumerable<T> collection, Func<T, PlayerMovementMetrics> selector)
    {
        if (collection.Count() < 1)
            return new PlayerMovementMetrics();
        PlayerMovementMetrics result = null;
        for (int i = 0; i < collection.Count(); i++)
            result += selector(collection.ElementAt(i));
        return result;
    }
}