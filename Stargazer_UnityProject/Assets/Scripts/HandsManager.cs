using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HandsManager : MonoBehaviour
{
    bool checkedHands;
    bool checkedControllers;
    public UnityEvent onHandsActive;
    public UnityEvent onControllersActive;

    void Update()
    {
        if (OVRInput.IsControllerConnected(OVRInput.Controller.Hands))
        {
            if (!checkedHands)
            {
                onHandsActive.Invoke();
                checkedHands = true;
                checkedControllers = false;
            }
        }
        else
        {
            if (!checkedControllers)
            {
                onControllersActive.Invoke();
                checkedControllers = true;
                checkedHands = false;
            }
        }
    }
}
