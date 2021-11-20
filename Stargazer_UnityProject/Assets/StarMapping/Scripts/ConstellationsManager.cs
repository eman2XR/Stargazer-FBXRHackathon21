using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ConstellationsManager : MonoBehaviour
{
    public GameObject nameTagPrefab;

    private void Start()
    {
        CreateNameTags();
    }

    void CreateNameTags()
    {
        foreach (Transform trans in GetComponentInChildren<Transform>())
        {
            if (trans.GetComponent<Renderer>())
            {
                GameObject nameTag = GameObject.Instantiate(nameTagPrefab, trans);
                Vector3 direction = trans.transform.position - Camera.main.transform.position;
                nameTag.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = trans.name;
                nameTag.transform.rotation = Quaternion.LookRotation(direction);
                nameTag.transform.position = trans.position;
                nameTag.transform.parent = trans;
            }
        }
    }
}
