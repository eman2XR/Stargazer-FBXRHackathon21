using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MicButton : MonoBehaviour
{
    bool isOn;
    public GameObject micOnVisual;
    public GameObject micOffVisual;
    public UnityEvent onMicOn;

    public void ToggleMic()
    {
        if(isOn)
        {
            //turn off
            micOnVisual.SetActive(false);
            micOffVisual.SetActive(true);
        }
        else
        {
            //turn on
            onMicOn.Invoke();
            micOnVisual.SetActive(true);
            micOffVisual.SetActive(false);
        }
    }

    public void TurnMicOn()
    {
        onMicOn.Invoke();
        micOnVisual.SetActive(true);
        micOffVisual.SetActive(false);
    }

    public void TurnMicOff()
    {
        micOnVisual.SetActive(false);
        micOffVisual.SetActive(true);
    }
}
