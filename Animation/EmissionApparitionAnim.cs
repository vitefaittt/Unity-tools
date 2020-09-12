using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EmissionApparitionAnim : MonoBehaviour
{
    public GameObject[] objects;
    Dictionary<GameObject, Renderer[]> objectsRenderers = new Dictionary<GameObject, Renderer[]>();
    [SerializeField]
    float delayBetweenObjects = .3f;
    [SerializeField]
    float apparitionDuration = .56f;

    [SerializeField]
    bool hideObjectsOnStart = true;


    void Reset()
    {
        objects = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
            objects[i] = transform.GetChild(i).gameObject;
    }

    void Start()
    {
        HideObjects();
    }


    [ContextMenu("Play")]
    public void Play()
    {
        // Get renderers and enable emission in each one.
        GetRenderers();
        HideObjects();
        for (int i = 0; i < objectsRenderers.Count; i++)
            EnableEmissionInRenderers(objectsRenderers.ElementAt(i).Value);

        // Start an animation for each object after a delay.
        for (int i = 0; i < objects.Length; i++)
        {
            if (!objects[i])
                continue;

            int index = i;
            this.Timer(index * delayBetweenObjects, () =>
            {
                objectsRenderers.ElementAt(index).Key.SetActive(true);
                this.ProgressionAnim(apparitionDuration, (progression) =>
                {
                    for (int j = 0; j < objectsRenderers.ElementAt(index).Value.Length; j++)
                        for (int k = 0; k < objectsRenderers.ElementAt(index).Value[j].materials.Length; k++)
                            objectsRenderers.ElementAt(index).Value[j].materials[k].SetColor("_EmissionColor", Color.white * (1 - progression));
                });
            });
        }
    }

    [ContextMenu("Hide")]
    public void HideObjects()
    {
        if (hideObjectsOnStart && objects != null)
            foreach (var objectToHide in objects)
                if (objectToHide)
                    objectToHide.SetActive(false);
    }

    void GetRenderers()
    {
        objectsRenderers.Clear();
        for (int i = 0; i < objects.Length; i++)
            if (objects[i])
                objectsRenderers.Add(objects[i], objects[i].GetComponentsInChildren<Renderer>(true));
    }

    void EnableEmissionInRenderers(Renderer[] renderers)
    {
        foreach (Renderer rend in renderers)
            if (rend)
                for (int i = 0; i < rend.materials.Length; i++)
                    rend.materials[i].EnableKeyword("_EMISSION");
    }
}
