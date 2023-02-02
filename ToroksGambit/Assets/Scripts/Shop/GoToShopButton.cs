using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GoToShopButton : MonoBehaviour
{
    [SerializeField]
    private GameObject shop;

    private TextMeshProUGUI t;
    private Button b;

    private bool inShop;

    void Start()
    {
        t = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        t.text = "Go To Shop";
    }

    public void GoToShop()
    {
        if (inShop)
        {
            t.text = "Enter Shop";
            inShop = false;
            
        }
        else
        {
            t.text = "Leave Shop";
            inShop = true;
        }

        shop.SetActive(inShop);
    }
}
