using UnityEngine;

public abstract class FlashAnim : MonoBehaviour
{
    public float delay = .5f;
    public float flashDuration = .7f;

    public event System.Action Flashed;


    public void OnEnable()
    {
        Flash();
    }

    void OnDisable()
    {
        HandleProgression(0);
    }


    protected abstract void HandleProgression(float progression);

    void Flash()
    {
        if (!gameObject.activeInHierarchy)
            return;
        this.ProgressionAnim(flashDuration, progression =>
        {
            HandleProgression(AniMath.Bell(progression));
        }, OnFlashEnd);
    }

    void OnFlashEnd()
    {
        // Call event.
        Flashed?.Invoke();

        if (!gameObject.activeInHierarchy)
            return;

        // Restart.
        if (delay <= 0)
            Flash();
        else
            this.Timer(delay, Flash);
    }
}
