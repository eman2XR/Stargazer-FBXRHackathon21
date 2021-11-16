using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    Vector3 targetPosition;

    void Update()
    {
        if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickUp) || OVRInput.Get(OVRInput.Button.SecondaryThumbstickUp))
        {
            targetPosition = Camera.main.transform.forward / 100;
            this.transform.position += new Vector3(targetPosition.x, 0, targetPosition.z);
        }
        else if(OVRInput.Get(OVRInput.Button.PrimaryThumbstickDown) || OVRInput.Get(OVRInput.Button.SecondaryThumbstickDown))
        {
            targetPosition = -(Camera.main.transform.forward / 100);
            this.transform.position += new Vector3(targetPosition.x, 0, targetPosition.z);
        }
        else if(OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight) || OVRInput.Get(OVRInput.Button.SecondaryThumbstickRight))
        {
            targetPosition = (Camera.main.transform.right / 100);
            this.transform.position += new Vector3(targetPosition.x, 0, targetPosition.z);
        }
        else if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft) || OVRInput.Get(OVRInput.Button.SecondaryThumbstickLeft))
        {
            targetPosition = -(Camera.main.transform.right / 100);
            this.transform.position += new Vector3(targetPosition.x, 0, targetPosition.z);
        }

    }
}
