using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ParticleEffects
{
    Smoke
}

public class ParticleEffectObjectPool : MonoBehaviour
{
    public static ParticleEffectObjectPool instance;
    public GameObject[] prefabs;
    private List<List<GameObject>> objectPools;
    public const int POOL_SIZE = 5;
    


    private void Awake()
    {
        if (instance == null) { instance = this; }
        Initialize();
    }

    private void Initialize()
    {
        Debug.Log("inside init");
        int enumLength = Enum.GetNames(typeof(ParticleEffects)).Length;
        objectPools = new List<List<GameObject>>(enumLength);

        for (int i = 0; i < enumLength; i++)
        {
            objectPools.Add(new List<GameObject>());
            for (int j = 0; j < POOL_SIZE; j++)
            {

                GameObject GO = Instantiate(prefabs[i], new Vector3(0, -50, 0), Quaternion.identity, transform);
                objectPools[i].Add(GO);
                GO.SetActive(false);
            }
        }
        
    }

    public ParticlePoolParticle GetParticleEffectGO(ParticleEffects particleType)
    {
        if (objectPools[(int)particleType].Count < 1)
        {
            Debug.LogWarning("ParticleOP| tried to get particle from list of size 0 or less");
            return null;
        }
        
        for (int i = 0; i < POOL_SIZE; i++)
        {
            if (!objectPools[(int)particleType][i].activeInHierarchy) {
                return objectPools[(int)particleType][i].GetComponent<ParticlePoolParticle>();
            }
        
        }

        Debug.LogWarning("Couldnt find unused object in pool category " + (int)particleType + ", try increasing pool size");
        return null;

    }
}


