using UnityEngine;

public class BuzzingAnim : MonoBehaviour
{
    Vector3 awakePosition;
    [SerializeField]
    Vector3 amplitude = new Vector3(1, 0, 1);
    Vector3 previousTargetPos;
    Vector3? targetPos;
    Vector3 velocity;
    [SerializeField]
    float duration = 1;
    [SerializeField]
    float maxSpeed = 1;
    [SerializeField]
    [Tooltip("When the distance to the target is smaller than smoothness, we switch targets. This smoothes out our movement.")]
    float smoothness = .7f;
    bool IsTargetReached => targetPos != null && Vector3.Distance(transform.localPosition, (Vector3)targetPos) < smoothness;


    private void Awake()
    {
        awakePosition = transform.localPosition;
        GetNewTargets();
    }

    private void Update()
    {
        if (IsTargetReached)
            GetNewTargets();

        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, (Vector3)targetPos, ref velocity, duration, maxSpeed);
    }


    void GetNewTargets()
    {
        previousTargetPos = targetPos ?? transform.localPosition;
        targetPos = awakePosition + new Vector3(Random.value.ToMin1_1() * amplitude.x, Random.value.ToMin1_1() * amplitude.y, Random.value.ToMin1_1()  * amplitude.z);
    }

    public void MatchSmoothnessToAmplitude()
    {
        smoothness = (amplitude.magnitude * .5f).RoundDecimals(3);
    }
}

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(BuzzingAnim)), UnityEditor.CanEditMultipleObjects]
class HoveringAnimEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GUILayout.Space(5);

        BuzzingAnim script = (BuzzingAnim)target;

        if (GUILayout.Button("Match smoothness to amplitude"))
            script.MatchSmoothnessToAmplitude();
    }
}
#endif
