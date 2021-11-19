using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarMapManager : MonoBehaviour
{
    IEnumerator Start()
    {
        //wait for stars particles
        yield return new WaitForSeconds(1);
        //get time of day and compass orientationrotate star sphere accordingly
        //for now just rotate manually 
        this.transform.eulerAngles = new Vector3(90, 0, -66);
    }

}
