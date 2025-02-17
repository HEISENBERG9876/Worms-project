using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIScript : MonoBehaviour
{
    public GameObject menuCanvas;
    public TextMeshProUGUI timerText;

    public void ShowGameMenu()
    {
        menuCanvas.SetActive(true);
    }
    public void HideGameMenu()
    {
        menuCanvas.SetActive(false);
    }

    public void UpdateTimer(int secondsLeft)
    {
        timerText.text = secondsLeft.ToString();
    }

    public void HideTimer()
    {
        timerText.gameObject.SetActive(false);
    }

    public void ShowTimer()
    {
        timerText.gameObject.SetActive(true);
    }

    public IEnumerator StartTimer(int seconds)
    {
        ShowTimer();
        while (seconds != 0)
        {
            timerText.text = seconds.ToString();
            seconds--;
            yield return new WaitForSeconds(1.0f);
        }
        HideTimer();
        EventManager.TriggerTurnEnded();
    }
}