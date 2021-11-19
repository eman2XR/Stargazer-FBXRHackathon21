using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstellationsManager : MonoBehaviour
{
    public GameObject nameTagPrefab;

    private void Start()
    {
        
    }

    void CreateNameTags()
    {
        foreach (Transform trans in GetComponentInChildren<Transform>())
        {
            if (trans.GetComponent<Renderer>())
            {
                GameObject nameTag = GameObject.Instantiate(nameTagPrefab, trans);
            }
        }
    }
}
