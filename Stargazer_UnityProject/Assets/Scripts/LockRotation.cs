using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockRotation : MonoBehaviour
{
    Quaternion startRotation;

    private void Start()
    {
        startRotation = this.transform.rotation;  
    }

    void Update()
    {
        this.transform.rotation = startRotation; 
    }
}
