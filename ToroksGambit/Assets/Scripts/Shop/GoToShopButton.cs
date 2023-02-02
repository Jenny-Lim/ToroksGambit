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

    private bool canShop;

    void Start()
    {
        t = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        //t.text = "Enter Shop";
    }

    public void GoToShop()
    {
        if (canShop)
        {
            t.text = "Enter Shop";
            canShop = false; // can't go to shop if you're already there
            
        }
        else
        {
            t.text = "Leave Shop";
            canShop = true;
        }

        shop.SetActive(canShop);
    }
}
