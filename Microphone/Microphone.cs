using UnityEngine;

public class Microphonee : MonoBehaviour
{
    AudioSource audio;


    private void Awake()
    {
        audio = GetComponent<AudioSource>();
    }

    private void Start()
    {
        audio.clip = UnityEngine.Microphone.Start(null, true, 10, 44100);
        audio.Play();
        //print(audio.clip.GetData(UnityEngine.Microphone.GetPosition()))
    }
}
