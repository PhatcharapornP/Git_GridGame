using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectPoolManager : MonoBehaviour
{
    [SerializeField] private List<Pool> pools = new List<Pool>(); 
    private Dictionary<Globals.PoolTag, Queue<GameObject>> poolDict = new Dictionary<Globals.PoolTag, Queue<GameObject>>();

    public void Initialize(UnityAction onFinishedSetupPools)
    {
        for (int p = 0; p < pools.Count; p++)
        {
            Queue<GameObject> objQueue = new Queue<GameObject>();
            for (int i = 0; i < pools[p].size; i++)
            {
                GameObject obj = Instantiate(pools[p].poolPrefab);
                if (pools[p].poolParent != null)
                    obj.transform.SetParent(pools[p].poolParent);
                obj.gameObject.name = obj.gameObject.name + i;
                obj.SetActive(false);
                
                if (obj.GetComponent<IPoolObject>()!= null)
                    obj.GetComponent<IPoolObject>().InitializePoolObj();
                
                objQueue.Enqueue(obj);
            }
            
            poolDict.Add(pools[p].tag,objQueue);
            if (p == pools.Count -1)
                onFinishedSetupPools?.Invoke();
        }
    }

    public GameObject PickFromPool(Globals.PoolTag tag)
    {
        if (!poolDict.ContainsKey(tag))
        {
            Debug.LogError($"pool with tag {tag} doesn't exit".InColor(Color.red));
            return null;
        }
    
        GameObject objToSpawn = poolDict[tag].Dequeue(); //Get the 1st gameobject in the Queue from pool [tag]
      
        objToSpawn.SetActive(true);
        
        
        
        poolDict[tag].Enqueue(objToSpawn); //Add objToSpawn back to the Queue as the last of the Queue again
        return objToSpawn;
    }
}

[Serializable]
public class Pool
{
    public Globals.PoolTag tag;
    public GameObject poolPrefab;
    public int size;
    public Transform poolParent;
}