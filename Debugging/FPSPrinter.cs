using UnityEngine;

public class FPSPrinter : MonoBehaviour
{
    TextMesh text;
    public float Framerate => 1f / Time.deltaTime;
    float lastRefreshTime;
    [Tooltip("The amount of time between each display.")]
    public float refreshDelay = .2f;
    public bool dontDestroyOnLoad = true;
    [Tooltip("The text display is only active when this component is active.")]
    public bool canDisableTextDisplay = true;


    void Reset()
    {
        this.RenameFromType();
    }

    private void Awake()
    {
        text = VRPrinter.CreateDisplay(TextAnchor.UpperLeft);
        if (dontDestroyOnLoad)
            DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Time.time - lastRefreshTime > refreshDelay)
        {
            lastRefreshTime = Time.time;
            text.text = Framerate.ToString("0.00") + " fps";
        }
    }

    void OnEnable()
    {
        if (text && canDisableTextDisplay)
            text.gameObject.SetActive(true);
    }

    void OnDisable()
    {
        if (text && canDisableTextDisplay)
            text.gameObject.SetActive(false);
    }
}