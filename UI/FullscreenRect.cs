using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FullscreenRect : MonoBehaviour
{
    [SerializeField]
    Button fullscreenButton, exitFullscreenButton;
    Image blocker;
    AspectRatioFitter ratioFitter;
    Vector2 startAnchorMin, startAnchorMax;
    Vector2 startSizeDelta;
    Vector2 startLocalPos;
    int startSiblingIndex;

    [SerializeField]
    Camera renderCamera;
    RawImage image;
    Vector2 startTextureResolution;

    bool fullscreenOn;

    public enum FullscreenBehaviour { DoNothing, BottomOfHierarchy, BottomOfHierarchyMinusIndex }
    [Header("Changing sibling index")]
    [SerializeField]
    public FullscreenBehaviour fullscreenBehaviour;
    [SerializeField]
    int hierarchyMinusIndex;

    public UnityEvent FullscreenEnabled, FullscreenDisabled;


    private void Start()
    {
        image = GetComponent<RawImage>();
        startTextureResolution = new Vector2(image.texture.width, image.texture.height);
        // Setup ratio fitter.
        ratioFitter = transform.GetOrAddComponent<AspectRatioFitter>();
        ratioFitter.aspectRatio = ((RectTransform)transform).GetAspectRatio();
        // Get start rect position.
        startAnchorMin = ((RectTransform)transform).anchorMin;
        startAnchorMax = ((RectTransform)transform).anchorMax;
        startSizeDelta = ((RectTransform)transform).sizeDelta;
        startLocalPos = transform.localPosition;
        startSiblingIndex = transform.GetSiblingIndex();
        // Exit fullscreen.
        fullscreenOn = true;    
        ExitFullscreen();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            ExitFullscreen();
    }


    [ContextMenu("Assign events")]
    void AssignEvents()
    {
        try
        {
            fullscreenButton.onClick.AddPersistentEvent(this, Fullscreen);
            exitFullscreenButton.onClick.AddPersistentEvent(this, ExitFullscreen);
        }
        catch { }
    }

    public void Fullscreen()
    {
        UpdateFullscreenState(true);
        // Call event.
        if (FullscreenEnabled != null)
            FullscreenEnabled.Invoke();
    }

    public void ExitFullscreen()
    {
        UpdateFullscreenState(false);
        // Call event.
        if (FullscreenDisabled != null)
            FullscreenDisabled.Invoke();
    }

    void UpdateFullscreenState(bool newState)
    {
        if (fullscreenOn == newState)
            return;
        fullscreenOn = newState;

        // Toggle buttons.
        fullscreenButton.gameObject.SetActive(!newState);
        exitFullscreenButton.gameObject.SetActive(newState);

        // Change sibling index.
        if (fullscreenBehaviour != FullscreenBehaviour.DoNothing)
        {
            if (newState)
            {
                if (fullscreenBehaviour == FullscreenBehaviour.BottomOfHierarchy)
                    transform.SetSiblingIndex(transform.parent.childCount - 1);
                else if (fullscreenBehaviour == FullscreenBehaviour.BottomOfHierarchyMinusIndex)
                    transform.SetSiblingIndex(Mathf.Max(transform.parent.childCount - 1 - hierarchyMinusIndex, 0));
            }
            else
                transform.SetSiblingIndex(startSiblingIndex);
        }

        // Update the blocker.
        if (!blocker)
            blocker = UIUtilities.CreateBlocker(transform.parent, Color.black, transform.GetSiblingIndex());
        blocker.enabled = newState;

        // Resize.
        if (newState)
            ratioFitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
        else
        {
            ratioFitter.aspectMode = AspectRatioFitter.AspectMode.None;
            // Reset our rectTransform.
            ((RectTransform)transform).anchorMin = startAnchorMin;
            ((RectTransform)transform).anchorMax = startAnchorMax;
            ((RectTransform)transform).sizeDelta = startSizeDelta;
            transform.localPosition = startLocalPos;
        }

        // Resize the render texture.
        if (newState)
            image.texture = UIUtilities.SetRenderTextureResolution(renderCamera, new Vector2(Screen.width, Screen.height));
        else
            image.texture = UIUtilities.SetRenderTextureResolution(renderCamera, startTextureResolution);
    }
}

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(FullscreenRect)), UnityEditor.CanEditMultipleObjects]
public class FullscreenRectEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        FullscreenRect script = target as FullscreenRect;
        System.Collections.Generic.List<string> excludedProperties = new System.Collections.Generic.List<string>();

        if (script.fullscreenBehaviour != FullscreenRect.FullscreenBehaviour.BottomOfHierarchyMinusIndex)
            excludedProperties.Add("hierarchyMinusIndex");

        DrawPropertiesExcluding(serializedObject, excludedProperties.ToArray());
        serializedObject.ApplyModifiedProperties();
    }
}
#endif