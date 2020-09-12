using UnityEngine;
using UnityEngine.UI;

public class IdeaButton : MonoBehaviour
{
    public Text output;


    private void Reset()
    {
        GetComponent<CustomMainMenuButton>().OnClick.AddPersistentEvent(this, SendIdea);
        output = transform.Find("Output").GetComponent<Text>();
    }

    private void Start()
    {
        HideOutput();
    }


    public void SendIdea()
    {
        Output(ExpeIdeaManager.Instance.TrySendIdea());
    }

    void Output(FrontEndLog log)
    {
        output.text = log.GetStylizedMessage();
        StopAllCoroutines();
        output.Flash(this, log.GetColor() * 1.2f, log.type == FrontEndLog.LogType.Sucess ? 1.5f : 4);
    }

    void HideOutput()
    {
        output.color = Color.clear;
    }
}
