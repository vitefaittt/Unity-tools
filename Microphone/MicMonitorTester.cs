using UnityEngine;

public class MicMonitorTester : MonoBehaviour
{
    MicMonitor monitor;
    public float dbThreshold = -80;
    public bool isSpeaking;


    private void Reset()
    {
        this.RenameFromType();
    }

    private void Start()
    {
        monitor = new MicMonitor();
    }

    private void Update()
    {
        monitor.speechDbThreshold = dbThreshold;
        monitor.Update();
        isSpeaking = monitor.IsSpeaking;
    }
}
