using UnityEngine;

public class FluidPour : MonoBehaviour
{
    public ParticleSystem fluidParticles;
    public Transform neck;
    public float pourAngle = 55;
    [Tooltip("Position the particles on the edge of the neck. Set to -1 to not reposition the particles.")]
    public float neckRadius = .1f;


    void Reset()
    {
        neck = transform;
        fluidParticles = GetComponentInChildren<ParticleSystem>();
    }

    void Update()
    {
        // Pour the liquid if we are tilted enough.
        if (Vector3.Angle(neck.up, Vector3.up) > pourAngle)
        {
            if (!fluidParticles.isPlaying)
                fluidParticles.Play();
            if (neckRadius > 0)
                fluidParticles.transform.position = GetPourPosition(neck, neckRadius);
        }
        else if (fluidParticles.isPlaying)
            fluidParticles.Stop();
    }


    static Vector3 GetPourPosition(Transform neck, float neckRadius)
    {
        return neck.position + (Vector3.ProjectOnPlane(Vector3.down, neck.up).normalized * neckRadius);
    }
}


#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(FluidPour)), UnityEditor.CanEditMultipleObjects]
class FluidPourEditor : UnityEditor.Editor
{
    public void OnSceneGUI()
    {
        FluidPour script = (FluidPour)target;
        if (script.neck)
            UnityEditor.Handles.DrawWireDisc(script.neck.position, script.neck.up, script.neckRadius);
    }
}
#endif

