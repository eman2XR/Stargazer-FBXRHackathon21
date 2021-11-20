using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ConstellationsManager : MonoBehaviour
{
    public GameObject nameTagPrefab;

    IEnumerator Start()
    {
        //wait for starts to move
        yield return new WaitForSeconds(2);
        foreach (Transform trans in GetComponentInChildren<Transform>())
        {
            if (trans.GetComponent<Renderer>())
            {
                trans.GetChild(0).eulerAngles = new Vector3(trans.GetChild(0).eulerAngles.x, trans.GetChild(0).eulerAngles.y, 0);
            }
        }
    }


    [ContextMenu("create name tags")]
    void CreateNameTags()
    {
        foreach (Transform trans in GetComponentInChildren<Transform>())
        {
            if (trans.GetComponent<Renderer>())
            {
                //DestroyImmediate(trans.GetChild(0).gameObject);
                //GameObject nameTag = GameObject.Instantiate(nameTagPrefab, trans);
                //Vector3 direction = trans.transform.position - Camera.main.transform.position;
                //nameTag.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = trans.name;
                //nameTag.transform.rotation = Quaternion.LookRotation(direction);
                //nameTag.transform.position = trans.position;
                //nameTag.transform.parent = trans;
            }
        }
    }
}
