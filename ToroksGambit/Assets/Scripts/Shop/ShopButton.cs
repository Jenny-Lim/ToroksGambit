using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopButton : MonoBehaviour
{

    //private int price;
    //private int type;
    public int price;
    public int type;
    //private int quantity;
    private TextMeshProUGUI priceText;
    private Button b;
    private PieceUI pUI;

    void Start()
    {
        priceText = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        priceText.text = price.ToString();
        b = gameObject.GetComponent<Button>();
        pUI = gameObject.GetComponentInParent(typeof(PieceUI)) as PieceUI;
        type = pUI.type;
    }

    public void BuyPiece() // on click
    {
        if (Currency.instance.tickets >= price) {
            Currency.instance.SubtractFromCurrency(price);
            priceText.text = "SOLD OUT";
            b.enabled = false;
            //pUI.PieceBought();
            pUI.isBought = true;
            Inventory.instance.AlterPiece((Inventory.InventoryPieces)type, 1);

            print(Inventory.instance.GetNumberOfPieces(0)); // testing --boi what the hell
        }
        else
        {
            print("u broke af");
        }
    }

    //public void SetPrice(int p)
    //{
    //    price = p;
    //}

    //public void SetType(int t)
    //{
    //    type = t;
    //}
}
