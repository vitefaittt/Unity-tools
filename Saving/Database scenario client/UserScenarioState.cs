using System;

public class UserScenarioState
{
    public string currentScenario;
    public string startDate;
    public string lastUpdateDate;
    public QuickUser user;

    public UserScenarioState(string inCurrentScenario, QuickUser inUser)
    {
        currentScenario = inCurrentScenario;
        startDate = JavascriptDateUtility.ToJavascript(DateTime.Now);
        user = inUser;
    }
}