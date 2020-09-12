using UnityEngine;
using UnityEngine.SceneManagement;

public class EnvironmentManager : MonoBehaviour
{
    [SerializeField]
    int[] environmentsInBuild;
    public int CurrentEnvironment { get; private set; } = -1;
    public int[] EnvironmentsInBuild => environmentsInBuild;
    public event System.Action<int> EnvironmentChanged;
    public bool HasValidEnvironment => CurrentEnvironment >= 0 && CurrentEnvironment < environmentsInBuild.Length;
    public static EnvironmentManager Instance { get; private set; }


    private void Awake()
    {
        Instance = this;
    }


    public bool TrySelectEnvironment(int index)
    {
        if (CurrentEnvironment == index)
            return true;
        if (index < -1 || index > environmentsInBuild.Length - 1)
            return false;

        // Unload the previous environment.
        if (CurrentEnvironment != -1)
            SceneManager.UnloadSceneAsync(environmentsInBuild[CurrentEnvironment]);

        // Load the environment.
        CurrentEnvironment = index;
        if (CurrentEnvironment >= 0)
            SceneManager.LoadSceneAsync(environmentsInBuild[CurrentEnvironment], LoadSceneMode.Additive);
        EnvironmentChanged?.Invoke(CurrentEnvironment);
        return true;
    }

    public void UnloadEnvironment()
    {
        TrySelectEnvironment(-1);
    }
}
