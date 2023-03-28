using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
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
    [SerializeField] public GameObject shopkeeper;
    [SerializeField] public string[] pieceNames;

    [SerializeField] private TextMeshPro[] inventoryShopCount;

    [SerializeField] private GameObject piecePanels;

    [SerializeField] private CameraHeadMovements c;
    private Camera cam;

    [SerializeField] private GameObject pieceDescriptionObject;

    [SerializeField] private TextMeshProUGUI pieceDescription;

    [SerializeField] private Canvas canvas;

    private RectTransform descriptionRect;

    private RectTransform canvasRect;

    private GameObject[] shopPieceModels;
    private int[] pieceType;

    private float canvasWidth;
    private float canvasHeight;

    [SerializeField] private Animator anim;

    [SerializeField] private Color leaveSignDefaultColor;
    [SerializeField] private Color leaveSignMouseHoverColor;
    [SerializeField] private TextMeshPro leaveSignText;
    private Coroutine activeCoRo;

    private void Awake()
    {
        instance = this;
        cam = Camera.main;
        shopkeeper.SetActive(false);
    }
    private void Start()
    {
        pieceType = new int[8];
        shopPieceModels = new GameObject[8];
        descriptionRect = pieceDescription.GetComponent<RectTransform>();
        canvasRect = canvas.GetComponent<RectTransform>();
        canvasWidth = canvasRect.rect.width;
        canvasHeight = canvasRect.rect.height;

        InitializeShop();
    }

    public void PhysicalShopUpdate()// created by jordan to allow for raycasts to use exit button/sign whatever
    {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        bool rayHit = Physics.Raycast(ray, out hit, 25f);
        if (rayHit)
        {
            //jordan -> idk if these need to be separate ifs or not so ima just leave em as separate
            if (hit.transform.CompareTag("Chess Piece"))
            {
                Debug.Log("Shop Piece HIt");
                Piece shopPiece = hit.transform.gameObject.GetComponent<Piece>();
                pieceDescription.text = pieceNames[(int)shopPiece.type];
                pieceDescriptionObject.SetActive(true);
                if(Input.GetMouseButtonDown(0))
                {                
                   if (prices[(int)shopPiece.type] <= Currency.instance.tickets)
                   {
                            for(int i = 0;i < 8;i++)
                            {
                                if(hit.transform.gameObject == shopPieceModels[i])
                                {
                                    priceText[i].text = "";
                                }
                            }
                    //priceText[i].text = "";
                    Currency.instance.SubtractFromCurrency(prices[(int)shopPiece.type]);
                    Inventory.instance.AlterPiece((Inventory.InventoryPieces)shopPiece.type,1);
                    Destroy(hit.transform.gameObject);
                   }
                }
            }
            if (hit.transform.CompareTag("StoreStock"))
            {
                Debug.Log("HONK");
            }
            if (hit.transform.CompareTag("LeaveShopSign"))
            {
                leaveSignText.color = leaveSignMouseHoverColor;
                Debug.Log("set to hover color");
                //change leave sign color text to "scrolled over"
                if (Input.GetMouseButtonDown(0) && activeCoRo == null)
                {
                    //leave shop function
                    StopAllCoroutines();
                    StartCoroutine(LeaveShopCoRo());
                }

            }
            else
            {
                //change leave sign color text to default
                leaveSignText.color = leaveSignDefaultColor;
                Debug.Log("set to default color");
            }
        }
        else
        {
            Debug.Log("OFF PIECE");
            pieceDescriptionObject.SetActive(false);
            leaveSignText.color = leaveSignDefaultColor;
        }


       

        for(int i = 0; i < inventoryShopCount.Length;i++)
        {
            inventoryShopCount[i].text = Inventory.instance.pieceCountText[i].text;
        }

        Vector2 mousePos = Input.mousePosition / canvas.scaleFactor;
        mousePos.x = (float)(mousePos.x - (canvasWidth * 0.5));
        mousePos.y = (float)(mousePos.y - (canvasHeight * 0.5));
        descriptionRect.anchoredPosition = mousePos;

    }

    void ShopkeeperInactive()
    {
        shopkeeper.SetActive(false);
    }

    public void InitializeShop()
    {
        for (int i = 0; i < pieceSpots.Length; i++)
        {
            pieceType[i] = (int)(Random.Range(0, 4));
            //Debug.Log("piecetype" + i + " is " + pieceType[i]);

            //place model at spot
            shopPieceModels[i] = Instantiate(pieceModels[pieceType[i]], pieceSpots[i].position, Quaternion.identity, gameObject.transform);
            //shopPieceModels[i].transform.localRotation = Quaternion.Euler(0, 90, 180);
            //change price
            priceText[i].text = prices[pieceType[i]].ToString();
            //re activate panels
            //uiSpots[i].SetActive(true);
            //Currency.instance.ticketsTxt.enabled = true;
        }
    }

    public void EnterPieceSpace(GameObject buttonObject)
    {
                for(int i = 0;i < uiSpots.Length;i++)
        {
            if(buttonObject == uiSpots[i])
            {
                pieceDescription.text = pieceNames[pieceType[i]];
            }
        }
        pieceDescriptionObject.SetActive(true);
    }

    public void LeavePieceSpace()
    {
        pieceDescriptionObject.SetActive(false);
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
            shopPieceModels[i] = Instantiate(pieceModels[pieceType[i]], pieceSpots[i].position,Quaternion.identity, gameObject.transform);
            shopPieceModels[i].transform.Rotate(0.0f,-90.0f,0.0f, Space.Self);
            //change price
            priceText[i].text = prices[pieceType[i]].ToString();
            //re activate panels
            //uiSpots[i].SetActive(true);
        }
    }

    public void EnterShop()
    {
        c.LookAtShop();
        //piecePanels.SetActive(true);
        //Currency.instance.ticketTextObject.enabled = true;
        shopkeeper.SetActive(true);
        anim.SetBool("EnteredShop", true);
        for (int i = 0; i < shopPieceModels.Length; i++)
        {
            //uiSpots[i].SetActive(true);
        }

    }

    public void PanelPressed(GameObject buttonObject)
    {
        for(int i = 0;i < uiSpots.Length;i++)
        {
            if(buttonObject == uiSpots[i] && prices[pieceType[i]] <= Currency.instance.tickets)
            {
                //Debug.Log(i);
                Destroy(shopPieceModels[i]);
                priceText[i].text = "";
                Currency.instance.SubtractFromCurrency(prices[pieceType[i]]);
                //add to inventory
                Inventory.instance.AlterPiece((Inventory.InventoryPieces)pieceType[i],1);
                buttonObject.SetActive(false);
                pieceDescriptionObject.SetActive(false);

            }
            else if((buttonObject == uiSpots[i] && prices[pieceType[i]] >= Currency.instance.tickets))
            {
                Debug.Log("expensive");
            }
        }
    }
    
    public IEnumerator LeaveShopCoRo()
    {
        anim.SetBool("ExitedShop", true);
        piecePanels.SetActive(false);
        Inventory.instance.objectiveArea.SetActive(true);
        Currency.instance.ticketTextObject.SetActive(false);
        Currency.instance.ticketBackgroundObject.SetActive(false);
        SaveManager.instance.SaveGame();

        yield return TorokPersonalityAI.instance.PlayAnimationAndSoundCoRo(SoundLibrary.Categories.ShopExit);

        //c.LookAtBoard();
        yield return CameraHeadMovements.instance.LookAtPlayAreaCoRo();
        GameStateManager.instance.SetNextLevel();
        pieceDescriptionObject.SetActive(false);
        Invoke("ShopkeeperInactive", 1.0f);
        //shopkeeper.SetActive(false);
    }

    //void Update()
    //{

    //}

}
