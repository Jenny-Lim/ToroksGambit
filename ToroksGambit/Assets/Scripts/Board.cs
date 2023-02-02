using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//create higher object for game. game manager
//cant move to spot you start in
//fix bug where it wont move after capturinig or when it cant capture

//place piece on enemy piece
//fix bug where it deletes friendly after clicking

//inventory - text in corner of boxes for counter - ticker to set to torok pieces being set down
//piece trait system

//traits
//Tough - cannot be captured by a pawn
//Promote - when captured it is instead upgraded to a higher piece
//last chance - if captured by equal or lower value then that piece is captured as well

//rebuidling movepiece

public class Board : MonoBehaviour
{
    public static int boardSize = 8;//size of 2D array
    [SerializeField] float boardVerticalOffset = 0.5f;//offset for board tiles vertically
    [SerializeField] float verticalPlaceOffset = 0.5f;

    private GameObject[,] hitBoxBoard;//array for hitboxes for raycasting

    public static GameObject[,] pieceBoard;//array for storing pieces and piece location -- made static (jenny)

    [SerializeField]
    private GameObject boardSquare;

    [SerializeField] private GameObject[] boardTiles = new GameObject[2];

    [SerializeField]
    private GameObject chessPiece;

    private Camera cam;

    private GameObject clickedPiece; 

    public List<Move> moveList = new List<Move>();

    private int undoCounter = 0;

    private Vector3 boardPosition;

