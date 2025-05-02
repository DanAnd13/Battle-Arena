using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    private ObjectPool _sharedInstance;
    private List<GameObject> _pooledObjects;

    void Awake()
    {
        _sharedInstance = this;
    }

    private void Start()
    {
        WriteObjectInPool();
    }

    private void WriteObjectInPool()
    {
        _pooledObjects = new List<GameObject>();
        GameObject tmp;
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            tmp = gameObject.transform.GetChild(i).gameObject;
            tmp.SetActive(false);
            _pooledObjects.Add(tmp);
        }
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            if (!_pooledObjects[i].activeInHierarchy)
            {
                return _pooledObjects[i];
            }
        }
        return null;
    }
}
