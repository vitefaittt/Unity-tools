using System;
using UnityEngine;

public class SpawnOnCollision : MonoBehaviour
{
    [SerializeField]
    GameObject templateToSpawn;
    [SerializeField]
    public Func<bool, GameObject> Condition;
    [SerializeField]
    string tagToCheckFor;


    void OnCollisionEnter(Collision collision)
    {
        if (TagMatch(collision.gameObject, tagToCheckFor) && (Condition == null || Condition(collision.gameObject)))
            Instantiate(templateToSpawn, collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal));
    }


    static bool TagMatch(GameObject go, string tag)
    {
        return string.IsNullOrWhiteSpace(tag) || go.CompareTag(tag);
    }
}
