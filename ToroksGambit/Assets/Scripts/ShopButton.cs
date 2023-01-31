using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopButton : MonoBehaviour // started
{

    private string[] pieceTypes = { "pawn", "knight", "bishop", "rook", "queen", "king" };
    private int price;
    private int quantity;
    private int type;

    private TextMeshProUGUI priceText;


    // Start is called before the first frame update
    void Start()
    {
        type = 0;
        price = 0;
        priceText = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        priceText.text = price.ToString();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void BuyPiece() // on click
    {
        priceText.text = "SOLD OUT";
        gameObject.GetComponent<Button>().enabled = false;
        //Inventory.AlterPiece((Inventory.InventoryPieces)type, quantity);
    }
}
