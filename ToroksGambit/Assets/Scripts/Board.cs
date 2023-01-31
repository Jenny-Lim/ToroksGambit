using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements.Experimental;
using UnityEngine.UI;
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

    private List<Move> moveList = new List<Move>();

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
                GameObject tempPiece = hit.transform.parent.gameObject;

                Piece piece = tempPiece.GetComponent<Piece>(); 

                if(!piece.isTorok)
                {
                    clickedPiece = hit.transform.parent.gameObject;//store piece

                    // Debug.Log(hit.transform.parent.gameObject);

                    for(int i=0;i<boardSize;i++)//doesnt appear to work, always returns as 0
                    {
                        for(int j=0;j<boardSize;j++)
                        {
                            if(hit.transform.parent.gameObject == pieceBoard[i,j])//get position of piece in array
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
                            Debug.Log(clickedPiece);
                            StartCoroutine(VisualMovePiece(pieceX, pieceY, clickedX, clickedY, clickedPiece));
                        }
                        else
                        {
                            GameStateManager.EndTurn();
                            isLastchance = false;

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
        //int pieceId = inventoryScript.GetStoredPiece();

        int placeX = -1;
        int placeY = -1;
        for(int i=0;i<boardSize;i++)
        {
            for(int j=0;j<boardSize;j++)
            {
                if(boardSpot.gameObject == hitBoxBoard[i,j])//get position of piece in array
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
        }else
        {
            //remove piece functionality
        }

    }

    public void PlacePiece(int xPos, int yPos, int pieceId)
    {
        //int pieceId = inventoryScript.GetStoredPiece();

        if (pieceBoard[xPos, yPos] != null)
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
        }

    }

    public void ToughButtonSet()
    {
        if(toughPlacer)
        {
            toughText.text = "Tough Activated";
            toughPlacer = false;
        }
        else
        {
            toughText.text = "Tough Deactivated";
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

    public void PlacePieceTorok(int xPos, int yPos, int pieceId)
    {
        if (pieceBoard[xPos, yPos] != null)
        {
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

    public void TorokPlacement()
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

    public bool MoveValidator(int pieceX, int pieceY, int endX, int endY)
    {
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
                //print("endX :" + endX + "endY: " + endY + " " + move.DisplayMove());
                canMove = true;
                MovePiece(pieceX, pieceY, endX, endY);
                return true;
            }
        }

        return false;
    }

    
    //input the X and Y of the piece being moved(startX and Y) and the X and Y of the spot being moved to(end X Y)
    //if click impossible move then clear storedmove
    //if click off bord then clear stored item
    public void MovePiece(int startX, int startY, int endX, int endY)//take 2 positions to move a piece
    {
      
        GameObject tempPiece = pieceBoard[startX, startY];
        GameObject tempEndPiece = pieceBoard[endX, endY];
        
        if (tempPiece == null)
        {
            Debug.LogError("Given start piece position did not direct to an active piece.");
            return;
        }

        Piece piece = tempPiece.GetComponent<Piece>();

        int pieceIdTaken = 0;
        if (pieceBoard[endX, endY] != null)
        {
            Piece pieceForCaptureId = pieceBoard[endX, endY].GetComponent<Piece>();
            pieceIdTaken = (int)(pieceForCaptureId.type) + 1;
            if (pieceForCaptureId.isTorok)
            {
                pieceIdTaken *= -1;
            }
        }

        if (tempEndPiece != null)
        {
            Piece oldPiece = tempEndPiece.GetComponent<Piece>();

            if (oldPiece.lastChance)
            {
                if (piece.value <= oldPiece.value)
                {
                    Destroy(tempPiece);
                    pieceBoard[startX, startY] = null;
                }
            }

            Destroy(tempEndPiece);
            pieceBoard[endX, endY] = null;
        }



        //stores move in a list so can be undone at any point
        moveList.Add(new Move(startX, startY, endX, endY, tempPiece, tempEndPiece, pieceIdTaken)); // moveList is a list of the moves done

        undoCounter++;

        pieceBoard[endX,endY] = tempPiece;//**why is it setting the end to the start
        pieceBoard[startX, startY] = null;

        Piece endPiece = pieceBoard[endX, endY].GetComponent<Piece>();//get piece script of object that moved

        endPiece.moved = true;// changes piece to say has moved
        endPiece.pieceX = endX;//alter x pos to new x pos for moved piece
        endPiece.pieceY = endY;//alter x pos to new y pos for moved piece
        //print("moved piece");

    }

    public void MovePieceVisual(int startX, int startY, int endX, int endY, GameObject piece)
    {
        StartCoroutine(VisualMovePiece(startX, startY,endX,endY, piece));
    }

    public void DisablePiece(GameObject piece)
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
        if (moveList.Count < 1)//guard clause added by jordan to handle error that occurs when undostorage is empty:: delete this when you see it if its fine
        {
            Debug.Log("List is empty, no undo occurred");
            return;
        }

        pieceBoard[moveList[undoCounter-1].startX,moveList[undoCounter-1].startY] = moveList[undoCounter-1].startObject;
        pieceBoard[moveList[undoCounter-1].endX,moveList[undoCounter-1].endY] = moveList[undoCounter-1].endObject;

        if (moveList[undoCounter-1].pieceTaken != 0)
        {
            if (moveList[undoCounter - 1].pieceTaken > 0)
            {
                PlacePiece(moveList[undoCounter-1].endX, moveList[undoCounter - 1].endY, moveList[undoCounter - 1].pieceTaken - 1);
            }
            else
            {
                PlacePiece(moveList[undoCounter - 1].endX, moveList[undoCounter - 1].endY, moveList[undoCounter - 1].pieceTaken + 1);
            }
        }

        moveList.RemoveAt(undoCounter-1);
        //Debug.Log("list legnth "+moveList.Count);

        undoCounter--;
    }

    public void UndoMoveVisual()//visually show undo moves
    {
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
        moveList[undoCounter-1].startObject.transform.position = hitBoxBoard[moveList[undoCounter-1].startX,moveList[undoCounter-1].startY].transform.position+Vector3.up;
        if(moveList[undoCounter-1].endObject)
        moveList[undoCounter-1].endObject.transform.position = hitBoxBoard[moveList[undoCounter-1].endX,moveList[undoCounter-1].endY].transform.position+Vector3.up;

        UndoMove();

    }
    //if click when movign it breaks
    //fix this later
    IEnumerator VisualMovePiece(int startX, int startY, int endX, int endY, GameObject piece)
    {
        //print("inside visualMove");

            while (Vector3.Distance(piece.transform.position, hitBoxBoard[endX, endY].transform.position + (Vector3.up * verticalPlaceOffset)) > 0.1f)
            {
                piece.transform.position = Vector3.MoveTowards(piece.transform.position, hitBoxBoard[endX, endY].transform.position + (Vector3.up * verticalPlaceOffset), pieceMoveSpeed * Time.deltaTime);
                yield return null;
            }
            piece.transform.position = hitBoxBoard[endX, endY].transform.position + (Vector3.up * verticalPlaceOffset);

        GameStateManager.EndTurn();//so that who ever's turn it is ends when the piece has finished moving

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
}

