using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour // started smth, got sleepy bruh
{

    private string[] pieceTypes = { "pawn", "knight", "bishop", "rook", "queen", "king" };
    private int price;
    private int quantity;
    private int type;

    private Text priceText;


    // Start is called before the first frame update
    void Start()
    {
        type = 1;
        gameObject.GetComponentInChildren<Text>().text = pieceTypes[type];
        price = 0;
        

        //priceText.text = this.price.ToString();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void BuyPiece() // on click
    {
        print("ayo");
        //Inventory.AlterPiece(type, quantity);
    }
}
