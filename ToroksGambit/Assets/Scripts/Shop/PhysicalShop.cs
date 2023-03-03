using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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

    [SerializeField] private GameObject piecePanels;

    [SerializeField] private CameraHeadMovements c;
    private Camera cam;

    private GameObject[] shopPieceModels;
    private int[] pieceType;

    private void Awake()
    {
        instance = this;
        cam = Camera.main;
    }
    private void Start()
    {
        pieceType = new int[8];
        shopPieceModels = new GameObject[8];

        InitializeShop();
    }

    public void PhysicalShopUpdate()// created by jordan to allow for raycasts to use exit button/sign whatever
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit ,25f))
            {
                if (hit.transform.CompareTag("LeaveShopSign"))
                {
                    //leave shop function
                    piecePanels.SetActive(false);
                    c.LookAtBoard();
                    GameStateManager.instance.SetNextLevel();
                }
            }
        }
    }

    public void InitializeShop()
    {
        for (int i = 0; i < pieceSpots.Length; i++)
        {
            pieceType[i] = (int)(Random.Range(0, 4));
            //Debug.Log("piecetype" + i + " is " + pieceType[i]);

            //place model at spot
            shopPieceModels[i] = Instantiate(pieceModels[pieceType[i]], pieceSpots[i].position, Quaternion.identity, gameObject.transform);
            //change price
            priceText[i].text = prices[pieceType[i]].ToString();
            //re activate panels
            uiSpots[i].SetActive(true);
        }
    }

    public void ResetShop()
    {
        for(int i = 0;i < pieceSpots.Length;i++)
        {
            Destroy(shopPieceModels[i]);
        }

        for (int i = 0; i < pieceSpots.Length; i++)
        {
            pieceType[i] = (int)(Random.Range(0, 4));
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
        piecePanels.SetActive(true);
        for (int i = 0; i < shopPieceModels.Length; i++)
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
                //Debug.Log(i);
                Destroy(shopPieceModels[i]);
                priceText[i].text = "SOLD OUT";
                Currency.instance.SubtractFromCurrency(prices[pieceType[i]]);
                //add to inventory

            }
        }

        buttonObject.SetActive(false);
    }

    //void Update()
    //{

    //}

}
