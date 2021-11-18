using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{

    public static PoolManager Instance { get; set; }
    Dictionary<int, Queue<GameObject>> pool = new Dictionary<int, Queue<GameObject>>();
    Camera cam;

    public void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    public void CreatePool(GameObject gameObj, int numberOfObj)
    {
        int gameObjKey = gameObj.GetInstanceID();
        if (!pool.ContainsKey(gameObjKey))
        {
            pool.Add(gameObjKey, new Queue<GameObject>());
            for (int i = 0; i < numberOfObj; i++)
            {
                var go = Instantiate(gameObj) as GameObject;
                go.SetActive(false);
                pool[gameObjKey].Enqueue(go);
            }
        }
    }
    public void Reuse(int gameObjKey, Vector3 position, Quaternion orientation)
    {
        GameObject go = pool[gameObjKey].Dequeue();
        go.SetActive(true);
        go.transform.position = position;
        go.transform.rotation = orientation;
        pool[gameObjKey].Enqueue(go);
    }
    public void Reuse(int gameObjKey)
    {
        GameObject go = pool[gameObjKey].Dequeue();
        go.SetActive(true);
        pool[gameObjKey].Enqueue(go);
    }

    private void Update()
    {
        foreach (var item in pool)
        {
            foreach (var element in item.Value)
            {
                if (element.transform.position.z + 25f < cam.transform.position.z)
                {
                    element.SetActive(false);
                }
            }
        }
    }
}
