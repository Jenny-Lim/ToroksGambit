using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegendUI : MonoBehaviour
{

    [SerializeField] private GameObject LegendObject;

    [SerializeField] private GameObject[] Tutorial;

    public static LegendUI instance;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    public void DisableLegendObject()
    {
        LegendObject.SetActive(false);
        if (GameStateManager.instance.currentLevelNumber < Tutorial.Length)
        {
            Tutorial[GameStateManager.instance.currentLevelNumber].SetActive(false);
        }
    }

    public void ActivateLegendObject()
    {
        LegendObject.SetActive(true);
        if (GameStateManager.instance.currentLevelNumber < Tutorial.Length)
        {
            Tutorial[GameStateManager.instance.currentLevelNumber].SetActive(true);
        }
    }

}
