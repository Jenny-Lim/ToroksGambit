using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class Inventory : MonoBehaviour
{

    public enum InventoryPieces {
        pawn,
        knight,
        bishop,
        rook,
        queen
    }

    private RectTransform rectTrans;
    [SerializeField] private TextMeshProUGUI hideShowText;
    [SerializeField] private int[] maxHeldPieces = new int[5];//the maximum number of each piece the player can have
    private int[] heldPieces = new int[5];//the amount of each piece the player has
    [SerializeField] private float ghostPieceVertOffset = -0.05f;
    private bool infinitePieces = true;

    public void ShowInventoryPanel()
    {
        gameObject.SetActive(true);
    }

    public void HideInventoryPanel()
    {
        gameObject.SetActive(false);
    }

    private bool isShowingPanel = false;
    private bool isMoving = false;
    [SerializeField] private float travelSpeed = 1f;

    [SerializeField] private Vector3 showLocation;
    [SerializeField] private Vector3 hideLocation;

    [SerializeField] private GameObject[] PiecePrefabs;
    private Camera cam;

    [SerializeField] private int storedPiece = 1;//pawn - 0, knight - 1, bishop - 2, rook - 3, queen - 4, remove - -1

    [SerializeField] private GameObject startButton;

    public static Inventory instance;

    [SerializeField] private TextMeshProUGUI[] pieceCountText;

    [SerializeField] private TextMeshProUGUI infiniteText;

    public void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        rectTrans = GetComponent<RectTransform>();
        cam = Camera.main;

        //initialize the max number of each puece can be held in inventory
        //placeholder values

        maxHeldPieces[0] = 5; //max pawns
        maxHeldPieces[1] = 5; //max knights
        maxHeldPieces[2] = 5; //max bishops
        maxHeldPieces[3] = 5; //max rooks
        maxHeldPieces[4] = 1; //max queens

        updateCountText();

    }

    public void InventoryUpdate()
    {
        //print("in invenotry update");
        //if in deploy mode
        if (isShowingPanel)
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);//shoot ray using mouse from camera

            if (Physics.Raycast(ray, out hit))
            {

                if (hit.transform.gameObject.CompareTag("Chess Board"))//if you hit a board tile
                {

                    //remove other visuals
                    foreach (GameObject obj in PiecePrefabs)
                    {
                        obj.transform.position = new Vector3(-200, 0, 0);
                    }

                    //show desired visual
                    if (storedPiece >= 0 && storedPiece < 5)
                    {
                        PiecePrefabs[storedPiece].transform.localPosition = hit.transform.position + (Vector3.up * ghostPieceVertOffset);
                    }

                    if(Input.GetMouseButtonDown(0))//Patrick - mouse input to place piece
                    {
                        if (Board.instance.torokPiece && storedPiece < 6)//
                        {
                            Board.instance.PlacePieceTorok(hit.transform, storedPiece);
                        }
                        else
                        {
                            Board.instance.PlacePiece(hit.transform, storedPiece);
                        }

                        if(!Board.instance.torokPiece && storedPiece > -1 && storedPiece < 6)//place peice nd remove form inevtory
                        {
                            //AlterPiece((InventoryPieces)storedPiece, -1);
                            //updateCountText();
                        }
                        else//remove piece from board back into inventroy by pickng board spot - STILL DOESNT WORK
                        {
                            //AlterPiece((InventoryPieces)storedPiece, -1);
                            updateCountText();
                        }
                        //remove piece from inventory of player
                    }
                }
                else if (hit.transform.gameObject.CompareTag("Chess Piece") && storedPiece == -1)//if trying to remove player piece
                {

                    Piece hitPiece = hit.transform.GetComponent<Piece>();
                    if (Input.GetMouseButtonDown(0) && hitPiece && !hitPiece.isTorok)
                    {
                        //print("inside removePLayer");
                        Debug.Log((int)storedPiece);
                        Board.instance.PlacePiece(hit.transform, storedPiece);
                        AlterPiece((InventoryPieces)hitPiece.type, 1);
                        updateCountText();
                    }
                }
                else
                {
                    //remove other visuals
                    foreach (GameObject obj in PiecePrefabs)
                    {
                        obj.transform.position = new Vector3(-200, 0, 0);
                    }
                }
            }
            else
            {
                //remove other visuals
                foreach (GameObject obj in PiecePrefabs)
                {
                    obj.transform.position = new Vector3(-200, 0, 0);
                }
            }
        }
        
    }

    //changes the designated number of held pieces by amount
    public void AlterPiece(InventoryPieces type, int amount)
    {
        heldPieces[(int)type] += amount;
        if (heldPieces[(int)type] > maxHeldPieces[(int)type])
        {
            heldPieces[(int)type] = maxHeldPieces[(int)type];
        }
        updateCountText();

    }

    //sets the designated held amount of pieces to amount
    public void SetPieceAmount(InventoryPieces type, int amount)
    {
        heldPieces[(int)type] = amount;
        if (heldPieces[(int)type] > maxHeldPieces[(int)type])
        {
            heldPieces[(int)type] = maxHeldPieces[(int)type];
        }
    }

    //returns how many pieces the player has of a designated type
    public int GetNumberOfPieces(InventoryPieces type)
    {
        return heldPieces[(int)type];
    }

    public void PawnButtonClicked()
    {
        if(heldPieces[0] > 0 || infinitePieces)
        {
            storedPiece = 0;
        }
        else
        {
            Debug.Log("no pawns in inventory");
        }

    }

    public void KnightButtonClicked()
    {
        if(heldPieces[1] > 0 || infinitePieces)
        {
            storedPiece = 1;
        }
        else
        {
            Debug.Log("no knights in inventory");
        }    
    }

    public void BishopButtonClicked()
    {
        if(heldPieces[2] > 0 || infinitePieces)
        {
            storedPiece = 2;
        }
        else
        {
            Debug.Log("no bishops in inventory");
        }   
    }

    public void RookButtonClicked()
    {
        if(heldPieces[3] > 0 || infinitePieces)
        {
            storedPiece = 3;
        }
        else
        {
            Debug.Log("no rooks in inventory");
        }   
    }

    public void QueenButtonClicked()
    {
        if(heldPieces[4] > 0 || infinitePieces)
        {
            storedPiece = 4;
        }
        else
        {
            Debug.Log("no queens in inventory");
        }   
    }

    public void KingButtonClicked()
    {
        storedPiece = 5;
    }

    public void RemoveButtonClicked()
    {
        storedPiece = -1;
    }

    public void WallButtonClicked()
    {
        storedPiece = 6;
    }

    public void HoleButtonClicked()
    {
        storedPiece = 7;
    }

    public void HideShowButtonClicked()
    {
        if (isShowingPanel)
        {
            hideShowText.text = "Show";
        }
        else
        {
            hideShowText.text = "Hide";
        }

        if (isMoving)
        {
            StopAllCoroutines();
        }
        StartCoroutine(ShowHideInventoryPanel());

    }

    public void updateCountText()
    {
        for(int i = 0; i < pieceCountText.Length;i++)
        {
            pieceCountText[i].text = heldPieces[i].ToString();
        }
    }

    public int GetStoredPiece()
    {
        return storedPiece;
    }

    private IEnumerator ShowHideInventoryPanel()
    {
        isMoving = true;

        isShowingPanel = !isShowingPanel;

        if (!isShowingPanel)
        {
            while (Vector3.Distance(rectTrans.anchoredPosition3D, hideLocation) >= 0.15)
            {
                rectTrans.anchoredPosition3D = Vector3.Lerp(rectTrans.anchoredPosition3D, hideLocation, Time.deltaTime * travelSpeed);
                yield return null;
            }
            
        }
        else
        {
            while (Vector3.Distance(rectTrans.anchoredPosition3D, showLocation) >= 0.15)
            {
                rectTrans.anchoredPosition3D = Vector3.Lerp(rectTrans.anchoredPosition3D, showLocation, Time.deltaTime * travelSpeed);
                yield return null;
            }
            
        }

        isMoving = false;
    }

    public void OnStartChessGameButtonPressed()
    {
        GameStateManager.instance.ChangeGameState(GameStateManager.GameState.game);
        startButton.SetActive(false);
    }

    public void ResetStartButton()
    {
        startButton.SetActive(true);
    }

    public void InfiniteButton()
    {
        if(infinitePieces)
        {
            infinitePieces = true;
            infiniteText.text = "Infinite = false";
        }
        else
        {
            infinitePieces = true;
            infiniteText.text = "Infinite = true";
        }
    }

}
