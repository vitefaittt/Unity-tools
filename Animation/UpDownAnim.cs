using UnityEngine;

public class UpDownAnim : MonoBehaviour
{
    float startY;
    [SerializeField]
    float speed = 5, strength = .5f;


    private void Start()
    {
        startY = transform.localPosition.y;
    }

    private void Update()
    {
        transform.localPosition = transform.localPosition.SetY(startY + AnimUtilities.SinTime01(speed, strength));
    }
}