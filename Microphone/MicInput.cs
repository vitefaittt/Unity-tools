using UnityEngine;

public class MicInput : MonoBehaviour
{
    AudioClip clip;
    int sampleWindow = 128;

    public float Loudness { get; private set; }
    public float LoudnessinDecibels { get; private set; }

    public static MicInput Instance { set; get; }


    void OnEnable()
    {
        InitMic();
        Instance = this;
    }

    void OnDisable()
    {
        StopMicrophone();
    }

    void Update()
    {
        Loudness = MicrophoneLevelMax();
        LoudnessinDecibels = MicrophoneLevelMaxDecibels();
    }

    void OnDestroy()
    {
        StopMicrophone();
    }


    public void InitMic()
    {
        clip = Microphone.Start(Microphone.devices[0], true, 999, 44100);
    }

    public void StopMicrophone()
    {
        Microphone.End(null);
    }

    float MicrophoneLevelMax()
    {
        // Get data from default microphone into audioclip.
        float levelMax = 0;
        float[] waveData = new float[sampleWindow];
        int micPosition = Microphone.GetPosition(null) - (sampleWindow + 1);
        if (micPosition < 0) return 0;
        clip.GetData(waveData, micPosition);

        // Getting a peak on the last 128 samples.
        for (int i = 0; i < sampleWindow; i++)
        {
            float wavePeak = waveData[i] * waveData[i];
            if (levelMax < wavePeak)
                levelMax = wavePeak;
        }
        return levelMax;
    }

    float MicrophoneLevelMaxDecibels()
    {
        float db = 20 * Mathf.Log10(Mathf.Abs(Loudness));
        return db;
    }
}