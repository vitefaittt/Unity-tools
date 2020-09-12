using System.Collections.Generic;
using UnityEngine;

public class EmissionHint : MonoBehaviour
{
    public float strength = .3f;
    [SerializeField]
    Renderer rend;
    Dictionary<Material, Color> defaultEmissions = new Dictionary<Material, Color>();


    void Reset()
    {
        rend = GetComponentInChildren<Renderer>();
    }

    void Awake()
    {
        // Get renderer and enable emission on its materials.
        rend = GetComponent<Renderer>();
        for (int i = 0; i < rend.materials.Length; i++)
        {
            rend.materials[i].EnableKeyword("_EMISSION");
            defaultEmissions.Add(rend.materials[i], rend.materials[i].GetColor("_EmissionColor"));
        }
    }

    public void Hint()
    {
        // Increase the emission on each material.
        for (int i = 0; i < rend.materials.Length; i++)
            if (defaultEmissions.ContainsKey(rend.materials[i]))
                rend.materials[i].SetColor("_EmissionColor", defaultEmissions[rend.materials[i]] + Color.white * strength);
    }

    public void UnHint()
    {
        // Reset the emission of every material to its default value.
        for (int i = 0; i < rend.materials.Length; i++)
            if (defaultEmissions.ContainsKey(rend.materials[i]))
                rend.materials[i].SetColor("_EmissionColor", defaultEmissions[rend.materials[i]]);
    }
}
