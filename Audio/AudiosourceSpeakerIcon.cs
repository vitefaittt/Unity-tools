using UnityEngine;

public class AudiosourceSpeakerIcon : MonoBehaviour
{
    [SerializeField]
    GameObject speakerIcon;

    AudioSource audio;
    public float updateStep = .1f;
    const int sampleDataLength = 1024;

    float currentUpdateTime = 0;

    float clipLoudness;
    float[] clipSampleData = new float[sampleDataLength];


    private void Reset()
    {
        speakerIcon = transform.FindWithKeywords("speaker").gameObject;
    }

    private void Awake()
    {
        audio = GetComponent<AudioSource>();
    }

    private void Update()
    {
        currentUpdateTime += Time.deltaTime;
        if (currentUpdateTime >= updateStep)
        {
            currentUpdateTime = 0;
            audio.clip.GetData(clipSampleData, audio.timeSamples);
            clipLoudness = 0;
            foreach (var sample in clipSampleData)
                clipLoudness += Mathf.Abs(sample);
            clipLoudness /= sampleDataLength;
        }

        speakerIcon?.SetActive(clipLoudness >= .01f);
    }
}
