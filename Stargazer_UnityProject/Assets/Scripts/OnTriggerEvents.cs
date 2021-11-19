using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnTriggerEvents : MonoBehaviour
{
    public string colliderTag;
    public UnityEvent onTriggerEnter;
    bool hasBeenTriggered;
    bool waited = true;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == colliderTag && waited)
        {
            StartCoroutine(DelayRetrigger());
            hasBeenTriggered = true;
            onTriggerEnter.Invoke();
        }
    }

    IEnumerator DelayRetrigger()
    {
        waited = false;
        yield return new WaitForSeconds(2);
        waited = true;
    }

}
