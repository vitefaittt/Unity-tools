using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMusic : MonoBehaviour
{
    AudioSource audio;
    public float lowerDelay = 5;
    public float lowVolume = .1f;
    float defaultVolume;


    void Reset()
    {
        this.RenameFromType();
        this.GetOrAddComponent<AudioSource>().loop = true;
    }

    void Awake()
    {
        audio = GetComponent<AudioSource>();
        defaultVolume = audio.volume;
    }

    void Start()
    {
        // Lower the audio after some time in the scene.
        this.Timer(lowerDelay, () =>
        {
            this.ProgressionAnim(3, (progression) =>
            {
                audio.volume = Mathf.Lerp(defaultVolume, lowVolume, AniMath.Smooth(progression));
            });
        });
    }


    public void CutAudio()
    {
        float volumeBeforeEnding = audio.volume;
        this.ProgressionAnim(3, (progression) =>
        {
            audio.volume = Mathf.Lerp(volumeBeforeEnding, 0, AniMath.Smooth(progression));
        });
    }

    [ContextMenu("Rename from scene")]
    void RenameFromScene()
    {
        gameObject.name = typeof(SceneMusic).ToTitle() + " " + SceneManager.GetActiveScene().name;
    }
}
