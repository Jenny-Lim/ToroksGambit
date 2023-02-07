using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PieceUI : MonoBehaviour 
{
    [SerializeField] private Color newColor;
    [SerializeField] private Sprite[] s;
    public int type;
    public bool isBought;
    private int price;
    private Image img;
    private ShopButton b;

    public void Start()
    {
        img = gameObject.GetComponent<Image>();
        b = gameObject.GetComponentInChildren<ShopButton>();

        price = 100;

        for (int i = 0; i < 5; i++) // no king
        {
            if (type == i)
            {
                img.sprite = s[i];
                break;
            }
            else
            {
                price = price + 100;
            }
        }

        //b.SetPrice(price);
        //b.SetType(type);

        b.price = price;
        b.type = type;
    }

    void Update()
    {
        if (isBought)
        {
            img.color = newColor;
            this.enabled = false;
        }
    }

    //public void SetType(int t)
    //{
    //    type = t;
    //}

    //public void PieceBought()
    //{
    //    isBought = true;
    //}
}
