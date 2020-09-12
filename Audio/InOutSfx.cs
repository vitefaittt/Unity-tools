using UnityEngine;

public class InOutSfx : MonoBehaviour
{
    public AudioClip inSfx, outSfx;
    public float volume = 1;

    public void PlayIn()
    {
        if (inSfx)
            AudioSource.PlayClipAtPoint(inSfx, transform.position, volume);
    }

    public void PlayOut()
    {
        if (outSfx)
            AudioSource.PlayClipAtPoint(outSfx, transform.position, volume);
    }
}
