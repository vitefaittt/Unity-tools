using UnityEngine;

public class AudioAutoDestroy : MonoBehaviour
{
    void Start()
    {
        GetComponent<AudioSource>().OnAudioEnd(this, () => Destroy(gameObject));
    }
}
