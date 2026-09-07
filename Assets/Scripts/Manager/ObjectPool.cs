using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;

    [SerializeField] List<GameObject> objList = new List<GameObject>();
  
    
    private Dictionary<string, Queue<GameObject>> poolList = new Dictionary<string, Queue<GameObject>>();

    int poolSize;
    //private Queue<GameObject> EnemyPool = new Queue<GameObject>();
    //private Queue<GameObject> BulletPool = new Queue<GameObject>();




    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else Destroy(gameObject);
        
        
    }

    void Start()
    {
        poolSize = 20;

        foreach(GameObject obj in objList)
        {
            poolList[obj.name] = new Queue<GameObject>();
            GameObject parentPool = new GameObject($"{obj.name}_Pool");
            parentPool.transform.SetParent(this.transform);

            for (int i = 0; i < poolSize; i++)
            {
                GameObject poolObject = Instantiate(obj, parentPool.transform);
                poolObject.SetActive(false);
                poolList[obj.name].Enqueue(poolObject);

            }
        }


    }

    public GameObject GetObject(string name)
    {
        if (!poolList.ContainsKey(name))
        {
            return null;
        }

        if (poolList[name].Count > 0)
        {
            GameObject poolObject = poolList[name].Dequeue();
            poolObject.SetActive(true);
            return poolObject;
        }
        else
        {
            GameObject poolObject = Instantiate(objList.Find(obj => obj.name == name));
            return poolObject;
        }


    }

    public void ReturnObject(string name, GameObject poolObject)
    {
        if (!poolList.ContainsKey(name))
        {
            Destroy(poolObject);
            return;
        }
        poolObject.SetActive(false);
        poolList[name].Enqueue(poolObject);
    }


    

}