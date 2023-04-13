using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
//using UnityEngine.UI;
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
    [SerializeField] private GameObject hideShowButton;
    private int[] maxHeldPieces = {100,100,100,100,100};//the maximum number of each piece the player can have - 8,4,4,3,1
    [SerializeField] private int[] heldPieces = {8,4,4,3,1};//the amount of each piece the player has
    [SerializeField] private float ghostPieceVertOffset = -0.05f;
    private bool infinitePieces = true;
    [SerializeField] public GameObject objectiveArea;
    [SerializeField] public GameObject inventoryUI;
    [SerializeField] AudioClip buttonAudioClip;

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

    [SerializeField] private int storedPiece = -1;//pawn - 0, knight - 1, bishop - 2, rook - 3, queen - 4, remove - -1

    [SerializeField] private GameObject startButton;

    public static Inventory instance;

    public TextMeshProUGUI[] pieceCountText;

    [SerializeField] private TextMeshProUGUI infiniteText;

    [SerializeField] private TextMeshProUGUI objectiveText;
    [SerializeField] private TextMeshProUGUI levelCountText;

    public bool hasPlacedPiece = false;
    public int numPiecesPlaced = 0;
    public int requiredPiecesToPlay = 0;

    private bool deployCapReached;

    public int deployPointCap = 0;
    public int deployPieceCap = 0;

    private int visualPointCap = 0;
    private int visualPieceCap = 0;

    private int deployPointCount = 0;
    private int deployPieceCount = 0;

    private int[] deployValues = {1, 3, 3, 5, 9};

    [SerializeField] private GameObject deployUIObject;

    [SerializeField] private GameObject dualDeployLimits;
    [SerializeField] private GameObject pieceDeployLimit;
    [SerializeField] private GameObject pointDeployLimit;

    [SerializeField] private TextMeshProUGUI deployPointText;
    [SerializeField] private TextMeshProUGUI deployPieceText;

    [SerializeField] private TextMeshProUGUI soloDeployPointText;
    [SerializeField] private TextMeshProUGUI soloDeployPieceText;


    [SerializeField] private GameObject testModifiers;

    public void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        rectTrans = GetComponent<RectTransform>();
        cam = Camera.main;
        objectiveArea.SetActive(false);
        //hideShowButton.SetActive(false);

        //initialize the max number of each puece can be held in inventory
        //placeholder values

        //maxHeldPieces[0] = 5; //max pawns
        //maxHeldPieces[1] = 5; //max knights
        //maxHeldPieces[2] = 5; //max bishops
        //maxHeldPieces[3] = 5; //max rooks
        //maxHeldPieces[4] = 1; //max queens

        updateCountText();
        DisableDeployUI();

    }

    public void InventoryUpdate()
    {

        if((deployPointCount >= visualPointCap) || (deployPieceCount >= visualPieceCap))
        {
            //Debug.Log("CAP REACHED");
            deployCapReached = true;
        }
        else
        {
            deployCapReached = false;
        }

        if (numPiecesPlaced >= requiredPiecesToPlay)
        {
            
            startButton.SetActive(true);
        }
        else
        {
            startButton.SetActive(false);
        }

        if (Input.GetKeyUp("m"))
        {
            if(testModifiers.activeSelf)
            {
                testModifiers.SetActive(false);
            }
            else
            {
                testModifiers.SetActive(true);
            }
        }

        if (Input.GetKeyUp("l"))
        {
            GameStateManager.instance.ChangeGameState(GameStateManager.GameState.win);
        }

        objectiveArea.SetActive(CameraHeadMovements.instance.menuDone);

        if (!CameraHeadMovements.instance.menuDone) {
            SlideHideInventoryPanel();
        }

        hideShowButton.SetActive(CameraHeadMovements.instance.menuDone);
        startButton.SetActive(CameraHeadMovements.instance.menuDone); // ???

        //print("in invenotry update");
        //if in deploy mode
        if (isShowingPanel)
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);//shoot ray using mouse from camera

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.CompareTag("Chess Board"))//if you hit a board tile -- AND the tile is deployment tile if player
                {

                    //remove other visuals
                    foreach (GameObject obj in PiecePrefabs)
                    {
                        obj.transform.position = new Vector3(-200, 0, 0);
                    }

                    //show desired visual
                    if (storedPiece >= 0 && storedPiece < 5)
                    {
                        for(int i = 0; i<Board.boardSize;i++)
                        {
                            for(int j = 0;j<Board.boardSize;j++)
                            {
                                if(hit.transform.gameObject == Board.instance.hitBoxBoard[i,j])
                                {
                                    for(int k = 0;k<Board.instance.deploymentZoneList.Count;k++)
                                    {
                                        if(Board.instance.deploymentZoneList[k].x == i && Board.instance.deploymentZoneList[k].y == j)
                                        {
                                            //Debug.Log("IN THE DEPLOY ZONE");
                                                if(heldPieces[storedPiece] > 0)
                                            {
                                                PiecePrefabs[storedPiece].transform.localPosition = hit.transform.position + (Vector3.up * ghostPieceVertOffset);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if(Input.GetMouseButtonDown(0))//Patrick - mouse input to place piece
                    {
                        if (Board.instance.torokPiece && storedPiece < 6)//
                        {
                            Board.instance.PlacePieceTorok(hit.transform, storedPiece);
                        }
                        else if (storedPiece >= (int)Piece.PieceType.wall && storedPiece <= (int)Piece.PieceType.hole)
                        {
                            Vector2Int coords = new Vector2Int(-1, -1);
                            for (int i = 0; i < Board.boardSize; i++)
                            {
                                for (int j = 0; j < Board.boardSize; j++)
                                {
                                    if (Board.instance.hitBoxBoard[i, j] == hit.transform.gameObject)
                                    {
                                        coords.Set(i, j);
                                    }
                                }
                            }
                            if (!(coords.x < 0 || coords.y < 0)) {
                                Board.instance.PlaceObstacle(coords.x, coords.y, storedPiece - 6,0);
                            }

                        }
                        else
                        {
                            //Debug.Log("PLACEPLAYERPIECE: "+(InventoryPieces)storedPiece);
                            //if(storedPiece < 5 && storedPiece > -1 && hit.transform.gameObject.name.Contains("_DeploySpot"))
                            if (storedPiece < 5 && storedPiece > -1)
                            {
                                foreach (Vector2Int location in Board.instance.deploymentZoneList)
                                {
                                    for (int i = 0; i < Board.boardSize; i++)
                                    {
                                        for (int j = 0; j < Board.boardSize; j++)
                                        {
                                             if (hit.transform.gameObject == Board.instance.hitBoxBoard[i, j] && i == location.x && j == location.y)
                                             {
                                                 if (heldPieces[storedPiece] > 0 && !deployCapReached && (deployPointCount + deployValues[storedPiece]) <= visualPointCap)
                                                 {
                                                     //AlterPiece((InventoryPieces)storedPiece, -1);
                                                     //deployPieceCount++;
                                                     //deployPointCount += deployValues[storedPiece];
                                                     //SetDeployUI();
                                                     if (Board.instance.PlacePiece(hit.transform, storedPiece) == true)
                                                     {
                                                        SoundObjectPool.instance.GetPoolObject().Play(Board.instance.boardAudioClips[(int)BoardSounds.MovePieceEnd]);
                                                        AlterPiece((InventoryPieces)storedPiece, -1);
                                                        deployPieceCount++;
                                                        deployPointCount += deployValues[storedPiece];
                                                        SetDeployUI();
                                                        numPiecesPlaced++;
                                                     }
                                                     else
                                                     {
                                                        SoundObjectPool.instance.GetPoolObject().Play(Board.instance.boardAudioClips[(int)BoardSounds.PlaceFail]);
                                                    }
                                                 }
                                             }
                                        }
                                    }
                                }
                            }
                            else if(storedPiece == 5)
                            {
                                if (Board.instance.PlacePiece(hit.transform, storedPiece) == true)
                                {
                                    SoundObjectPool.instance.GetPoolObject().Play(Board.instance.boardAudioClips[(int)BoardSounds.MovePieceEnd]);
                                    hasPlacedPiece = true;
                                }
                                else
                                {
                                    SoundObjectPool.instance.GetPoolObject().Play(Board.instance.boardAudioClips[(int)BoardSounds.PlaceFail]);
                                }
                            }
                            else if (storedPiece == -1)// if on remove
                            {
                                for(int i = 0;i<8;i++)
                                {
                                    for (int j = 0; j < 8; j++)
                                    {
                                        //if (hit.transform.gameObject == Board.instance.hitBoxBoard[i, j] || hit.transform.gameObject == Board.instance.deployBoard[i,j])
                                        if (hit.transform.gameObject == Board.instance.hitBoxBoard[i, j])
                                        {
                                            //Debug.Log("BOARDREMOVETESTING");
                                            if (Board.pieceBoard[i, j] != null)
                                            {
                                                Piece removePiece = Board.pieceBoard[i, j].GetComponent<Piece>();

                                                if (!removePiece.isTorok && (int)removePiece.type < 5)
                                                {
                                                    AlterPiece((InventoryPieces)removePiece.type, 1);
                                                    deployPieceCount--;
                                                    deployPointCount -= deployValues[(int)removePiece.type];
                                                    SetDeployUI();
                                                }

                                                if (Board.instance.PlacePiece(Board.pieceBoard[i, j].transform, storedPiece) == true)
                                                {
                                                    //Debug.Log("REMOVE?");
                                                    //numPiecesPlaced--;
                                                }
                                            }
                                        }
                                    }
                                }
                                if (Board.instance.PlacePiece(hit.transform, storedPiece) == true)
                                {
                                    //hasPlacedPiece = true;
                                }
                                
                            }
                        }

                        if(!Board.instance.torokPiece && storedPiece == -1)//place peice nd remove form inevtory
                        {
                            //Debug.Log("remove player piece");
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
                    else if (Input.GetMouseButtonDown(1))
                            {
                                for(int i = 0;i<8;i++)
                                {
                                    for (int j = 0; j < 8; j++)
                                    {
                                        //if (hit.transform.gameObject == Board.instance.hitBoxBoard[i, j] || hit.transform.gameObject == Board.instance.deployBoard[i,j])
                                        if (hit.transform.gameObject == Board.instance.hitBoxBoard[i, j])
                                        {
                                            //Debug.Log("BOARDREMOVETESTING");
                                            if (Board.pieceBoard[i, j] != null)
                                            {
                                                Piece removePiece = Board.pieceBoard[i, j].GetComponent<Piece>();

                                                if (!removePiece.isTorok && (int)removePiece.type < 5)
                                                {

                                                    if (Board.instance.PlacePiece(Board.pieceBoard[i, j].transform, -1) == true)
                                                    {
                                                        SoundObjectPool.instance.GetPoolObject().Play(Board.instance.boardAudioClips[(int)BoardSounds.RemovePiece]);
                                                    }
                                                    AlterPiece((InventoryPieces)removePiece.type, 1);
                                                    deployPieceCount--;
                                                    deployPointCount -= deployValues[(int)removePiece.type];
                                                    SetDeployUI();
                                                }

                                            }
                                        }
                                    }
                                }
                                //if (Board.instance.PlacePiece(hit.transform, -1) == true)
                                //{
                                //    Debug.Log("TESTETEST");
                                    //hasPlacedPiece = true;
                                //}
                                
                            }
                }
                else if (hit.transform.gameObject.CompareTag("Chess Piece"))//if trying to remove player piece
                {

                    Piece hitPiece = hit.transform.GetComponent<Piece>();
                    if (Input.GetMouseButtonDown(1) && hitPiece && !hitPiece.isTorok && (int)hitPiece.type < 5)
                    {
                        //print("inside removePLayer");
                        //Debug.Log((int)storedPiece);
                        if (Board.instance.PlacePiece(hit.transform, -1) == true)
                        {
                            SoundObjectPool.instance.GetPoolObject().Play(Board.instance.boardAudioClips[(int)BoardSounds.RemovePiece]);
                            numPiecesPlaced--;
                        }
                        if ((int)hitPiece.type < 5)
                        {

                            AlterPiece((InventoryPieces)hitPiece.type, 1);
                            deployPieceCount--;
                            deployPointCount -= deployValues[(int)hitPiece.type];
                            SetDeployUI();

                        }
                        updateCountText();
                    }
                    else if (Input.GetMouseButtonDown(0))
                    {
                        SoundObjectPool.instance.GetPoolObject().Play(Board.instance.boardAudioClips[(int)BoardSounds.PlaceFail]);
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
            SoundObjectPool.instance.GetPoolObject().Play(buttonAudioClip);
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
            SoundObjectPool.instance.GetPoolObject().Play(buttonAudioClip);
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
            SoundObjectPool.instance.GetPoolObject().Play(buttonAudioClip);
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
            SoundObjectPool.instance.GetPoolObject().Play(buttonAudioClip);
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
            SoundObjectPool.instance.GetPoolObject().Play(buttonAudioClip);
            storedPiece = 4;
        }
        else
        {
            Debug.Log("no queens in inventory");
        }   
    }

    public void KingButtonClicked()
    {
        SoundObjectPool.instance.GetPoolObject().Play(buttonAudioClip);
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
        SoundObjectPool.instance.GetPoolObject().Play(buttonAudioClip);
        if (isShowingPanel)
        {
            hideShowText.text = "show";
        }
        else
        {
            hideShowText.text = "hide";
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

    public void SlideShowInventoryPanel()
    {
        if (isMoving)
        {
            StopAllCoroutines();
        }
        isShowingPanel = false;
        hideShowText.text = "hide";
        StartCoroutine(ShowHideInventoryPanel());
    }

    public void SlideHideInventoryPanel()
    {
        if (isMoving)
        {
            StopAllCoroutines();
        }
        isShowingPanel = true;
        hideShowText.text = "show";
        StartCoroutine(ShowHideInventoryPanel());
    }

    public int GetStoredPiece()
    {
        return storedPiece;
    }

    public void SetObjective()
    {
        levelCountText.text = "Board "+ (GameStateManager.instance.currentLevelNumber + 1);
        objectiveText.text = GameStateManager.instance.winCondition.GetObjectiveText();
    }

    public void SetDeployUI()
    {
        if(deployPieceCap < 0 && deployPointCap < 0)//no cap
        {
            visualPointCap = deployPointCap;
            visualPieceCap = 100000;
            visualPointCap = 100000;

            dualDeployLimits.SetActive(false);
            pieceDeployLimit.SetActive(false);
            pointDeployLimit.SetActive(true);

            soloDeployPointText.text = "No limits";

        }
        else if(deployPieceCap < 0)//using point cap
        {
            visualPointCap = deployPointCap;
            visualPieceCap = 100000;

            dualDeployLimits.SetActive(false);
            pieceDeployLimit.SetActive(false);
            pointDeployLimit.SetActive(true);

            soloDeployPointText.text = "Point Limit: "+deployPointCount+"/"+visualPointCap;
        }
        else if(deployPointCap < 0)//using piece cap
        {
            visualPieceCap = deployPieceCap;
            visualPointCap = 100000;

            dualDeployLimits.SetActive(false);
            pieceDeployLimit.SetActive(true);
            pointDeployLimit.SetActive(false);

            soloDeployPieceText.text = "Piece Limit: "+deployPieceCount+"/"+visualPieceCap;
        }
        else//using both
        {
            visualPointCap = deployPointCap;
            visualPieceCap = deployPieceCap;

            dualDeployLimits.SetActive(true);
            pieceDeployLimit.SetActive(false);
            pointDeployLimit.SetActive(false);

            deployPieceText.text = "Piece Limit: "+deployPieceCount+"/"+visualPieceCap;
            deployPointText.text = "Point Limit: "+deployPointCount+"/"+visualPointCap;
        }

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
        SoundObjectPool.instance.GetPoolObject().Play(buttonAudioClip);
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

    public int[] GetInventoryCount()
    {
        return heldPieces;
    }

    public void SetInventoryCount(int[] count)
    {
        heldPieces = count;
    }

    public void resetDeployCount()
    {
        deployPieceCount = 0;
        deployPointCount = 0;
    }

    public void DisableDeployUI()
    {
        deployUIObject.SetActive(false);
    }

    public void EnableDeployUI()
    {
        deployUIObject.SetActive(true);
    }

    public void DisableModifiers()
    {
        testModifiers.SetActive(false);
    }

}
