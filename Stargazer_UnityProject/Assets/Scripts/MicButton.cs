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
    public Transform head;
    public float distanceToActivate;

    private void Update()
    {
        if(Vector3.Distance(transform.position, head.position) < distanceToActivate)
        {
            if (!isOn)
                TurnMicOn();
        }
    }

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
        isOn = true;
        onMicOn.Invoke();
        micOnVisual.SetActive(true);
        micOffVisual.SetActive(false);
    }

    public void TurnMicOff()
    {
        isOn = false;
        micOnVisual.SetActive(false);
        micOffVisual.SetActive(true);
    }
}
