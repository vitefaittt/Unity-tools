using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BPMFinder : MonoBehaviour
{
    public float ResultBPM { get; private set; }
    List<float> timestamps = new List<float>();
    float lastTap = -1;

    public UnityEvent<float> BPMUpdated;


    private void Reset()
    {
        this.RenameFromType(false);
    }


    public void Tap()
    {
        if (lastTap > 0)
            timestamps.Add(Time.time - lastTap);
        lastTap = Time.time;
        ComputeBPM();
    }

    void ComputeBPM()
    {
        float sum = 0;
        for (int i = 0; i < timestamps.Count; i++)
            sum += timestamps[i];
        ResultBPM = 60 / (sum / timestamps.Count);
        BPMUpdated.Invoke(ResultBPM);
    }
}
