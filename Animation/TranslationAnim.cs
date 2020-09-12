using UnityEngine;

public class TranslationAnim : MonoBehaviour
{
    [SerializeField]
    Vector3 translationPerSec = Vector3.forward;
    [SerializeField]
    Space spaceRelativeTo = Space.Self;


    void Update()
    {
        transform.Translate(translationPerSec * Time.deltaTime, spaceRelativeTo);
    }
}
