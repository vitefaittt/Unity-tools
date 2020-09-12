using System.Collections.Generic;
using UnityEngine;

public class CrepuscularSceneBehaviour : MonoBehaviour
{
    Crepuscular crepuscular;
    List<Light> recordedLightsInScene = new List<Light>();


    public static CrepuscularSceneBehaviour Instance { get; private set; }


    private void Reset()
    {
        crepuscular = GetComponent<Crepuscular>();
        if (crepuscular && !crepuscular.light)
            crepuscular.enabled = false;
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        crepuscular = GetComponent<Crepuscular>();
        TryTurnOff();
    }

    private void OnLevelWasLoaded(int level)
    {
        TryTurnOff();
    }


    void TryTurnOff()
    {
        if (!HasCrepuscularLightsInScene())
            crepuscular.enabled = false;
    }

    bool HasCrepuscularLightsInScene()
    {
        recordedLightsInScene.RemoveAll(light => light == null);
        return recordedLightsInScene.Count > 0;
    }

    public void AddCrepuscularLight(Light light)
    {
        if (light != null)
        {
            recordedLightsInScene.Add(light);
            crepuscular.enabled = true;
            crepuscular.light = light.gameObject;
        }
    }
}
