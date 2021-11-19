using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarsManager : MonoBehaviour
{
    private void Start()
    {
        foreach (Transform trans in GetComponentInChildren<Transform>())
        {
            if (trans.GetComponent<GrabObject>())
            {
                trans.GetComponent<GrabObject>().ObjectName = trans.name;
            }
        }
    }
}