using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleObjectPool : MonoBehaviour
{
    private ParticleObjectPool _sharedInstance;
    private List<ParticleSystem> _pooledObjects;

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
        _pooledObjects = new List<ParticleSystem>();
        GameObject tmp;
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            tmp = gameObject.transform.GetChild(i).gameObject;
            ParticleSystem particle = tmp.GetComponent<ParticleSystem>();
            _pooledObjects.Add(particle);
        }
    }

    public ParticleSystem GetPooledObject()
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            if (!_pooledObjects[i].isPlaying)
            {
                return _pooledObjects[i];
            }
        }
        return null;
    }
}
