using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EmissionFlashAnim : FlashAnim
{
    List<Renderer> rends = new List<Renderer>();
    [SerializeField]
    float strength = 1;
    [SerializeField]
    Color color = Color.white;
    [Space]
    [SerializeField]
    [Tooltip("Apply the effect to all children renderers.")]
    bool inEveryChild;


    private void Awake()
    {
        // Get affected renderers.
        if (!inEveryChild)
            rends.Add(GetComponent<Renderer>());
        else
            rends = GetComponentsInChildren<Renderer>(true).ToList();

        // Enable emission on renderer materials.
        foreach (Renderer rend in rends)
            for (int i = 0; i < rend.materials.Length; i++)
                rend.materials[i].EnableKeyword("_EMISSION");
    }


    protected override void HandleProgression(float progression)
    {
        for (int i = 0; i < rends.Count; i++)
            for (int j = 0; j < rends[i].materials.Length; j++)
                rends[i].materials[j].SetColor("_EmissionColor", color * progression * strength);
    }
}
