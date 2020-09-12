using UnityEngine;
using UnityEngine.Events;

public class BPMEvent_LastFireTime : MonoBehaviour
{
    [SerializeField]
    float bpm = 110;
    [SerializeField]
    bool continuousUpdate;
    public UnityEvent OnBeat;
    float beatInterval;
    float lastFireTime;


    private void Start()
    {
        SetBeatInterval();
    }

    private void Update()
    {
        if (continuousUpdate)
            SetBeatInterval(bpm);

        // Fire onbeat the number of times that a beat was reached in the previous frame.
        if (Time.time - lastFireTime > beatInterval)
        {
            for (int i = 0; i < beatInterval / (Time.time - lastFireTime); i++)
                OnBeat.Invoke();
            lastFireTime = Time.time;
        }
    }


    void SetBeatInterval(float newBpm = -1)
    {
        if (newBpm >= 0)
            bpm = newBpm;
        beatInterval = 60 / bpm;
    }
}
