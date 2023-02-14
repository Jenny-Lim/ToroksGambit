using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PiecePhysical : MonoBehaviour
{
    [SerializeField] private Color newColor;
    [SerializeField] private GameObject[] pieceModels;
    public int type;
    public bool isBought;
    private int price;
    private Image img;
    private ShopButton b;
    private TextMeshProUGUI owned;

    public void Start()
    {
        img = gameObject.GetComponent<Image>();
        b = gameObject.GetComponentInChildren<ShopButton>();
        owned = gameObject.GetComponentInChildren<TextMeshProUGUI>();

        price = 100;

        for (int i = 0; i < 5; i++) // no king
        {
            if (type == i)
            {
                //img.sprite = s[i];
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


        owned.text = "Owned: " + Inventory.instance.GetNumberOfPieces((Inventory.InventoryPieces)type).ToString();
    }

    void Update()
    {
        owned.text = "Owned: " + Inventory.instance.GetNumberOfPieces((Inventory.InventoryPieces)type).ToString();
        if (isBought)
        {
            owned.fontStyle = FontStyles.Strikethrough; // doesnt update disabled pieces for now, so strikethru ig
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
