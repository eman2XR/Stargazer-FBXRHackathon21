using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppManager : MonoBehaviour
{
    public Transform room;
    public Material maskMat;

    public void Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    [ContextMenu("room setup done")]
    public void RoomSetupDone()
    {
        foreach (Transform trans in room.GetComponentsInChildren<Transform>())
        {
            if (trans.GetComponent<Renderer>() && trans.name != "edge")
            {
                trans.GetComponent<Renderer>().material = maskMat;
            }
        }
    }
}
