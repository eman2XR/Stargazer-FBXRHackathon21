using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Move : MonoBehaviour
{
    public float speed;
    Vector3 forward = Vector3.forward;
    public List<GameObject> Tiles = new List<GameObject>();
    private float zPos = 0;
    private int starTiles = 2;
    private int prevRand = 0;
    private int num = 0;


    void Start()
    {
        for (int i = 0; i < Tiles.Count; i++)
        {
            PoolManager.Instance.CreatePool(Tiles[i], 2);
        }
        for (int i = 0; i < starTiles; i++)
        {
            while (prevRand == num)
            {
                num = Tiles[Random.Range(0, Tiles.Count)].GetInstanceID();
            }
            PoolManager.Instance.Reuse(num, new Vector3(0, 0, zPos), Quaternion.identity);
            prevRand = num;
            zPos += 50;
        }
    }

    void Update()
    {
        if (transform.position.z + 100 > zPos)
        {
            while (prevRand == num)
            {
                num = Tiles[Random.Range(0, Tiles.Count)].GetInstanceID();
            }
            PoolManager.Instance.Reuse(num, new Vector3(0, 0, zPos), Quaternion.identity);
            zPos += 50;
            prevRand = num;
        }
        transform.position += Time.deltaTime * speed * forward;
    }
}
