using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Start a loop to check on the database scenario over time.
/// If a scenario is loaded, send its value to the database over time to signal that it is playing.
/// </summary>
public class ScenarioClient : MonoBehaviour
{
    float lastGetRequestTime;
    [SerializeField]
    [Tooltip("Delay between each request asking for the database's value.")]
    float getLoopDelay = 1;
    public static ValueHolder<UserScenarioState> DatabaseScenario { get; } = new ValueHolder<UserScenarioState>();
    public bool GetLoopIsRunning => runningGetLoop.IsRunning();
    public bool SignalIsRunning => runningSignal.IsRunning();
    [SerializeField]
    [Tooltip("Delay between each request telling the database about our value.")]
    float signalDelay = 3;

    public static ScenarioClient Instance { get; private set; }


    private void Awake()
    {
        Instance = this;
    }

    void OnDisable()
    {
        runningGetLoop = null;
        runningSignal = null;
    }


    #region Get scenario loop ----------------
    public void StartGetLoop(Func<QuickUser> UserGetter)
    {
        this.StopAndEmptyCoroutine(ref runningGetLoop);
        runningGetLoop = StartCoroutine(ScenarioGetLoop(UserGetter));
    }

    public void StopGetLoop()
    {
        this.StopAndEmptyCoroutine(ref runningGetLoop);
    }

    Coroutine runningGetLoop;
    IEnumerator ScenarioGetLoop(Func<QuickUser> UserGetter)
    {
        lastGetRequestTime = Time.time;
        while (true)
        {
            UserScenarioState responseScenario = null;
            bool errorReceived = false;
            lastGetRequestTime = Time.time;

            Database.GetUserScenarioState(
                UserGetter(),
                (result) => responseScenario = result,
                delegate { errorReceived = true; });

            while (responseScenario == null && !errorReceived)
                yield return null;

            if (errorReceived)
                ResetDatabaseScenario();
            else
                HandleDatabaseScenario(responseScenario, UserGetter());

            yield return new WaitForSeconds(getLoopDelay - (Time.time - lastGetRequestTime));
        }
    }

    #region Handling the database scenario -------------------
    void HandleDatabaseScenario(UserScenarioState loadedScenario, QuickUser user)
    {
        if (string.IsNullOrEmpty(loadedScenario.currentScenario))
            return;

        // If the scenario state from the database is too old, reset it and stop.
        DateTime parsedDate = JavascriptDateUtility.ToDateTime(loadedScenario.startDate);
        if ((DateTime.Now - parsedDate).TotalMinutes > 1)
        {
            ResetDatabaseScenario();
            return;
        }

        loadedScenario.user = user;
        DatabaseScenario.Value = loadedScenario;
        return;
    }

    public void ResetDatabaseScenario(Action SuccessAction = null)
    {
        Database.SetUserScenario(new UserScenarioState("", Login.CurrentUser));
        DatabaseScenario.Value = new UserScenarioState("", Login.CurrentUser);

        if (SuccessAction != null)
            StartCoroutine(ResetScenarioConfirmation(SuccessAction));
    }

    IEnumerator ResetScenarioConfirmation(Action SuccessAction)
    {
        // Call SuccessAction when we accessed an empty scenario or no scenario.
        bool success = false;
        while (!success)
        {
            bool hasResponse = false;
            Database.GetUserScenarioState(Login.CurrentUser, delegate (UserScenarioState state)
            {
                if (string.IsNullOrEmpty(state.currentScenario))
                    success = true;
                hasResponse = true;
            }, delegate
            {
                success = true;
                hasResponse = true;
            });
            while (!hasResponse)
                yield return null;
        }
        SuccessAction();
    }
    #endregion ---------------------------------------------------
    #endregion --------------------------------

    #region Scenario signal -------------------
    public void StartSignal(Func<UserScenarioState> ScenarioGetter)
    {
        this.StopAndEmptyCoroutine(ref runningSignal);
        runningSignal = StartCoroutine(ScenarioSignal(ScenarioGetter));
    }

    public void StopSignal()
    {
        if (runningSignal != null)
            StopCoroutine(runningSignal);
    }

    Coroutine runningSignal;
    IEnumerator ScenarioSignal(Func<UserScenarioState> ScenarioGetter)
    {
        while (true)
        {
            ScenarioGetter().lastUpdateDate = JavascriptDateUtility.ToJavascript(DateTime.Now);
            Database.SetUserScenario(ScenarioGetter());
            yield return new WaitForSeconds(signalDelay);
        }
    }
    #endregion ---------------------------------
}