    public bool canMove = false;

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
        BuildBoard();
    }

    public void BoardUpdate()
    {
        // print("in bvoard update");

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
            if (hit.transform.tag == "Chess Piece")//if mouse is clicked on chess piece
            {

                // Debug.Log("Piece");
                GameObject tempPiece = hit.transform.gameObject;//removed the ,parent cuz i changed the hitbox to be on the highest level of the piece prefabs - jordan

                Piece piece = tempPiece.GetComponent<Piece>(); 

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

                piece.UpdateMoves();

                //signifier that piece is chosen
                //storedPiece.transform.position = Vector3.MoveTowards(hit.transform.position, hit.transform.position+new Vector3(0,5,0), 10f * Time.deltaTime);

            }
            else if((hit.transform.tag == "Chess Board" || hit.transform.tag == "Chess Piece") && clickedPiece)//if a piece is stored and another spot is chosen
            {
                //Debug.Log("TEST");
                int clickedX = 0;
                int clickedY = 0;

                //clickedX = 0;//position for second click
                //clickedY = 0;

                for(int i=0;i<boardSize;i++)
                {
                    for(int j=0;j<boardSize;j++)
                    {
                        if(hit.transform.gameObject == hitBoxBoard[i,j])
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
                bool isValid = MoveValidator(pieceX, pieceY, clickedX, clickedY);

                if(isLastchance)
                    {
                        clickedPiece = null;
                    }

                if(canMove & isValid)
                {
                        //DisablePiece(tempPiece);
                        if(clickedPiece)
                        {
                            clickedPiece = pieceBoard[clickedX,clickedY];
                            MovePieceVisual(pieceX, pieceY, clickedX, clickedY, clickedPiece,false);
                           //StartCoroutine(VisualMovePiece(pieceX, pieceY, clickedX, clickedY, clickedPiece,false));
                        }
                        else
                        {
                            GameStateManager.EndTurn();
                            isLastchance = false;
                            isPromote = false;

                        }

                    }
                canMove = false;


                //storedPiece.transform.position = hit.transform.position + new Vector3(0,0,0);

                    // added by jenny
                //if (storedPiece.GetComponent<Piece>().type == "pawn")
                    //{
                        //storedPiece.GetComponent<Piece>().moved == true;
                    //}

                clickedPiece = null;

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
            UndoMoveVisual();
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

    public void PlacePiece(Transform boardSpot, int pieceId)
    {
        //**should reformat this function cuz im sure there is some getComponent overlapping**

        if (!boardSpot)
        {
            Debug.Log("Trying to place piece, given piece transform was null");
            return;
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
                return;
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
            return;
        }

        if (pieceId >= 0)
        {
            GameObject newPiece = pieceBoard[placeX, placeY] = Instantiate(piecePrefabs[pieceId], boardSpot.position + (Vector3.up * verticalPlaceOffset), Quaternion.identity, gameObject.transform);//instantiate piece and place in pieceBoard location
            Piece piece = newPiece.GetComponent<Piece>();

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
        } else
        {
            //do any inventory stuff here
            Destroy(pieceBoard[placeX, placeY]);
            pieceBoard[placeX, placeY] = null;
        }

    }

    public void PlacePiece(int xPos, int yPos, int pieceId)
    {
        //int pieceId = inventoryScript.GetStoredPiece();

        if (pieceBoard[xPos, yPos] != null && pieceId != -1)
        {
            Debug.LogError("Error trying to place piece where piece already is.");
            return;
        }

        if (pieceId >= 0)
        {
            GameObject newPiece = pieceBoard[xPos, yPos] = Instantiate(piecePrefabs[pieceId], hitBoxBoard[xPos,yPos].transform.position + (Vector3.up * verticalPlaceOffset), Quaternion.identity, gameObject.transform);//instantiate piece and place in pieceBoard location
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
    }

    public void PlacePieceTorok(int xPos, int yPos, int pieceId)
    {
        if (pieceBoard[xPos, yPos] != null)
        {
            Debug.Log("Trace Stack for ");
            Debug.LogError("Error trying to place piece where piece already is.");
            return;
        }

        if (pieceId >= 0)
        {
            GameObject newPiece = pieceBoard[xPos, yPos] = Instantiate(piecePrefabs[pieceId], hitBoxBoard[xPos, yPos].transform.position + (Vector3.up * verticalPlaceOffset), Quaternion.identity, gameObject.transform);//instantiate piece and place in pieceBoard location
            Piece piece = newPiece.GetComponent<Piece>();

            piece.pieceX = xPos;
            piece.pieceY = yPos;
            piece.isTorok = true;
        }
        else
        {
            Debug.Log("Couldn't place piece: unrecognized pieceId");
        }
    }

    public void PlaceObstacle(int xPos, int yPos, int obstacleId)
    {
        if (obstacleId >= 0 && obstacleId < obstaclePrefabs.Length)
        {
            GameObject newPiece = pieceBoard[xPos, yPos] = Instantiate(obstaclePrefabs[obstacleId], hitBoxBoard[xPos, yPos].transform.position + (Vector3.up * verticalPlaceOffset), Quaternion.identity, gameObject.transform);//instantiate obstacle and place in pieceBoard location
            Piece piece = newPiece.GetComponent<Piece>();
            piece.pieceX = xPos;
            piece.pieceY = yPos;
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
                    newTile = Instantiate(boardTiles[0], (boardPosition + new Vector3(i - boardOffset, 0, j - boardOffset)) + (Vector3.up * boardVerticalOffset) , Quaternion.Euler(new Vector3(90f,0f,0f)), gameObject.transform);
                }
                else
                {
                    newTile = Instantiate(boardTiles[1], (boardPosition + new Vector3(i - boardOffset, 0, j - boardOffset)) + (Vector3.up * boardVerticalOffset), Quaternion.Euler(new Vector3(90f, 0f, 0f)), gameObject.transform);
                }

                newTile.gameObject.name = i + "_" + j;
                hitBoxBoard[i, j] = newTile;
            }

        }
        
    }

    public bool MoveValidator(int pieceX, int pieceY, int endX, int endY)
    {
        print("initX: " + pieceX + "initY: " + pieceY + "endX: " + endX + "endY: " + endY);
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
                print("Confirmed valid move");
                //print("endX :" + endX + "endY: " + endY + " " + move.DisplayMove());
                canMove = true;
                MovePiece(pieceX, pieceY, endX, endY);
                return true;
            }
        }
        print("move not valid");
        return false;
    }

    
    //input the X and Y of the piece being moved(startX and Y) and the X and Y of the spot being moved to(end X Y)
    //if click impossible move then clear storedmove
    //if click off bord then clear stored item
    public void MovePiece(int startX, int startY, int endX, int endY)//take 2 positions to move a piece
    {
        
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
            movingTorok = true;
        }


        int pieceIdTaken = 0;
        if (pieceBoard[endX, endY] != null)//if a piece is captured
        {
            Piece pieceForCaptureId = pieceBoard[endX, endY].GetComponent<Piece>();

            takenTough = pieceForCaptureId.isTough;
            takenPromote = pieceForCaptureId.promote;
            takenLastChance = pieceForCaptureId.lastChance;

            pieceIdTaken = (int)(pieceForCaptureId.type) + 1;
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

        moveList.Add(new Move(startX, startY, endX, endY, pieceIdMoving, pieceIdTaken, willPromote, movingTorok, takingTorok, movingPromote, takenPromote, movingTough, takenTough, movingLastChance, takenLastChance)); // moveList is a list of the moves done

        if(willPromote && !lastChanceCheck)//if this piece captured another piece and has promotion
        {
            if(piece.type != Piece.PieceType.queen)
            {
                Debug.Log("PROMTOION");
                PlacePiece(endX,endY,((int)(piece.type) + 1));
            }
        }
        else if(!lastChanceCheck)//if it doesnt have that, that being which was written above, this not being that, that wouldnt make sense cause this isnt that. This is this.
        {
            Debug.Log("regular move");
            PlacePiece(endX,endY,((int)(piece.type)));

            Piece endPiece = pieceBoard[endX,endY].GetComponent<Piece>();//get piece script of object that moved
                
            endPiece.promote = willPromote;
            endPiece.isTorok = oldIsTorok;
            endPiece.isTough = oldIsTough;
            endPiece.lastChance = oldLastChance;

            endPiece.moved = true;// changes piece to say has moved
            endPiece.pieceX = endX;//alter x pos to new x pos for moved piece
            endPiece.pieceY = endY;//alter x pos to new y pos for moved piece
        }

        Destroy(pieceBoard[startX,startY]);
        pieceBoard[startX, startY] = null;

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
        GameStateManager.EndTurn();//so that who ever's turn it is ends when the piece has finished moving
        
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
        GameObject startingPiece = null;
        GameObject endPiece = null;

        if (moveList.Count < 1)//guard clause added by jordan to handle error that occurs when undostorage is empty:: delete this when you see it if its fine
        {
            Debug.Log("List is empty, no undo occurred");
            return;
        }

        //pieceBoard[moveList[undoCounter-1].startX,moveList[undoCounter-1].startY] = moveList[undoCounter-1].startObject;
        //pieceBoard[moveList[undoCounter-1].endX, moveList[undoCounter-1].endY] = moveList[undoCounter-1].endObject;

        //delete whats at positions now
        Destroy(pieceBoard[moveList[undoCounter-1].startX,moveList[undoCounter-1].startY]);
        Destroy(pieceBoard[moveList[undoCounter-1].endX, moveList[undoCounter-1].endY]);

        //clear old pieces from array
        pieceBoard[moveList[undoCounter-1].endX, moveList[undoCounter - 1].endY] = null;
        pieceBoard[moveList[undoCounter-1].startX, moveList[undoCounter - 1].startY] = null;

        //take piece ids for both pieces
        if(!moveList[undoCounter-1].movingTorok && moveList[undoCounter-1].pieceMoving > 0)
        {
            PlacePiece(moveList[undoCounter-1].startX,moveList[undoCounter-1].startY, moveList[undoCounter-1].pieceMoving - 1);
        }
        else if(moveList[undoCounter-1].movingTorok && moveList[undoCounter-1].pieceMoving > 0)
        {
            PlacePieceTorok(moveList[undoCounter-1].startX,moveList[undoCounter-1].startY, moveList[undoCounter-1].pieceMoving - 1);
        }

        if(moveList[undoCounter-1].pieceMoving > 0)
        {
            startingPiece = pieceBoard[moveList[undoCounter-1].startX, moveList[undoCounter - 1].startY];
            Piece startScript = startingPiece.GetComponent<Piece>();
            startScript.isTough = moveList[undoCounter-1].movingTough;
            startScript.promote = moveList[undoCounter-1].movingPromote;
            startScript.lastChance = moveList[undoCounter-1].movingLastChance;
        }

        if(!moveList[undoCounter-1].takingTorok && moveList[undoCounter-1].pieceTaken > 0)
        {
            PlacePiece(moveList[undoCounter-1].endX, moveList[undoCounter-1].endY, moveList[undoCounter-1].pieceTaken - 1);
        }
        else if(moveList[undoCounter-1].takingTorok && moveList[undoCounter-1].pieceTaken > 0)
        {
            PlacePieceTorok(moveList[undoCounter-1].endX, moveList[undoCounter-1].endY, moveList[undoCounter-1].pieceTaken - 1);
        }

        if(moveList[undoCounter-1].pieceTaken > 0)
        {
            endPiece = pieceBoard[moveList[undoCounter-1].endX, moveList[undoCounter-1].endY];
            Piece endScript = endPiece.GetComponent<Piece>();
            endScript.isTough = moveList[undoCounter-1].takenTough;
            endScript.promote = moveList[undoCounter-1].takenPromote;
            endScript.lastChance = moveList[undoCounter-1].takenLastChance;
        }
        //place new pieces at locations
        //MovingpieceId -> start
        //piecetakenId -> end
/*
        if(moveList[undoCounter-1].promoted)
        {

        }

        if (moveList[undoCounter-1].pieceTaken != 0)
        {
            if (moveList[undoCounter - 1].pieceTaken > 0)
            {
                Debug.Log( moveList[undoCounter - 1].pieceTaken - 1);
                pieceBoard[moveList[undoCounter-1].endX, moveList[undoCounter - 1].endY] = null;
                print("place player piece in undo");
                PlacePiece(moveList[undoCounter-1].endX, moveList[undoCounter - 1].endY, moveList[undoCounter - 1].pieceTaken - 1);
                
            }
            else
            {
                pieceBoard[moveList[undoCounter-1].endX, moveList[undoCounter - 1].endY] = null;
                print("place torok piece in undo");
                //moveList[undoCounter - 1].pieceTaken = moveList[undoCounter - 1].pieceTaken * -1;
                PlacePiece(moveList[undoCounter - 1].endX, moveList[undoCounter - 1].endY, moveList[undoCounter - 1].pieceTaken + 1);
                
            }
        }
        */

        moveList.RemoveAt(undoCounter-1);
        //Debug.Log("list legnth "+moveList.Count);

        undoCounter--;
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


    // added by jenny
    public static int GetX()
    {
        return pieceX;
    }

    public static int GetY()
    {
        return pieceY;
    }

    public static GameObject[,] GetPieceBoard()
    {
        return pieceBoard;
    }

    public static int GetSize()
    {
        return boardSize;
    }

    //added by jordan to get all moves on the board of a certain player
    public List<Move> GetAllMoves(bool toroksPieces)
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

    public static bool isSameteam(GameObject chessPiece)
    {

        return false;
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
}

