using UnityEngine;
using UnityEngine.UI;

public class TimeDisplay : MonoBehaviour
{
    [SerializeField]
    Text hoursDisplay, minutesDisplay, secondsDisplay;

    TimeControl.TimeData Time => TimeControl.Time;


    private void Update()
    {
        UpdateDisplays();
    }


    void UpdateDisplays()
    {
        if (hoursDisplay)
            hoursDisplay.text = Time.hours.ToString("00");
        if (minutesDisplay)
            minutesDisplay.text = Time.minutes.ToString("00");
        if (secondsDisplay)
            secondsDisplay.text = Time.seconds.ToString("00");
    }
}