using UnityEngine;

public class SplineWalker : MonoBehaviour
{
    [SerializeField]
    Spline spline;

    [SerializeField]
    float duration;

    [SerializeField]
    bool lookForward;

    enum Mode
    {
        Once,
        Loop,
        PingPong
    }
    Mode mode;

    float progress;
    bool goingForward = true;


    private void Update()
    {
        if (goingForward)
        {
            progress += Time.deltaTime / duration;
            if (progress > 1f)
            {
                if (mode == Mode.Once)
                    progress = 1f;
                else if (mode == Mode.Loop)
                    progress -= 1f;
                else
                {
                    progress = 2f - progress;
                    goingForward = false;
                }
            }
        }
        else
        {
            progress -= Time.deltaTime / duration;
            if (progress < 0f)
            {
                progress = -progress;
                goingForward = true;
            }
        }

        Vector3 position = spline.GetPoint(progress);
        transform.localPosition = position;
        if (lookForward)
            transform.LookAt(position + spline.GetDirection(progress));
    }
}