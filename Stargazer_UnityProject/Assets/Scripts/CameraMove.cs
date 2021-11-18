using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    Vector3 targetPosition;

    void Update()
    {
        if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickUp))
        {
            targetPosition = Camera.main.transform.forward / 100;
            this.transform.position += new Vector3(targetPosition.x, 0, targetPosition.z);
        }
        else if(OVRInput.Get(OVRInput.Button.PrimaryThumbstickDown))
        {
            targetPosition = -(Camera.main.transform.forward / 100);
            this.transform.position += new Vector3(targetPosition.x, 0, targetPosition.z);
        }
        else if(OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight))
        {
            targetPosition = (Camera.main.transform.right / 100);
            this.transform.position += new Vector3(targetPosition.x, 0, targetPosition.z);
        }
        else if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft) )
        {
            targetPosition = -(Camera.main.transform.right / 100);
            this.transform.position += new Vector3(targetPosition.x, 0, targetPosition.z);
        }

        else if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickLeft))
        {
            targetPosition = -(Camera.main.transform.up / 100);
            this.transform.position += new Vector3(0, targetPosition.y, 0);
        }
        else if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickRight))
        {
            targetPosition = (Camera.main.transform.up / 100);
            this.transform.position += new Vector3(0, targetPosition.y, 0);
        }

    }
}
