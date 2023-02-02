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
    private int quantity;
    private TextMeshProUGUI priceText;
    private Button b;
    private PieceUI pUI;

    void Start()
    {
        priceText = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        priceText.text = price.ToString();

        b = gameObject.GetComponent<Button>();
        pUI = gameObject.GetComponentInParent(typeof(PieceUI)) as PieceUI;
    }

    public void BuyPiece() // on click
    {
        priceText.text = "SOLD OUT";
        b.enabled = false;
        //pUI.PieceBought();
        pUI.isBought = true;
        //Inventory.AlterPiece((Inventory.instance.InventoryPieces)type, quantity);
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
