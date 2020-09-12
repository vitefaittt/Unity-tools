using UnityEngine;

public class Comestible : MonoBehaviour
{
    [SerializeField]
    string collisionTag = "HeadCollider";

    [Header("On first bite")]
    [SerializeField]
    GameObject destroy;
    [SerializeField]
    GameObject activate;

    [Space]
    [SerializeField]
    float delayBetweenBites = .5f;
    [SerializeField]
    AudioClip sfx;
    [SerializeField]
    float volume = 1;
    float lastBiteTime = 0;
    public bool destroyInTheEnd = true;

    public event System.Action Consumed;


    private void Start()
    {
        if (activate)
            activate.SetActive(false);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag(collisionTag) || Time.time - lastBiteTime < delayBetweenBites)
            return;

        Consumed?.Invoke();

        // Show the new mesh and play a sound.
        if (sfx)
            AudioSource.PlayClipAtPoint(sfx, transform.position, volume);
        if (destroy)
        {
            Destroy(destroy);
            if (activate)
                activate.SetActive(true);
        }
        else
        {
            if (destroyInTheEnd)
                Destroy(gameObject);
        }
        lastBiteTime = Time.time;
    }
}
