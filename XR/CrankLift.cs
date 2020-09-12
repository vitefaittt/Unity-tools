using UnityEngine;
using UnityEngine.Events;

public class CrankLift : MonoBehaviour
{
    CircularDrive circularDrive;

    float previousDrive;
    float maxVelocity = .5f;
    float velocity = 0;
    float accelerationSpeed = 1;
    [SerializeField]
    float targetY = 2.5f;

    new AudioSource audio;
    [SerializeField]
    Renderer cordeRend;

    float startPlayerPosY;
    bool moved;

    public UnityEvent Moves;
    public UnityEvent StopsMoving;

    public static Lift Instance { get; private set; }


    private void Awake()
    {
        Instance = this;
        circularDrive = GetComponentInChildren<CircularDrive>();
        audio = GetComponentInChildren<AudioSource>();
    }

    private void Start()
    {
        startPlayerPosY = Player.instance.transform.position.y;
    }

    private void Update()
    {
        // Get velocity from circular drive.
        velocity += ((circularDrive.outAngle - previousDrive > 3) ? 1 : -1) * Time.deltaTime * accelerationSpeed;
        velocity = Mathf.Clamp(velocity, 0, maxVelocity);
        previousDrive = circularDrive.outAngle;

        // Move up the player from our velocity.
        transform.Translate(Vector3.up * velocity * Time.deltaTime);
        Player.instance.transform.position = Player.instance.transform.position.SetY(transform.position.y + startPlayerPosY);
        cordeRend.material.SetTextureOffset("_MainTex", Vector2.up * transform.position.y);

        // Call event.
        if (Player.instance.transform.position.y > startPlayerPosY + .1f && !moved)
            Moves.Invoke();

        if (transform.position.y >= targetY)
        {
            StopsMoving.Invoke();
            enabled = false;
        }
    }
}
