using UnityEngine;

[RequireComponent(typeof(FluidPour))]
public class FluidPourAdvanced : MonoBehaviour
{
    FluidPour pour;
    ParticleSystem.MainModule fluidModule;
    public float maxPourAngle = 150;
    public float CurrentPourIntensity { get; private set; }

    [Header("Particles")]
    [Tooltip("By default: (particles.startSize * .5, particles.startSize * 2).")]
    public Vector2 minMaxParticleSize;

    [Header("Audio")]
    public AudioSource audio;
    public Vector2 minMaxVolume = new Vector2(0, 1);
    public Vector2 minMaxPitch = new Vector2(2, 1);


    void Reset()
    {
        pour = GetComponent<FluidPour>();
        if (pour && pour.fluidParticles)
        {
            // Get minMax particle size from the existing particle size.
            minMaxParticleSize = new Vector2(pour.fluidParticles.main.startSizeMultiplier * .5f, pour.fluidParticles.main.startSizeMultiplier * 2);

            // Setup an audiosource on the particles.
            audio = pour.fluidParticles.GetComponent<AudioSource>();
            if (!audio)
            {
                audio = pour.fluidParticles.gameObject.AddComponent<AudioSource>();
                audio.spatialBlend = 1;
                Debug.Log("Created an audioSource on " + pour.fluidParticles.gameObject);
            }
        }
    }

    void Awake()
    {
        // Get pour & pour particles.
        pour = GetComponent<FluidPour>();
        fluidModule = pour.fluidParticles.main;
    }

    void Update()
    {
        CurrentPourIntensity = GetPourIntensity();

        if (pour.IsPouring)
        {
            // Pour smaller or bigger particles.
            fluidModule.startSizeMultiplier = Mathf.Lerp(minMaxParticleSize.x, minMaxParticleSize.y, CurrentPourIntensity);

            // Play the audio and change its volume and pitch.
            if (!audio.isPlaying)
                audio.Play();
            audio.volume = Mathf.Lerp(minMaxVolume.x, minMaxVolume.y, CurrentPourIntensity * CurrentPourIntensity);
            audio.pitch = Mathf.Lerp(minMaxPitch.x, minMaxPitch.y, CurrentPourIntensity * CurrentPourIntensity);
        }
        else
        {
            // Stop the audio.
            if (audio.isPlaying)
                audio.Stop();
        }
    }


    float GetPourIntensity()
    {
        if (!pour.IsPouring)
            return 0;

        // Return the progression between minPourAngle and maxPourAngle.
        return (pour.CurrentPourAngle - pour.minPourAngle) / (maxPourAngle - pour.minPourAngle);
    }
}
