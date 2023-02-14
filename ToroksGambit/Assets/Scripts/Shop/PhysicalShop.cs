using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PhysicalShop : MonoBehaviour
{
    public static PhysicalShop instance; 

    [SerializeField] private Transform[] pieceSpots;
    [SerializeField] private GameObject[] pieceModels;
    [SerializeField] private int[] prices;
    [SerializeField] private TextMeshPro[] priceText;
    [SerializeField] public GameObject[] uiSpots;

    [SerializeField] private CameraHeadMovements c;

    private GameObject[] shopPieceModels;
    private int[] pieceType;

    private void Start()
    {
        pieceType = new int[8];
        shopPieceModels = new GameObject[8];

        InitializeShop();
    }

    public void InitializeShop()
    {
        for (int i = 0; i < pieceSpots.Length; i++)
        {
            pieceType[i] = (int)(Random.Range(0, 4));
            Debug.Log("piecetype" + i + " is " + pieceType[i]);

            //place model at spot
            shopPieceModels[i] = Instantiate(pieceModels[pieceType[i]], pieceSpots[i].position, Quaternion.identity, gameObject.transform);
            //change price
            priceText[i].text = prices[pieceType[i]].ToString();
            //re activate panels
            uiSpots[i].SetActive(true);
        }
    }

    public void EnterShop()
    {
        c.LookAtShop();
        for(int i = 0; i < shopPieceModels.Length; i++)
        {
            uiSpots[i].SetActive(true);
        }

    }

    public void PanelPressed(GameObject buttonObject)
    {
        for(int i = 0;i < uiSpots.Length;i++)
        {
            if(buttonObject == uiSpots[i])
            {
                Debug.Log(i);
                Destroy(shopPieceModels[i]);
                priceText[i].text = "SOLD OUT";
                //add to inventory

            }
        }

        buttonObject.SetActive(false);
    }

    //void Update()
    //{

    //}

}
