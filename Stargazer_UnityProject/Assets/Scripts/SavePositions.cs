using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePositions : MonoBehaviour
{
    private void Start()
    {
        foreach (Transform trans in GetComponentsInChildren<Transform>())
        {
           if(PlayerPrefs.HasKey(trans.name + "x"))
            {
                Vector3 savePos = new Vector3(PlayerPrefs.GetFloat(trans.name + "x"), PlayerPrefs.GetFloat(trans.name + "y"), PlayerPrefs.GetFloat(trans.name + "z"));
                trans.position = savePos;
            }
        }

        InvokeRepeating("SavePositionsOfWalls", 10, 5);
    }
    void SavePositionsOfWalls()
    {
        foreach(Transform trans in GetComponentsInChildren<Transform>())
        {
            PlayerPrefs.SetFloat(trans.name + "x", trans.position.x);
            PlayerPrefs.SetFloat(trans.name + "y", trans.position.y);
            PlayerPrefs.SetFloat(trans.name + "z", trans.position.z);
        }
    }
}
