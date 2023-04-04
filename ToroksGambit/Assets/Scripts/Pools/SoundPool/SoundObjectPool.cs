using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundObjectPool : MonoBehaviour
{
    private static GameObject[] soundPool;
    [SerializeField] private int POOL_SIZE;
    [SerializeField] private GameObject prefab;
    public static SoundObjectPool instance;

    public void Awake()
    {
        if (instance == null) instance = this; 
        soundPool = new GameObject[POOL_SIZE];
        for (int i = 0; i < POOL_SIZE; i++)
        {
            soundPool[i] = Instantiate(prefab, transform);
            soundPool[i].SetActive(false);
        }
    }

    public SoundObject GetPoolObject()
    {
        for (int i = 0; i < POOL_SIZE; i++)
        {
            if (!soundPool[i].activeInHierarchy)
            {
                soundPool[i].SetActive(true);
                return soundPool[i].GetComponent<SoundObject>();
            }
        }
        Debug.Log("Didnt not find objetc pool object");
        return null;
    }

}
