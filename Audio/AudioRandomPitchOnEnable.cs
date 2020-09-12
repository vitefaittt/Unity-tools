using UnityEngine;

public class AudioRandomPitchOnEnable : MonoBehaviour
{
    public Vector2 minMax = new Vector2(.7f, 1.3f);


    void OnEnable()
    {
        SetRandomPitch();
    }


    public void SetRandomPitch()
    {
        GetComponent<AudioSource>().pitch = Random.Range(minMax.x, minMax.y);
    }
}
