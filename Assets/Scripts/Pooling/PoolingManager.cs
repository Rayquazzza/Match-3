using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour, IPoolingService
{

    private Dictionary<int, Queue<GameObject>> poolDictionary = new Dictionary<int, Queue<GameObject>>();

    [SerializeField] private List<PoolPrewarmConfig> objectsToPrewarm;


    /// <summary>
    /// Optional : Prepare the pool with a certain number of instances.
    /// </summary>
    public void Prewarm(GameObject prefab, int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            ReturnToPool(prefab, obj);
        }

        foreach (var item in objectsToPrewarm)
        {
            Prewarm(item.prefab, item.amount);
        }
    }


    private void Awake()
    {
        GameServiceLocator.Register<IPoolingService>(this);
    }

    //==============================================
    // GET POOLS AND RETURN TO POOL
    //==============================================


    /// <summary>
    /// Recover an object from the pool or create a new one if none are available.
    /// </summary>
    public GameObject GetFromPool(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        // Get the unique ID for the prefab
        int id = prefab.GetInstanceID();

        // 1. Initialize the pool for this prefab if it doesn't exist
        if (!poolDictionary.ContainsKey(id))
        {
            poolDictionary.Add(id, new Queue<GameObject>());
        }

        GameObject obj;

        // 2. Recover an object from the pool if available
        if (poolDictionary[id].Count > 0)
        {
            obj = poolDictionary[id].Dequeue();
        }
        else
        {
            obj = Instantiate(prefab);

            // We can add a PoolMember component to track the prefab reference
            PoolMember member = obj.AddComponent<PoolMember>();
            member.myPrefab = prefab;
        }

        // Configure and activate the object
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.SetActive(true);

        return obj;
    }

    /// <summary>
    /// Return an object to its pool.
    /// </summary>
    public void ReturnToPool(GameObject prefab, GameObject obj)
    {
        int id = prefab.GetInstanceID();
        obj.SetActive(false);
        poolDictionary[id].Enqueue(obj);
    }
}

[Serializable]
public class PoolPrewarmConfig
{
    public GameObject prefab;
    public int amount;
}
