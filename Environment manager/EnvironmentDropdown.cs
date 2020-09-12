using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnvironmentDropdown : MonoBehaviour
{
    Dropdown dropdown;
    public string placeholder = "Environnement...";


    private void Awake()
    {
        dropdown = GetComponent<Dropdown>();
        dropdown.onValueChanged.AddListener(TrySendSelection);
    }

    private void OnEnable()
    {
        if (!EnvironmentManager.Instance)
            this.WaitUntil(() => EnvironmentManager.Instance, () =>
            {
                UpdateOptions();
            });
        else
            UpdateOptions();
    }


    void UpdateOptions()
    {
        dropdown.ClearOptions();
        dropdown.AddOptions(new List<Dropdown.OptionData>() { new Dropdown.OptionData() { text = placeholder } });

        // Add an option for each scene.
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        for (int i = 0; i < EnvironmentManager.Instance.EnvironmentsInBuild.Length; i++)
            options.Add(new Dropdown.OptionData() { text = Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(EnvironmentManager.Instance.EnvironmentsInBuild[i])) });
        dropdown.AddOptions(options);
    }

    void TrySendSelection(int selection)
    {
        EnvironmentManager.Instance.TrySelectEnvironment(selection - 1);
    }
}
