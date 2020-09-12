using UnityEngine;

public class ScenarioStarter : MonoBehaviour, IOpenClose
{
    [SerializeField]
    OpenCloseProperty openClose;

    UserScenarioState currentScenarioState;
    public event System.Action<string> ScenarioLoaded;

    public static ScenarioStarter Instance { get; private set; }


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ScenarioClient.DatabaseScenario.OnValueChanged += HandleNewDatabaseScenario;
    }

    void OnEnable()
    {
        // Stop saying what our scenario is, and start getting the scenario from the database.
        StopScenarioSignal();
        StartScenarioGetLoop();
    }

    void OnDisable()
    {
        // Stop getting the scenario from the database, and start saying what our scenario is.
        StopScenarioGetLoop();
        if (!string.IsNullOrEmpty(currentScenarioState.currentScenario))
            StartScenarioSignal();
    }


    #region Scenario stream --------------
    void StartScenarioGetLoop()
    {
        // Reset the scenario, then start checking again for a scenario.
        ScenarioClient.Instance.ResetDatabaseScenario(delegate
        {
            if (Login.CurrentUser != null && !ScenarioClient.Instance.GetLoopIsRunning)
                ScenarioClient.Instance.StartGetLoop(() => Login.CurrentUser);
        });
    }

    void StopScenarioGetLoop()
    {
        ScenarioClient.Instance.StopGetLoop();
    }

    void StartScenarioSignal()
    {
        ScenarioClient.Instance.StartSignal(() => currentScenarioState);
    }

    void StopScenarioSignal()
    {
        ScenarioClient.Instance.StopSignal();
    }

    void HandleNewDatabaseScenario(UserScenarioState dBScenario)
    {
        // Start a new scenario if the database has a scenario to load.
        if (string.IsNullOrEmpty(dBScenario.currentScenario))
            return;
        currentScenarioState = dBScenario;
        StartScenario(dBScenario.currentScenario);
    }
    #endregion ----------------------------

    public void StartScenario(string scenario)
    {
        // Update our scenario's user.
        if (currentScenarioState == null || Login.CurrentUser != currentScenarioState.user)
            currentScenarioState = new UserScenarioState(scenario, Login.CurrentUser);

        // Set our scenario, and stream that we are using this scenario.
        currentScenarioState.currentScenario = scenario;

        // Close, then tell that a scenario was loaded.
        Close();
        ScenarioLoaded?.Invoke(currentScenarioState.currentScenario);
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
