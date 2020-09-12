using UnityEngine;

public class TexturesAnim : MonoBehaviour
{
    [SerializeField]
    bool playOnAwake = true;
    [SerializeField]
    float frameDuration = 1;

    Renderer rend;
    int currentFrame = 0;
    [SerializeField] Texture[] frames;


    void Awake()
    {
        rend = GetComponent<Renderer>();
        if (playOnAwake)
            Play();
    }


    public void Play()
    {
        currentFrame = 0;
        RunAnimLoop();
    }

    public void Pause()
    {
        StopAllCoroutines();
    }

    public void Unpause()
    {
        RunAnimLoop();
    }

    public void Stop()
    {
        currentFrame = 0;
        StopAllCoroutines();
    }

    void RunAnimLoop()
    {
        rend.material.SetTexture("_MainTex", frames[currentFrame]);
        currentFrame++;
        if (currentFrame > frames.Length - 1)
            currentFrame = 0;
        this.Timer(frameDuration, RunAnimLoop);
    }
}
