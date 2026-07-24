using UnityEngine;
using System.Collections.Generic;




public class EnemyPool : MonoBehaviour
{
    public static EnemyPool instance;

    [SerializeField] private List<GameObject> enemyList = new List<GameObject>();

    [SerializeField] private int poolSize;
    private Dictionary<string, Queue<GameObject>> poolList = new Dictionary<string, Queue<GameObject>>();

    
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
        

        foreach (GameObject obj in enemyList)
        {
            

            poolList[obj.name] = new Queue<GameObject>();
            GameObject parentPool = new GameObject($"{obj.name}_Pool");
            parentPool.transform.SetParent(this.transform);

            for (int i = 0; i < poolSize; i++)
            {
                GameObject poolObject = Instantiate(obj, parentPool.transform);
                Enemy enemy = poolObject.GetComponent<Enemy>();
                enemy.SetPoolId(obj.name);
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
            GameObject poolObject = Instantiate(enemyList.Find(obj => obj.name == name));
            return poolObject;
        }


    }

    public void ReturnObject(string poolId, GameObject poolObject)
    {
        if (poolList.ContainsKey(poolId)== false)
        {
            Debug.Log("enemyPool return Error");
            Destroy(poolObject);
            return;
        }
        poolObject.SetActive(false);
        poolList[poolId].Enqueue(poolObject);
    }




}