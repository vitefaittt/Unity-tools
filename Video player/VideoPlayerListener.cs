using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

public class VideoPlayerListener : MonoBehaviour
{
    bool wasPlaying;
    VideoPlayer player;
    public UnityEvent Played, Stopped;


    void Awake()
    {
        player = GetComponent<VideoPlayer>();
    }


    void Update()
    {
        if (!wasPlaying && player.isPlaying)
            Played.Invoke();
        else if (wasPlaying && !player.isPlaying)
            Stopped.Invoke();

        wasPlaying = player.isPlaying;
    }
}
