using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : Singleton<ObjectPooler> {

  Dictionary<int, ObjectPoolItem> poolDictionary = new Dictionary<int, ObjectPoolItem>();

  public void CreatePool(GameObject prefab, int amountToPool, bool expandable = false) {
    int poolKey = prefab.GetInstanceID();

    GameObject poolHolder = new GameObject(prefab.name +" pool");
    poolHolder.transform.parent = transform;

    if (!poolDictionary.ContainsKey(poolKey)) {
      poolDictionary.Add(poolKey, new ObjectPoolItem(poolHolder.transform, expandable));
      ObjectPoolItem poolItem = poolDictionary[poolKey];

      for (int i = 0; i < amountToPool; i++) {
        GameObject newObject = Instantiate(prefab) as GameObject;
        poolItem.AddObjectToList(newObject);
      }
    }
  }

  public GameObject GetPooledObject(GameObject prefab) {
    int poolKey = prefab.GetInstanceID();

    if (poolDictionary.ContainsKey(poolKey)) {
      ObjectPoolItem poolItem = poolDictionary[poolKey];
      
      int i = poolItem.lastGetIndex;
      do {
        if (!poolItem.poolList[i].activeInHierarchy) {
          poolItem.lastGetIndex = i;
          return poolItem.poolList[i];
        }
        i = (i + 1) % poolItem.poolList.Count;
      } while (i != poolItem.lastGetIndex);

      if (poolItem.expandable) {
        GameObject newObject = Instantiate(prefab) as GameObject;
        poolItem.AddObjectToList(newObject);
        return newObject;
      }
    }
    return null;
  }
}

public class ObjectPoolItem {
  public List<GameObject> poolList;
  public Transform parent;
  public int lastGetIndex;
  public bool expandable;

  public ObjectPoolItem(Transform parent, bool expandable) {
    poolList = new List<GameObject>();
    lastGetIndex = 0;
    this.parent = parent;
    this.expandable = expandable;
  }

  public void AddObjectToList(GameObject prefab) {
    prefab.SetActive(false);
    prefab.transform.parent = parent;
    poolList.Add(prefab);
    lastGetIndex = (poolList.Count - 1);
  }
}