using UnityEngine;

public class AutoAim : MonoBehaviour
{
    [SerializeField]
    Transform target;
    public Transform Target => target;
    [SerializeField]
    Health playerHealth;

    [SerializeField]
    new Camera camera;
    bool shouldFollowTarget;
    public bool FollowsTarget { get { return shouldFollowTarget && target; } }

    [SerializeField]
    Transform weaponSocket;


    void Reset()
    {
        camera = Camera.main;
        playerHealth = FindObjectOfType<Health>();
    }

    void Awake()
    {
        playerHealth.onDamaged += HandleDamageSource;
    }

    void Update()
    {
        // Get whether we should follow the target.
        if (target && camera.SeesTarget(target) && Input.GetKey(KeyCode.LeftAlt))
            shouldFollowTarget = true;
        else
            shouldFollowTarget = false;

        // Follow the target or don't.
        if (shouldFollowTarget)
            weaponSocket.transform.LookAt(target);
        else
            weaponSocket.transform.localRotation = Quaternion.identity;

    }


    void HandleDamageSource(float damage, GameObject damageSource)
    {
        // Try to get a new target from the damage source.
        if (damageSource.GetComponent<EnemyController>())
            target = damageSource.GetComponentInChildren<Damageable>().transform;
    }
}
