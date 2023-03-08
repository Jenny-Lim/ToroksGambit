using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Board : MonoBehaviour
{
    public static int boardSize = 8;//size of 2D array
    [SerializeField] float boardVerticalOffset = 0.5f;//offset for board tiles vertically
    [SerializeField] float verticalPlaceOffset = 0.5f;

    private GameObject[,] moveTileBoard;

    public GameObject[,] hitBoxBoard;//array for hitboxes for raycasting

    public static GameObject[,] pieceBoard;//array for storing pieces and piece location -- made static (jenny)

    private GameObject[,] winSpotBoard;

    [SerializeField]
    private GameObject boardSquare;

    [SerializeField] private GameObject[] boardTiles = new GameObject[2];

    [SerializeField] private GameObject moveTile;

    [SerializeField] private GameObject winSpotTile;

    [SerializeField]
    private GameObject chessPiece;

    private Camera cam;

    public GameObject clickedPiece;

    public List<Move> moveList = new List<Move>();

    private int undoCounter = 0;

    private Vector3 boardPosition;

    public bool canMove = true;

    [SerializeField]
    private Inventory inventoryScript;

    [SerializeField] private float pieceMoveSpeed = 35f;//made by jordan, can change piece move speed easier

    // made static -- jenny
    private static int pieceX;
    private static int pieceY;

    [SerializeField] private GameObject[] piecePrefabs;//list of prefabs corresponding to indices in inventory storedPiece format (0 - pawn, 1 - knight, 2 - bishop, etc)
    [SerializeField] private GameObject[] obstaclePrefabs;//list of obstacles, 0 -> wall, 1 -> hole

    public static Board instance;//jordan, static ref to board
    public List<Vector2> deploymentZoneList;//jordan, list of positions on the board that can be deployed on

    [SerializeField] private GameObject selectionIndicator;// testing gameobject that floats above the selected piece for indication purposes 


    public bool torokPiece = false;

    private bool isLastchance;

    private bool isPromote;

    [SerializeField] private Material[] pieceMats = new Material[2];// 0 -> player piece color, 1 means torok piece color

    //traits

    public bool toughPlacer;
    public bool lastChancePlacer;
    public bool promotePlacer;

    [SerializeField]
    public TextMeshProUGUI text;

    [SerializeField]
    public TextMeshProUGUI toughText;

    [SerializeField]
    public TextMeshProUGUI lastChanceText;

    [SerializeField]
    public TextMeshProUGUI promoteText;

    [SerializeField]
    private GameObject toughButton;

    [SerializeField]
    private GameObject lastChanceButton;

    [SerializeField]
    private GameObject promoteButton;

    public static bool playerInCheck;//set if player is in check <- needs to be implemented
    public static bool torokInCheck;//set if torok is in check <- needs to be implemented

    [SerializeField] private GameObject moveStartIndicator;
    [SerializeField] private GameObject moveEndIndicator;

    [SerializeField] private float idleDialogueCounter = 0;

    public List<Vector2Int> winLocations;

    [SerializeField] private Material[] TorokPieceMats;

    // brought them up here
    //private static int clickedX;
    //private static int clickedY;

    void Start()
    {
        if (instance == null) { instance = this; }//added by jordan for static reference to board for minmax
        deploymentZoneList = new List<Vector2>();
        boardPosition = transform.position;
        cam = Camera.main;
        hitBoxBoard = new GameObject[boardSize,boardSize];
        pieceBoard = new GameObject[boardSize, boardSize];
        moveTileBoard = new GameObject[boardSize, boardSize];
        winSpotBoard = new GameObject[boardSize, boardSize];
        idleDialogueCounter = Random.Range(TorokPersonalityAI.instance.maxTimeBetweenIdleBark,TorokPersonalityAI.instance.maxTimeBetweenIdleBark);
        BuildBoard();
    }

    //**Place anything were that needs to be reset when a new level is loaded**
    public void Reset()
    {
        winLocations.Clear();
        winLocations.TrimExcess();
        moveStartIndicator.transform.position = new Vector3(-200,0,0);
        moveEndIndicator.transform.position = new Vector3(-200, 0, 0);
    }

    public void BoardUpdate()
    {
        
        // print("in bvoard update");
        if (idleDialogueCounter <= 0.0f)
        {
            TorokPersonalityAI.instance.PlayAnimationAndSound(SoundLibrary.Categories.Idle);
            idleDialogueCounter = Random.Range(TorokPersonalityAI.instance.maxTimeBetweenIdleBark, TorokPersonalityAI.instance.maxTimeBetweenIdleBark);
        }
        else
        {
            idleDialogueCounter -= Time.deltaTime;
        }
            
        


        if (Input.GetKeyUp("q")) {
            print(IsKingInCheck(false));
        }

        if (Input.GetKeyDown(KeyCode.B) && clickedPiece != null)//***Testing move generating
        {
            print("printing selected piece moves...");
            foreach (Move move in clickedPiece.GetComponent<Piece>().moves)
            {
                print(move.DisplayMove());
            }
        }

        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);//shoot ray using mouse from camera

        if (Input.GetMouseButtonDown(0))//left click mouse to move pieces
        {
            int piecePlace = inventoryScript.GetStoredPiece();

        if (Physics.Raycast(ray, out hit))
        {
            
            if((hit.transform.tag == "Chess Board" || hit.transform.tag == "Chess Piece") && clickedPiece)//if a piece is stored and another spot is chosen
            {
                Debug.Log("hit piece");
                //Debug.Log("TEST");
                int clickedX = 0;
                int clickedY = 0;

                //clickedX = 0;//position for second click
                //clickedY = 0;

                for(int i=0;i<boardSize;i++)
                {
                    for(int j=0;j<boardSize;j++)
                    {
                        if(hit.transform.gameObject == hitBoxBoard[i,j] || hit.transform.gameObject == pieceBoard[i,j])
                        {
                            clickedX = i;//get position of second spot
                            clickedY = j;
                            
                        }
                    }
                }

                GameObject tempPiece = pieceBoard[clickedX, clickedY];


                if (tempPiece)
                {
                    Piece endPieceScript = tempPiece.GetComponent<Piece>();

                    if (endPieceScript.lastChance)
                    {
                        isLastchance = true;
                    }
                }



                //pieceBoard[clickedX, clickedY] = null;
                //DisablePiece(tempPiece);
                //print("pieceX of board :" + pieceX);
                //print("pieceY of board :" + pieceY);
                //bool isValid = MoveValidator(pieceX, pieceY, clickedX, clickedY);

                if (canMove)
                {
                    ClearMoveTiles();
                    StartCoroutine(MovePieceValidatorCoRo(pieceX, pieceY, clickedX, clickedY));
                }

                //print(IsKingInCheck(true));

                if (isLastchance)
                {
                    clickedPiece = null;
                }

                if(canMove)
                {
                    //DisablePiece(tempPiece);
                    if(clickedPiece)
                    {
                        //clickedPiece = pieceBoard[clickedX,clickedY];// <- is this redundant because of moveValidator, shouldnt that have already moved the piece?
                        //MovePieceVisual(pieceX, pieceY, clickedX, clickedY, clickedPiece,false);
                        //StartCoroutine(VisualMovePiece(pieceX, pieceY, clickedX, clickedY, clickedPiece,false));
                    }
                    else
                    {
                        //GameStateManager.EndTurn();
                        isLastchance = false;//<- idk what this does, so idk if this should be moved after moving validation to a coro above
                        isPromote = false;

                    }
                    //GameStateManager.EndTurn();
                }
                //canMove = false;
                clickedPiece = null;

            }
            else if (hit.transform.tag == "Chess Piece")//if mouse is clicked on chess piece
            {

                // Debug.Log("Piece");
                GameObject tempPiece = hit.transform.gameObject;//removed the ,parent cuz i changed the hitbox to be on the highest level of the piece prefabs - jordan

                Piece piece = tempPiece.GetComponent<Piece>(); 

                //Debug.Log("CHOSEN PIECE TYPE: "+piece.type);

                if(piece.promote)
                {

                }

                if(!piece.isTorok)
                {
                    clickedPiece = hit.transform.gameObject;//store piece, same as above comment about prefabs

                    // Debug.Log(hit.transform.parent.gameObject);

                    for(int i=0;i<boardSize;i++)//doesnt appear to work, always returns as 0
                    {
                        for(int j=0;j<boardSize;j++)
                        {
                            if(hit.transform.gameObject == pieceBoard[i,j])//get position of piece in array, smae as the two above comments
                            {
                                pieceX = i;//store locations
                                pieceY = j;
                                    
                            }
                        }
                    }
                }

                ClearMoveTiles();

                piece.UpdateMoves();
                if(!piece.isTorok)
                {
                for(int i = 0;i < piece.moves.Count;i++)
                {
                    moveTileBoard[piece.moves[i].endX, piece.moves[i].endY].SetActive(true);
                }
                }

                //signifier that piece is chosen
                //storedPiece.transform.position = Vector3.MoveTowards(hit.transform.position, hit.transform.position+new Vector3(0,5,0), 10f * Time.deltaTime);

            }
        else if (hit.transform.tag == "Chess Board")
        {

                    for(int i=0;i<boardSize;i++)//doesnt appear to work, always returns as 0
                    {
                        for(int j=0;j<boardSize;j++)
                        {
                            if(hit.transform.gameObject == hitBoxBoard[i,j])//get position of piece in array, smae as the two above comments
                            {
                                if(pieceBoard[i,j])
                                {
                                    Piece piece = pieceBoard[i,j].GetComponent<Piece>();

                                    if(!piece.isTorok)
                                    {
                                        clickedPiece = pieceBoard[i,j];//store piece, same as above comment about prefabs
                                        pieceX = i;//store locations
                                        pieceY = j;
                                    }

                                    ClearMoveTiles();

                                    piece.UpdateMoves();
                                    if(!piece.isTorok)
                                    {
                                    for(int q = 0;q < piece.moves.Count;q++)
                                    {
                                        moveTileBoard[piece.moves[q].endX, piece.moves[q].endY].SetActive(true);
                                    }
                                    }


                                }
                                    
                            }
                        }
                    }

            Debug.Log("no piece and picked board");
        }
        }

        else 
        {

                //storedPiece.transform.position = Vector3.MoveTowards(hit.transform.position, hit.transform.position - new Vector3(0, 5, 0), 10f * Time.deltaTime);
                clickedPiece = null; 
        }
        }

        if (Input.GetMouseButtonDown(1))//right click mouse to undo moves
        {
            UndoMove();
        }

        if (clickedPiece != null)//added by jordan to indicate what piece is clicked
        {
            selectionIndicator.transform.position = clickedPiece.transform.position + new Vector3(0.5f,0.5f,0f);
        }
        else
        {
            selectionIndicator.transform.position = new Vector3(-200f,0f,0f);
        }

    }

    public bool PlacePiece(Transform boardSpot, int pieceId)
    {
        //**should reformat this function cuz im sure there is some getComponent overlapping**

        if (!boardSpot)
        {
            Debug.LogError("Trying to place piece, given piece transform was null");
            return false;
        }

        int placeX = -1;
        int placeY = -1;
        if (boardSpot.CompareTag("Chess Board"))
        {
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    if (boardSpot.gameObject == hitBoxBoard[i, j])//get position of piece in array
                    {
                        placeX = i;//store locations
                        placeY = j;

                    }
                }
            }

            if (placeX == -1 && placeY == -1)
            {
                Debug.LogError("Error trying to place piece where piece already is.");
                return false;
            }
        }
        else if (boardSpot.CompareTag("Chess Piece")) 
        {
            Piece getPosPiece = boardSpot.GetComponent<Piece>();
            if (getPosPiece)
            {
                placeX = getPosPiece.pieceX;
                placeY = getPosPiece.pieceY;
            }
        }

        if (pieceBoard[placeX, placeY] != null && pieceId != -1)
        {
            Debug.LogError("Did not place piece because piece was already there");
            return false;
        }

        if (pieceId >= 0 && pieceId < 6)
        {
            GameObject newPiece = pieceBoard[placeX, placeY] = Instantiate(piecePrefabs[pieceId], boardSpot.position + (Vector3.up * verticalPlaceOffset), Quaternion.identity, gameObject.transform);//instantiate piece and place in pieceBoard location
            //newPiece.transform.GetChild(1).GetComponent<MeshRenderer>().material = pieceMats[0];//ik this is bad but whatever
            Piece piece = newPiece.GetComponent<Piece>();

            //Debug.Log("placed piece type: "+piece.type);

            if(torokPiece)
            {
                piece.isTorok = true;
            }

            if (toughPlacer)
            {
                piece.isTough = true;
            }
            if (lastChancePlacer)
            {
                piece.lastChance = true;
            }
            if (promotePlacer)
            {
                piece.promote = true;
            }

            piece.pieceX = placeX; 
            piece.pieceY = placeY;
        }
        else if(pieceId > 5)
        {
            GameObject newPiece = pieceBoard[placeX, placeY] = Instantiate(obstaclePrefabs[pieceId-6], boardSpot.position + (Vector3.up * verticalPlaceOffset), Quaternion.identity, gameObject.transform);//instantiate piece and place in pieceBoard location
        }
        else 
        {
            //do any inventory stuff here
            Destroy(pieceBoard[placeX, placeY]);
            pieceBoard[placeX, placeY] = null;
        }

        return true;
    }

    public bool PlacePiece(int xPos, int yPos, int pieceId)
    {
        //int pieceId = inventoryScript.GetStoredPiece();

        if (pieceBoard[xPos, yPos] != null && pieceId != -1)
        {
            Debug.LogError("Error trying to place piece where piece already is.");
            return false;
        }

        if (pieceId >= 0)
        {
            GameObject newPiece = pieceBoard[xPos, yPos] = Instantiate(piecePrefabs[pieceId], hitBoxBoard[xPos,yPos].transform.position + (Vector3.up * verticalPlaceOffset), Quaternion.identity, gameObject.transform);//instantiate piece and place in pieceBoard location
            //newPiece.transform.GetChild(1).GetComponent<MeshRenderer>().material = pieceMats[0];//ik this is bad but whatever
            Piece piece = newPiece.GetComponent<Piece>();
            piece.pieceX = xPos;
            piece.pieceY = yPos;

        }
        else
        {
            //remove piece functionality
            Destroy(pieceBoard[xPos, yPos]);
            pieceBoard[xPos, yPos] = null;
        }
        return true;
    }

    public bool PlacePieceTorok(int xPos, int yPos, int pieceId)
    {
        if (pieceBoard[xPos, yPos] != null)
        {
            //Debug.Log("Trace Stack for ");
            Debug.LogError("Error trying to place piece where piece already is.");
            return false;
        }

        if (pieceId >= 0)
        {
            GameObject newPiece = pieceBoard[xPos, yPos] = Instantiate(piecePrefabs[pieceId], hitBoxBoard[xPos, yPos].transform.position + (Vector3.up * verticalPlaceOffset), Quaternion.identity, gameObject.transform);//instantiate piece and place in pieceBoard location
            Piece piece = newPiece.GetComponent<Piece>();
            newPiece.transform.GetChild(1).GetComponent<MeshRenderer>().material = TorokPieceMats[(int)piece.type];
            if ((int)piece.type <= (int)Piece.PieceType.king)
            {
                newPiece.transform.rotation = Quaternion.Euler(0, 180, 0);
            }

            piece.pieceX = xPos;
            piece.pieceY = yPos;
            piece.isTorok = true;
        }
        else
        {
            Debug.LogError("Couldn't place piece: unrecognized pieceId");
        }
        return true;
    }

    public bool PlacePieceTorok(Transform boardSpot, int pieceId)
    {
        //**should reformat this function cuz im sure there is some getComponent overlapping**

        if (!boardSpot)
        {
            Debug.LogError("Trying to place piece, given piece transform was null");
            return false;
        }

        int placeX = -1;
        int placeY = -1;
        if (boardSpot.CompareTag("Chess Board"))
        {
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    if (boardSpot.gameObject == hitBoxBoard[i, j])//get position of piece in array
                    {
                        placeX = i;//store locations
                        placeY = j;

                    }
                }
            }

            if (placeX == -1 && placeY == -1)
            {
                Debug.LogError("Error trying to place piece where piece already is.");
                return false;
            }
        }
        else if (boardSpot.CompareTag("Chess Piece"))
        {
            Piece getPosPiece = boardSpot.GetComponent<Piece>();
            if (getPosPiece)
            {
                placeX = getPosPiece.pieceX;
                placeY = getPosPiece.pieceY;
            }
        }

        if (pieceBoard[placeX, placeY] != null && pieceId != -1)
        {
            Debug.LogError("Did not place piece because piece was already there");
            return false;
        }

        if (pieceId >= 0)
        {
            GameObject newPiece = pieceBoard[placeX, placeY] = Instantiate(piecePrefabs[pieceId], boardSpot.position + (Vector3.up * verticalPlaceOffset), Quaternion.identity, gameObject.transform);//instantiate piece and place in pieceBoard location
            Piece piece = newPiece.GetComponent<Piece>();
            newPiece.transform.GetChild(1).GetComponent<MeshRenderer>().material = TorokPieceMats[(int)piece.type];
            if (piece.type == Piece.PieceType.knight)
            {
                print("get rotated nerd");
                newPiece.transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            piece.isTorok = true;

            Debug.Log("placed piece type: " + piece.type);

            if (torokPiece)
            {
                piece.isTorok = true;
            }

            if (toughPlacer)
            {
                piece.isTough = true;
            }
            if (lastChancePlacer)
            {
                piece.lastChance = true;
            }
            if (promotePlacer)
            {
                piece.promote = true;
            }

            piece.pieceX = placeX;
            piece.pieceY = placeY;
        }
        else
        {
            //do any inventory stuff here
            Destroy(pieceBoard[placeX, placeY]);
            pieceBoard[placeX, placeY] = null;
        }
        return true;
    }

    public void PlaceObstacle(int xPos, int yPos, int obstacleId)
    {
        if (obstacleId >= 0 && obstacleId < obstaclePrefabs.Length)
        {
            GameObject newPiece = pieceBoard[xPos, yPos] = Instantiate(obstaclePrefabs[obstacleId], hitBoxBoard[xPos, yPos].transform.position + (Vector3.up * verticalPlaceOffset), Quaternion.identity, gameObject.transform);//instantiate obstacle and place in pieceBoard location
            Piece piece = newPiece.GetComponent<Piece>();
            piece.pieceX = xPos;
            piece.pieceY = yPos;
            if (obstacleId == 1)//if its a hole, remove renderer of that tile
            {
                hitBoxBoard[xPos, yPos].GetComponent<MeshRenderer>().enabled = false;
            } 

        }
        else
        {
            Debug.Log("Place Error| Could not place obstacle: obstacle ID not recognized");
        }
    }

    private void BuildBoard()//generates hitbox for chess board
    {
        float boardOffset = ((float)boardSize)/2;
        for(int i=0;i<boardSize;i++)
        {
            for(int j=0;j<boardSize;j++)
            {
                GameObject newTile = null;
                if ( (i+j) % 2 == 0)
                {
 
                    newTile = Instantiate(boardTiles[0], (boardPosition + new Vector3(i - boardOffset, 0, j - boardOffset)) + (Vector3.up * boardVerticalOffset) , Quaternion.Euler(new Vector3(-90f,0f,90f)), gameObject.transform);
                }
                else
                {
                    newTile = Instantiate(boardTiles[1], (boardPosition + new Vector3(i - boardOffset, 0, j - boardOffset)) + (Vector3.up * boardVerticalOffset), Quaternion.Euler(new Vector3(-90f, 0f, 90f)), gameObject.transform);
                }

                newTile.gameObject.name = i + "_" + j;
                hitBoxBoard[i, j] = newTile;
                GameObject moveTileObject = Instantiate(moveTile, (boardPosition + new Vector3(i - boardOffset, 0, j - boardOffset)) + ((Vector3.up * 0.149f)), Quaternion.Euler(new Vector3(90f, 0f, 0f)), gameObject.transform);
                moveTileObject.gameObject.name = i + "_" + j + "_MoveTile";
                moveTileBoard[i,j] = moveTileObject;
                moveTileObject.SetActive(false);

                GameObject winTileObject = Instantiate(winSpotTile, (boardPosition + new Vector3(i - boardOffset, 0, j - boardOffset)) + ((Vector3.up * 0.149f)), Quaternion.Euler(new Vector3(90f, 0f, 0f)), gameObject.transform);
                winTileObject.gameObject.name = i + "_" + j + "_WinSpot";
                winSpotBoard[i,j] = winTileObject;
                winTileObject.SetActive(false);
            }

        }
        
    }

    //resets the rendering on tiles, holes take it away etc
    public void ResetTiles()
    {
        for (int i = 0; i < boardSize; i++)
        {
            for (int j = 0; j < boardSize; j++)
            {
                hitBoxBoard[i, j].GetComponent<MeshRenderer>().enabled = true;
            }
        }
    }

    public void ActivateWinTiles()
    {
        for(int i = 0;i < winLocations.Count;i++)
        {
            winSpotBoard[winLocations[i].x, winLocations[i].y].SetActive(true);
        }
    }

    public void DeactivateWinTiles()
    {
        for(int i = 0;i < boardSize;i++)
        {
            for(int j = 0;j < boardSize;j++)
            {
                winSpotBoard[i,j].SetActive(false);
            }
        }
    }

    public void MoveValidatorCoRo(int pieceX, int pieceY, int endX, int endY)
    {
        StartCoroutine(MovePieceValidatorCoRo(pieceX, pieceY, endX, endY));
    }

    public IEnumerator MovePieceValidatorCoRo(int pieceX, int pieceY, int endX, int endY)
    {
        //Debug.Log("inside coro");
        canMove= false;
        if (pieceBoard[pieceX, pieceY] == null)//guard clause if piece given is null
        {
            GameStateManager.lastValidateCheck = false;
            canMove = true;
            yield break;
        }

        //find type of piece
        Piece pieceScript = pieceBoard[pieceX, pieceY].GetComponent<Piece>();

        pieceScript.pieceX = pieceX;
        pieceScript.pieceY = pieceY;


        pieceScript.UpdateMoves();

        foreach (Move move in pieceScript.moves)
        {
            if ((move.endX == endX) && (move.endY == endY))
            {
                Vector3 startIndicatorPos = hitBoxBoard[move.startX, move.startY].transform.position;
                startIndicatorPos.y = 0.113f;
                moveStartIndicator.transform.position = startIndicatorPos;

                Vector3 endIndicatorPos = hitBoxBoard[move.endX, move.endY].transform.position;
                endIndicatorPos.y = 0.113f;
                moveEndIndicator.transform.position = endIndicatorPos;

                bool pieceAtEndLocation = pieceBoard[endX,endY] != null;

                yield return MovePieceVisual(pieceX, pieceY, endX, endY);

                //print("Confirmed valid move");
                //print("endX :" + endX + "endY: " + endY + " " + move.DisplayMove());
                //canMove = true;
                //MovePieceVisualTeleport(pieceX, pieceY, endX, endY);
                MovePiece(pieceX, pieceY, endX, endY);

                //this might get replaced when the piece actually moves visually with coro, to inside that coro
                if (pieceAtEndLocation)
                {
                    float rand = Random.Range(0, 1);
                    if (pieceScript.isTorok && TorokPersonalityAI.instance.ShouldPlay(SoundLibrary.Categories.TakesPiece, rand))
                    {
                        TorokPersonalityAI.instance.PlayAnimationAndSound(SoundLibrary.Categories.TakesPiece);
                    }
                    else if (!pieceScript.isTorok && TorokPersonalityAI.instance.ShouldPlay(SoundLibrary.Categories.LosesPiece, rand))
                    {
                        TorokPersonalityAI.instance.PlayAnimationAndSound(SoundLibrary.Categories.LosesPiece);
                    }
                }

                GameStateManager.lastValidateCheck = true;
                canMove = true;
                GameStateManager.instance.EndTurn();
                yield break;
            }
        }
        // print("move not valid");
        GameStateManager.lastValidateCheck = false;
        canMove = true;
        yield break;

    }

    public bool MoveValidator(int pieceX, int pieceY, int endX, int endY)
    {
        //print("initX: " + pieceX + "initY: " + pieceY + "endX: " + endX + "endY: " + endY);
        if (pieceBoard[pieceX, pieceY] == null)//guard clause if piece given is null
        {
            return false;
        }

        //find type of piece
        Piece pieceScript = pieceBoard[pieceX, pieceY].GetComponent<Piece>();
       
        pieceScript.pieceX = pieceX;
        pieceScript.pieceY = pieceY;

        
        pieceScript.UpdateMoves();

        foreach (Move move in pieceScript.moves)
        {
            if ((move.endX == endX) && (move.endY == endY))
            {
                //print("Confirmed valid move");
                //print("endX :" + endX + "endY: " + endY + " " + move.DisplayMove());
                canMove = true;
                MovePieceVisualTeleport(pieceX, pieceY, endX, endY);
                MovePiece(pieceX, pieceY, endX, endY);

                Vector3 startIndicatorPos = hitBoxBoard[move.startX, move.startY].transform.position;
                startIndicatorPos.y = 0.032f;
                moveStartIndicator.transform.position = startIndicatorPos;

                Vector3 endIndicatorPos = hitBoxBoard[move.endX, move.endY].transform.position;
                endIndicatorPos.y = 0.032f;
                moveEndIndicator.transform.position = endIndicatorPos;

                //this might get replaced when the piece actually moves visually with coro, to inside that coro
                float rand = Random.Range(0,1);
                if (pieceScript.isTorok && TorokPersonalityAI.instance.ShouldPlay(SoundLibrary.Categories.TakesPiece, rand))
                {
                    TorokPersonalityAI.instance.PlayAnimationAndSound(SoundLibrary.Categories.TakesPiece);
                }
                else if (!pieceScript.isTorok && TorokPersonalityAI.instance.ShouldPlay(SoundLibrary.Categories.LosesPiece, rand))
                {
                    TorokPersonalityAI.instance.PlayAnimationAndSound(SoundLibrary.Categories.LosesPiece);
                }

                return true;
            }
        }
       // print("move not valid");
        return false;
    }

    public IEnumerator MovePieceVisual(int startX, int startY, int endX, int endY)
    {
        GameObject piece = pieceBoard[startX, startY];
        if (piece == null)//get out if null
        {
            yield break;
        }

        float percentMoved = 0.0f;
        Vector3 targetPos = hitBoxBoard[endX, endY].transform.position;
        targetPos.y += verticalPlaceOffset;
        float elapsedTime = 0.0f;
        float desiredDuration = 1f;
        Vector3 startPos = hitBoxBoard[startX, startY].transform.position;
        startPos.y += verticalPlaceOffset;

        while (percentMoved < 1.0f)
        {
            //Debug.Log("Moving");

            elapsedTime += Time.deltaTime * pieceMoveSpeed;
            percentMoved = elapsedTime / desiredDuration;
            piece.transform.position = Vector3.Lerp(startPos, targetPos, percentMoved);
            Debug.Log(percentMoved);
            yield return null;
        }
        piece.transform.position = targetPos;
        Debug.Log("Finished moving");
    }

    //input the X and Y of the piece being moved(startX and Y) and the X and Y of the spot being moved to(end X Y)
    //if click impossible move then clear storedmove
    //if click off bord then clear stored item
    public void MovePiece(int startX, int startY, int endX, int endY)//take 2 positions to move a piece
    {

        //Debug.Log("movepiece called");
        
        bool willPromote = false;
        bool lastChanceCheck = false;

        bool movingTorok = false;
        bool takingTorok = false;

        bool movingPromote = false;
        bool takenPromote = false;

        bool movingTough = false;
        bool takenTough = false;

        bool movingLastChance = false;
        bool takenLastChance = false;

        bool takenPieceMoved = false;

        int pieceIdMoving = 0;
      
        GameObject tempPiece = pieceBoard[startX, startY];
        GameObject tempEndPiece = pieceBoard[endX, endY];
        
        if (tempPiece == null)
        {
            Debug.LogError("Given start piece position did not direct to an active piece.");
            return;
        }

        Piece piece = pieceBoard[startX, startY].GetComponent<Piece>();

        movingTough = piece.isTough;
        movingPromote = piece.promote;
        movingLastChance = piece.lastChance;

        pieceIdMoving = (int)(piece.type) + 1;
        if(piece.isTorok)
        {
            //Debug.Log("torok moving piece ID: " + pieceIdMoving);
            movingTorok = true;
        }


        int pieceIdTaken = 0;
        if (pieceBoard[endX, endY] != null)//if a piece is captured
        {
            Piece pieceForCaptureId = pieceBoard[endX, endY].GetComponent<Piece>();


            takenTough = pieceForCaptureId.isTough;
            takenPromote = pieceForCaptureId.promote;
            takenLastChance = pieceForCaptureId.lastChance;
            takenPieceMoved = pieceForCaptureId.moved;

            pieceIdTaken = (int)(pieceForCaptureId.type) + 1;
            //print("taken ID " + pieceIdTaken);
            if (pieceForCaptureId.isTorok)
            {
                takingTorok = true;
            }
            //Piece oldPiece = tempEndPiece.GetComponent<Piece>();

            if (pieceForCaptureId.lastChance)//if piece being taken has last chance trait
            {
                if (piece.value <= pieceForCaptureId.value)
                {
                    lastChanceCheck = true;
                }
            }
            if(piece.promote)//if piece taking has promote trait
            {
                willPromote = true;
            }

            Destroy(tempEndPiece);
            pieceBoard[endX, endY] = null;
        }



        //stores move in a list so can be undone at any point
        //moveList.Add(new Move(startX, startY, endX, endY, tempPiece, tempEndPiece, pieceIdTaken)); // moveList is a list of the moves done

        undoCounter++;

        bool oldIsTorok = piece.isTorok;
        bool oldIsTough = piece.isTough;
        bool oldLastChance = piece.lastChance;
        

        moveList.Add(new Move(startX, startY, endX, endY, pieceIdMoving, pieceIdTaken, willPromote, movingTorok, takingTorok, movingPromote, takenPromote, movingTough, takenTough, movingLastChance, lastChanceCheck, piece.moved, takenPieceMoved)); // moveList is a list of the moves done

        if(willPromote && !lastChanceCheck)//if this piece captured another piece and has promotion
        {
            if(piece.type != Piece.PieceType.queen)
            {
                PlacePiece(endX,endY,pieceIdMoving);

                Piece endPiece = pieceBoard[endX,endY].GetComponent<Piece>();//get piece script of object that moved
                
                if(piece.type == Piece.PieceType.rook)
                {
                    endPiece.promote = false;
                }
                else
                {
                    endPiece.promote = willPromote;
                }

                endPiece.isTorok = oldIsTorok;
                endPiece.isTough = oldIsTough;
                endPiece.lastChance = oldLastChance;

                endPiece.moved = true;// changes piece to say has moved
                endPiece.pieceX = endX;//alter x pos to new x pos for moved piece
                endPiece.pieceY = endY;//alter x pos to new y pos for moved piece

                Destroy(pieceBoard[startX, startY]);
                pieceBoard[startX, startY] = null;

            }
        }
        else if(!lastChanceCheck)
        {
            //Debug.Log("regular move");
            //PlacePiece(endX,endY, pieceIdMoving-1);
            pieceBoard[startX, startY].transform.position = hitBoxBoard[endX, endY].transform.position + (Vector3.up * verticalPlaceOffset);
            pieceBoard[endX, endY] = pieceBoard[startX, startY];
            pieceBoard[startX, startY] = null;

            Piece endPiece = pieceBoard[endX,endY].GetComponent<Piece>();//get piece script of object that moved
                
            //endPiece.promote = willPromote;
            //endPiece.isTorok = oldIsTorok;
            //endPiece.isTough = oldIsTough;
            //endPiece.lastChance = oldLastChance;

            endPiece.moved = true;// changes piece to say has moved
            endPiece.pieceX = endX;//alter x pos to new x pos for moved piece
            endPiece.pieceY = endY;//alter x pos to new y pos for moved piece
        }
        else
        {
            Destroy(pieceBoard[startX, startY]);
            pieceBoard[startX, startY] = null;
        }

    }

    public void MovePieceVisual(int startX, int startY, int endX, int endY, GameObject piece, bool promoteCheck)
    {
        //just moves in move now, but very fast

        //have future plan for this

        /*
        Debug.Log("WILL THE VISUAL MOVE BE DONE: "+promoteCheck);
        if(!promoteCheck)
        {
            StartCoroutine(VisualMovePiece(startX, startY,endX,endY, piece, promoteCheck));
        }
        */
        //GameStateManager.EndTurn();//so that who ever's turn it is ends when the piece has finished moving
        
    }

    public void MovePieceVisualTeleport(int startX, int startY, int endX, int endY)
    {
        Vector3 newPos = hitBoxBoard[endX, endY].transform.position;
        newPos.y += verticalPlaceOffset;
        pieceBoard[startX, startY].transform.position = newPos;


    }

    public void DisablePiece(GameObject piece)//is this even used anymore?
    {
        if (piece)
        {
            piece.SetActive(false);
            //print("disabled piece");
        }
    }

    //undoes most recent move
    //repeatedly calling will undo moves until beggining
    public void UndoMove()
    {
        //Debug.Log("Undo called");

        //GameObject startingPiece = null;
        GameObject endPiece = null;

        if (moveList.Count < 1)
        {
            Debug.LogError("List is empty, no undo occurred");
            return;
        }

        if (moveList[moveList.Count - 1].takenLastChance)
        {
            PlacePiece(moveList[moveList.Count - 1].startX, moveList[moveList.Count - 1].startY, moveList[moveList.Count - 1].pieceMoving - 1);
        }
        else if (moveList[moveList.Count - 1].promoted)
        {
            Destroy(pieceBoard[moveList[moveList.Count - 1].endX, moveList[moveList.Count - 1].endY]);
            pieceBoard[moveList[moveList.Count - 1].endX, moveList[moveList.Count - 1].endY] = null;
            PlacePiece(moveList[moveList.Count - 1].endX, moveList[moveList.Count - 1].endY, moveList[moveList.Count - 1].pieceMoving - 1);
            MovePieceVisualTeleport(moveList[moveList.Count - 1].endX, moveList[moveList.Count - 1].endY, moveList[moveList.Count - 1].startX, moveList[moveList.Count - 1].startY);
            pieceBoard[moveList[moveList.Count - 1].startX, moveList[moveList.Count - 1].startY] = pieceBoard[moveList[moveList.Count - 1].endX, moveList[moveList.Count - 1].endY];
            pieceBoard[moveList[moveList.Count - 1].endX, moveList[moveList.Count - 1].endY] = null;            
        }
        else
        {
            MovePieceVisualTeleport(moveList[moveList.Count - 1].endX, moveList[moveList.Count - 1].endY, moveList[moveList.Count - 1].startX, moveList[moveList.Count - 1].startY);
            pieceBoard[moveList[moveList.Count - 1].startX, moveList[moveList.Count - 1].startY] = pieceBoard[moveList[moveList.Count - 1].endX, moveList[moveList.Count - 1].endY];
            pieceBoard[moveList[moveList.Count - 1].endX, moveList[moveList.Count - 1].endY] = null;
        }






        
        if (moveList[moveList.Count - 1].pieceTaken > 0)
        {
            if (!moveList[moveList.Count - 1].takenTorok)
            {
                PlacePiece(moveList[moveList.Count - 1].endX, moveList[moveList.Count - 1].endY, moveList[moveList.Count - 1].pieceTaken - 1);
            }
            else if (moveList[moveList.Count - 1].takenTorok)
            {
                PlacePieceTorok(moveList[moveList.Count - 1].endX, moveList[moveList.Count - 1].endY, moveList[moveList.Count - 1].pieceTaken - 1);
            }
        }
        

        if(moveList[moveList.Count -1].pieceTaken > 0)
        {
            endPiece = pieceBoard[moveList[moveList.Count -1].endX, moveList[moveList.Count -1].endY];
            Piece endScript = endPiece.GetComponent<Piece>();
            endScript.isTough = moveList[moveList.Count -1].takenTough;
            endScript.promote = moveList[moveList.Count -1].takenPromote;
            endScript.lastChance = moveList[moveList.Count -1].takenLastChance;
            endScript.moved = moveList[moveList.Count -1].takenPieceSetFirstMove;
        }

        Piece startPosPiece = pieceBoard[moveList[moveList.Count - 1].startX, moveList[moveList.Count - 1].startY].GetComponent<Piece>();

        if(moveList[moveList.Count - 1].promoted || moveList[moveList.Count - 1].takenLastChance)
        {
            startPosPiece.isTough = moveList[moveList.Count -1].movingTough;
            startPosPiece.promote = moveList[moveList.Count -1].movingPromote;
            startPosPiece.lastChance = moveList[moveList.Count -1].movingLastChance;
        }

        startPosPiece.moved = moveList[moveList.Count - 1].setFirstMove;

        moveList.RemoveAt(moveList.Count -1);

        //undoCounter--;

    }

    public void UndoMoveVisual()//visually show undo moves
    {
        /*
        if (moveList.Count < 1)//guard clause added by jordan to handle error that occurs when undostorage is empty:: delete this when you see it if its fine
        {
            return;
        }
        if (moveList[undoCounter - 1].startObject)
        {
            moveList[undoCounter - 1].startObject.SetActive(true);
        }
        if(moveList[undoCounter - 1].endObject)
        {
            moveList[undoCounter - 1].endObject.SetActive(true);
        }
        moveList[undoCounter-1].startObject.transform.position = hitBoxBoard[moveList[undoCounter-1].startX,moveList[undoCounter-1].startY].transform.position+(Vector3.up * verticalPlaceOffset);
        if(moveList[undoCounter-1].endObject)
        moveList[undoCounter-1].endObject.transform.position = hitBoxBoard[moveList[undoCounter-1].endX,moveList[undoCounter-1].endY].transform.position+ (Vector3.up * verticalPlaceOffset);
        */
        UndoMove();

    }
    //if click when movign it breaks
    //fix this later
    IEnumerator VisualMovePiece(int startX, int startY, int endX, int endY, GameObject piece, bool promoted)
    {
        //print("inside visualMove");
        if(promoted)
        {
            Debug.Log("was promoted");
        }
        piece = pieceBoard[endX,endY];

            while (Vector3.Distance(piece.transform.position, hitBoxBoard[endX, endY].transform.position + (Vector3.up * verticalPlaceOffset)) > 0.1f)
            {
                Debug.Log("running");
                piece.transform.position = Vector3.MoveTowards(piece.transform.position, hitBoxBoard[endX, endY].transform.position + (Vector3.up * verticalPlaceOffset), pieceMoveSpeed * Time.deltaTime);
                yield return null;
            }
            piece.transform.position = hitBoxBoard[endX, endY].transform.position + (Vector3.up * verticalPlaceOffset);

        //print("visual move has ended");
    }

    public static GameObject[,] GetPieceBoard()
    {
        return pieceBoard;
    }

    public bool InCheckMate(bool lookingAtTorok)//if false, looking if player is in check, if true looking if torok is in check
    {
        if (lookingAtTorok)
        {
            if (torokInCheck && GetAllMoves(lookingAtTorok).Count <= 0)
            {
                return true;
            }
        }
        else
        {
            if (playerInCheck && GetAllMoves(lookingAtTorok).Count <= 0)
            {
                return true;
            }
        }


        return false;
    }

    public static int GetSize()
    {
        return boardSize;
    }

    //added by jordan to get all moves on the board of a certain player
    public List<Move> GetAllMoves(bool toroksPieces)//true is torrok
    {
        List<Move> returnArray = new List<Move>();

        for (int i = 0; i < boardSize; i++)//go through board array
        {
            for (int j = 0; j < boardSize; j++)
            {
                if (pieceBoard[i,j] == null) continue;//if piece doesnt exist skip
                if (!pieceBoard[i,j].activeInHierarchy) { continue; }// might be able to remove this once the deactivate captured piece thing is resolved, depending on how that is done

                Piece piece = pieceBoard[i,j].GetComponent<Piece>();

                if (toroksPieces && piece.isTorok)//looking for toroks pieces and is toroks piece
                {
                    piece.UpdateMoves();
                    returnArray.AddRange(piece.moves);
                }
                else if (!toroksPieces && !piece.isTorok)//looking for players pieces and is players piece
                {
                    piece.UpdateMoves();
                    returnArray.AddRange(piece.moves);
                }
                
            }
        }

        return returnArray;
    }

    public List<Move> GetCapturingMoves(List<Move> moves) // wait i need to rewrite this sort of
    {
        List<Move> capturingMoves = new List<Move>();

        foreach (Move m in moves)
        {
            if (m.capturedPiece != -1)
            {
                capturingMoves.Add(m);
            }
        }

        return capturingMoves;
    }

    //returns the location of a gameobject inside the pieceboard if it exists, or -1,-1 if it doesnt
    public Vector2Int GetPieceLocation(GameObject piece)
    {
        if (piece == null) { return new Vector2Int(-1, -1); }//guard clause

        for (int i = 0; i < boardSize; i ++)//find piece in matrix
        {
            for (int j = 0; j < boardSize; j++)
            {
                if (pieceBoard[i,j] == null) { continue; }//if no piece in spot move on

                if (piece == pieceBoard[i,j])//return piece if ref match
                {
                    return new Vector2Int(i, j);
                }
            }
        }
        return new Vector2Int(-1, -1);
    }

    public static bool isSameteam(GameObject chessPiece, GameObject chessPiece2)
    {
        if (!chessPiece || !chessPiece2) { return false; }

        Piece piece1 = chessPiece.GetComponent<Piece>();
        Piece piece2 = chessPiece2.GetComponent<Piece>();

        if (!piece1 || !piece2) { return false; }


        return (piece1.isTorok && piece2.isTorok) || (!piece1.isTorok && !piece2.isTorok);
    }

    public void ClearBoard()
    {
        for (int i = 0; i < boardSize; i++)
        {
            for (int j = 0; j < boardSize; j++)
            {
                if (pieceBoard[i,j] == null) { continue; }

                Destroy(pieceBoard[i,j]);
                pieceBoard[i,j] = null;
            }
        }

    }

    public void TorokPlacementButton()
{
    if(torokPiece)
    {
        torokPiece = false;
        text.text = "Placing Player";

        if(toughPlacer)
        {
            ToughButtonSet();
        }
        if(lastChancePlacer)
        {
            LastChanceButtonSet();
        }
        if(promotePlacer)
        {
            PromoteButtonSet();
        }

        toughButton.SetActive(false);
        lastChanceButton.SetActive(false);
        promoteButton.SetActive(false);


    }
    else if(!torokPiece)
    {
        torokPiece = true;
        text.text = "Placing Torok";

        toughButton.SetActive(true);
        lastChanceButton.SetActive(true);
        promoteButton.SetActive(true);
    }
}

    public void ToughButtonSet()
    {   
        if(toughPlacer)
        {
            toughText.text = "Tough Deactivated";
            toughPlacer = false;
        }
        else
        {
            toughText.text = "Tough Activated";
            toughPlacer = true;
        }
    } 

    public void LastChanceButtonSet()
    {
        if(lastChancePlacer)
        {
            lastChanceText.text = "Last Chance Deactivated";
            lastChancePlacer = false;
        }
        else
        {
            lastChanceText.text = "Last Chance Activated";
            lastChancePlacer = true;
        }
    }

    public void PromoteButtonSet()
    {
        if (promotePlacer)
        {
            promoteText.text = "Promote Deactivated";
            promotePlacer = false;
        }
        else
        {
            promoteText.text = "Promote Activated";
            promotePlacer = true;
        }
    }

    public void PrintInternalPieceBoard()
    {
        string resultLine = "";
        for (int i = boardSize-1; i > -1; i--)
        {
            for (int j = 0; j < boardSize; j++)
            {
                if (pieceBoard[j,i] != null)
                {
                    resultLine += " " + (int)pieceBoard[j,i].GetComponent<Piece>().type;
                }
                else
                {
                    resultLine += " -";
                }
            }
            print(resultLine);
            resultLine = "";
        }
    }

    private void ClearMoveTiles()
    {
                for(int i = 0;i < boardSize;i++)
                {
                    for(int j = 0;j < boardSize;j++)
                    {
                        moveTileBoard[i,j].SetActive(false);
                    }

                }
    }

    public void ReturnPiecesToInventory()
    {
        for(int i = 0;i < boardSize;i++)
        {
            for(int j = 0; j < boardSize;j++)
            {
                if(pieceBoard[i,j])
                {
                    if(!pieceBoard[i,j].GetComponent<Piece>().isTorok && (int)pieceBoard[i,j].GetComponent<Piece>().type < 5)
                    {
                        Inventory.instance.AlterPiece((Inventory.InventoryPieces)pieceBoard[i,j].GetComponent<Piece>().type, 1);
                    }
                }
            }
        }
    }

    public bool IsKingInCheck(bool checkingTorok)
    {
        //there should probably be 2 bools for what side has a king, cuz then this check doesnt need to happen, and its a pretty lengthy check

        //find king location
        Vector2Int kingPos = FindKing(checkingTorok);

        if (kingPos.x < 0 || kingPos.y < 0)//no king found
        {
            return false;
        }

        //search vertical and horizontal for rooks/queen
        if (SearchLineForCheckingKingPiece(0,1, kingPos, checkingTorok) || SearchLineForCheckingKingPiece(0, -1, kingPos, checkingTorok) 
            || SearchLineForCheckingKingPiece(1, 0, kingPos, checkingTorok) || SearchLineForCheckingKingPiece(-1, 0, kingPos, checkingTorok))
        {
            return true;
        }
        //search diagonal for bishop/queen/maybe pawn
        if (SearchLineForCheckingKingPiece(1, 1, kingPos, checkingTorok) || SearchLineForCheckingKingPiece(1, -1, kingPos, checkingTorok) 
            || SearchLineForCheckingKingPiece(-1, 1, kingPos, checkingTorok) || SearchLineForCheckingKingPiece(-1, -1, kingPos, checkingTorok))
        {
            return true;
        }
        //search L for knights
        if (SearchLForCheckingKingPiece(kingPos, checkingTorok))
        {
            return true;
        }

        return false;
    }

    private bool SearchLForCheckingKingPiece(Vector2Int kingPos, bool checkingTorok)
    {
        for (int hOffset = -2; hOffset < 3; hOffset++)
        {
            if (hOffset == 0) { continue; }
            int yLocation = 2;
            if (hOffset % 2 == 0) { yLocation = 1; }

            //print((kingPos.x + hOffset) + ", " + (kingPos.y + yLocation));
            //print((kingPos.x + hOffset) + ", " + (kingPos.y - yLocation));

            if (Piece.InBoundsCheck(kingPos.x + hOffset, kingPos.y + yLocation) && pieceBoard[kingPos.x + hOffset, kingPos.y + yLocation] != null)// upper knight positions
            {
                Piece lookingAt = pieceBoard[kingPos.x + hOffset, kingPos.y + yLocation].GetComponent<Piece>();

                if (lookingAt.type == Piece.PieceType.knight && lookingAt.isTorok != checkingTorok) { return true; }
            }
            if (Piece.InBoundsCheck(kingPos.x + hOffset, kingPos.y - yLocation) && pieceBoard[kingPos.x + hOffset, kingPos.y - yLocation] != null)//lower knight positions
            {
                Piece lookingAt = pieceBoard[kingPos.x + hOffset, kingPos.y - yLocation].GetComponent<Piece>();

                if (lookingAt.type == Piece.PieceType.knight && lookingAt.isTorok != checkingTorok) { return true; }
            }
        }
        return false;
    }

    private bool SearchLineForCheckingKingPiece(int dirX, int dirY, Vector2Int kingPos, bool checkingTorok)
    {
        for (int offset = 1; offset < boardSize; offset++)
        {
            if (!Piece.InBoundsCheck(kingPos.x + (dirX * offset), kingPos.y + (dirY * offset)))//if not in bounds return false, cuz end of search in direciton dirX, dirY
            {
                return false;
            }

            if (pieceBoard[kingPos.x + (dirX * offset), kingPos.y + (dirY * offset)] == null)// null guard
            {
                continue;
            }

            Piece lookingAt = pieceBoard[kingPos.x + (dirX * offset), kingPos.y + (dirY * offset)].GetComponent<Piece>();

            if (lookingAt.isTorok == checkingTorok)// is ally piece
            {
                return false;
            }

            //is diagonal check or horizontal/vertical check
            if (dirX == 0 || dirY == 0)//is hor/vert check
            {
                //print("inside hor/vert check");
                if (lookingAt.type == Piece.PieceType.rook || lookingAt.type == Piece.PieceType.queen || lookingAt.type == Piece.PieceType.king)//if looking at is rook/queen then king is in check
                {
                    return true;
                }
            }
            else//is diagonal check
            {
                //print("inside diagonal check");
                if (lookingAt.type == Piece.PieceType.bishop || lookingAt.type == Piece.PieceType.queen || lookingAt.type == Piece.PieceType.king)//looking at is a bishop/queen, then king is in check
                {
                    return true;
                }
                else if (lookingAt.type == Piece.PieceType.pawn)// if looking at is a pawn
                {
                    if (checkingTorok && offset == 1 && dirY == -1)
                    {
                        return true;
                    }
                    else if (!checkingTorok && offset == 1 && dirY == 1)
                    {
                        return true;
                    }
                }
            }

            return false;

        }

        return false;
    }

    private Vector2Int FindKing(bool checkingTorok)
    {
        Vector2Int kingPos = new Vector2Int(-1, -1);
        for (int xPos = 0; xPos < boardSize; xPos++)
        {
            for (int yPos = 0; yPos < boardSize; yPos++)
            {
                if (pieceBoard[xPos, yPos] == null) { continue; }//null guard

                Piece lookingAt = pieceBoard[xPos, yPos].GetComponent<Piece>();

                if (lookingAt.type == Piece.PieceType.king)
                {
                    if (checkingTorok && lookingAt.isTorok)
                    {
                        kingPos.Set(xPos, yPos);
                        return kingPos;
                    }
                    else if (!checkingTorok && !lookingAt.isTorok)
                    {
                        kingPos.Set(xPos, yPos);
                        return kingPos;
                    }
                }
            }
        }
        return kingPos;
    }


}

