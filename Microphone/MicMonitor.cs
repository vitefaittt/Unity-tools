using System;
using UnityEngine;

public class MicMonitor
{
    public MicInput MicInput
    {
        get
        {
            if (!MicInput.Instance)
            {
                MicInput newInput = new GameObject("Mic input").AddComponent<MicInput>();
                return newInput;
            }
            return MicInput.Instance;
        }
    }
    float lastSpeechTime;
    public float speechDbThreshold = -80;
    public float speechDurationThreshold = 1.5f;
    public bool IsSpeaking { get; private set; }
    public event Action SpeechStart, SpeechEnd;


    public MicMonitor() { }

    public MicMonitor(float speechDbThreshold, float speechDurationThreshold)
    {
        this.speechDbThreshold = speechDbThreshold;
        this.speechDurationThreshold = speechDurationThreshold;
    }

    public void Reset()
    {
        lastSpeechTime = Time.time;
    }

    public void Update()
    {
        if (MicInput.LoudnessinDecibels > speechDbThreshold)
        {
            lastSpeechTime = Time.time;
            IsSpeaking = true;
            SpeechStart?.Invoke();
        }

        if (IsSpeaking && Time.time - lastSpeechTime > speechDurationThreshold)
        {
            IsSpeaking = false;
            SpeechEnd?.Invoke();
        }
    }
}
