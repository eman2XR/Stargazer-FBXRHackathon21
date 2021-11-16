using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : MonoBehaviour
{
    public bool magnetic;

    private void Start()
    {
        Input.location.Start();
    }

    void Update()
    {
        if(magnetic)
            transform.rotation = Quaternion.Euler(0, -Input.compass.magneticHeading, 0);
        else
            // Orient an object to point northward.
            transform.rotation = Quaternion.Euler(0, -Input.compass.trueHeading, 0);
    }
}
