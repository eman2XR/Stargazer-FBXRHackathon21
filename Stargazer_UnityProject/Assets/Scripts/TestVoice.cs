using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TestVoice : MonoBehaviour
{
    public UnityEvent buttonPressed;

    //void Update()
    //{
    //    if(OVRInput.GetDown(OVRInput.Button.Any) || Input.GetKeyDown("l"))
    //    {
    //        buttonPressed.Invoke();
    //    }
    //}

    public void ButtonPressed()
    {
        buttonPressed.Invoke();
    }
}
