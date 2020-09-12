using UnityEngine;
using UnityEngine.UI;

public class Occluder : MonoBehaviour
{
    Image image;
    Color startColor;
    [SerializeField]
    float fadeDuration = 1;
    [SerializeField]
    Color fadeColor = Color.black;
    [SerializeField]
    bool dontDestroyOnLoad = true;
    public event System.Action FadeInComplete, FadeOutCompleted;

    public static Occluder Instance { get; private set; }


    private void Reset()
    {
        this.RenameFromType();
        GetComponentInChildren<Image>().color = Color.clear;
        if (GetComponent<Canvas>())
        {
            GetComponent<Canvas>().worldCamera = Camera.main;
            GetComponent<Canvas>().planeDistance = .1f;
        }
    }

    private void Awake()
    {
        Instance = this;
        if (dontDestroyOnLoad)
            this.GetOrAddComponent<DontDestroyOnLoad>();
        image = GetComponentInChildren<Image>();
        startColor = image.color;
    }

    private void Start()
    {
        image.color = Color.clear;
    }


    /// <summary>
    /// Show the occluder over time.
    /// </summary>
    public void FadeIn(System.Action EndAction = null)
    {
        StopAllCoroutines();
        this.ProgressionAnim(fadeDuration, delegate (float progression)
        {
            image.color = Color.Lerp(startColor, fadeColor, progression);
        }, delegate
        {
            // Invoke endAction and event.
            EndAction?.Invoke();
            FadeInComplete?.Invoke();
        });
    }

    /// <summary>
    /// Hide the occluder over time.
    /// </summary>
    public void FadeOut(System.Action EndAction = null)
    {
        StopAllCoroutines();
        this.ProgressionAnim(fadeDuration, delegate (float progression)
        {
            image.color = Color.Lerp(fadeColor, startColor, progression);
        }, delegate
        {
            // Invoke endAction and event.
            EndAction?.Invoke();
            FadeInComplete?.Invoke();
        });
    }
}
