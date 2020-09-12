using UnityEngine;
using UnityEngine.UI;

public class ChatMessage : MonoBehaviour
{
    Text text;
    public string Text { get => text.text; set => text.text = value; }
    public float deletionDelay = 5;
    [SerializeField]
    float deletionSpeed = .5f;


    void Awake()
    {
        text = GetComponentInChildren<Text>(true);
    }

    void Start()
    {
        this.Timer(deletionDelay, () =>
        {
            this.ProgressionAnim(deletionSpeed, progression =>
            {
                text.color = Color.white.SetA(1 - progression);
            }, () =>
            {
                Destroy(gameObject);
            });
        });
    }
}