using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomMapperExtention : MonoBehaviour
{
    public Material maskMat;

    //IEnumerator Start()
    //{
    //    yield return new WaitForSeconds(1);
    //    RoomComplete();
    //}

    public void RoomComplete()
    {
        foreach(Transform trans in GameObject.Find("(RoomAnchor)").GetComponentsInChildren<Transform>())
        {
            if (trans.name == ("(Walls)"))
                trans.GetComponent<Renderer>().material = maskMat;
            else if (trans.name == ("(Ceiling)"))
                trans.gameObject.SetActive(false);
            else if (trans.name == ("(Floor)"))
                trans.gameObject.SetActive(false);
        }
    }
}
