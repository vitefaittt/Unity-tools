using UnityEngine;

public class RandomTimeOffset : MonoBehaviour
{
    void OnEnable()
    {
        TimeControl.AddTimeOffset(Random.Range(-3600, 3600));
    }
}
