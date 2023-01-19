using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    /*
    public class UndoStorage
    {
        public int startX;
        public int startY;

        public int endX;
        public int endY;

        public GameObject startObject;
        public GameObject endObject;

        public UndoStorage(int x1, int y1, int x2, int y2, GameObject object1, GameObject object2)
        {
            startX = x1;
            startY = y1;
            endX = x2;
            endY = y2;
            startObject = object1;
            endObject = object2;
        }

    }
    */

    //take out undostorage - make own class
    //make it possible for board to create a list if possible moves
    //change undostorage to move
    //change undostorage to list of possible moves
    //clean board class to work with undo being own class
    //fix busg with moving pieces

    //click on piece, board stores selected piece
    //click location to move piece
    //make function "movebiecevalidotor"
    //function checks if piece CAN move to spot
    //checks location and cross references with possible moves and check sif location is in list
    //board function to give Piece its own location

    [SerializeField]
    private static int boardSize = 8;//size of 2D array

    private GameObject[,] hitBoxBoard;//array for hitboxes for raycasting

    private static GameObject[,] pieceBoard;//array for storing pieces and piece location -- made static (jenny)

    [SerializeField]
    private GameObject boardSquare;

    [SerializeField]
    private GameObject chessPiece;

    private Camera camera;

    private GameObject storedPiece; 

    private List<Move> moveList = new List<Move>();

    private int undoCounter = 0;

    private Vector3 boardPosition;

    [SerializeField] private float pieceMoveSpeed = 35f;//made by jordan, can change piece move speed easier

    // made static -- jenny
    private static int pieceX;
    private static int pieceY;

    void Start()
    {
        boardPosition = transform.position;
        camera = Camera.main;
        hitBoxBoard = new GameObject[boardSize,boardSize];
        pieceBoard = new GameObject[boardSize, boardSize];

        BuildBoard();

        
    }

    void Update()
    {
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);//shoot ray using mouse from camera

        if (Input.GetMouseButtonDown(0))//left click mouse to move pieces
        {

        if (Physics.Raycast(ray, out hit))
        {
                if (hit.transform.tag == "Chess Piece")//if mouse is clicked on chess piece
                {
                storedPiece = hit.transform.gameObject;//store piece

                for(int i=0;i<boardSize;i++)
                {
                    for(int j=0;j<boardSize;j++)
                    {
                        if(hit.transform.gameObject == pieceBoard[i,j])//get position of piece in array
                        {
                            pieceX = i;//store locations
                            pieceY = j;
                        }
                    }
                }
                    //signifier that piece is chosen
                    //storedPiece.transform.position = Vector3.MoveTowards(hit.transform.position, hit.transform.position+new Vector3(0,5,0), 10f * Time.deltaTime);

            }
            if(hit.transform.tag == "Chess Board" && storedPiece)//if a piece is stored and another spot is chosen
            {
                int clickedX = 0;//position for second click
                int clickedY = 0;

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

                    //pieceBoard[clickedX, clickedY] = null;

                MovePiece(pieceX, pieceY, clickedX, clickedY);
                StartCoroutine(VisualMovePiece(pieceX, pieceY, clickedX, clickedY, storedPiece));
                //storedPiece.transform.position = hit.transform.position + new Vector3(0,0,0);

                storedPiece = null;

            }
        }
        else 
        {
                //storedPiece.transform.position = Vector3.MoveTowards(hit.transform.position, hit.transform.position - new Vector3(0, 5, 0), 10f * Time.deltaTime);
                storedPiece = null; 
        }
        }
        if (Input.GetMouseButtonDown(1))//right click mouse to undo moves
        {
            UndoMoveVisual();
        }


    }

    private void BuildBoard()//generates hitbox for chess board
    {
        float boardOffset = ((float)boardSize)/2;
        for(int i=0;i<boardSize;i++)
        {
            for(int j=0;j<boardSize;j++)
            {
                GameObject instance = Instantiate(boardSquare,(boardPosition + new Vector3(i - boardOffset, 0,j - boardOffset)), Quaternion.identity, gameObject.transform);
                instance.gameObject.name = i + "_" + j;
                hitBoxBoard[i, j] = instance;
            }

            GameObject pieceInstance = Instantiate(chessPiece,(boardPosition + new Vector3(i - boardOffset,0,-boardOffset)), Quaternion.identity, gameObject.transform);
            pieceInstance.gameObject.name = "chessPiece"; 
            pieceBoard[i,0] = pieceInstance; 

        }
        
    }

    public void RefreshVisualBoard()//not used in current layout
    {
        GameObject[] pieces;
        pieces = GameObject.FindGameObjectsWithTag("Chess Piece");

        foreach (GameObject piece in pieces)
        {
            Destroy(piece);
        }

        for(int i = 0;i<boardSize;i++)
        {
            for(int j = 0;j<boardSize;j++)
            {
                if(pieceBoard[i,j])
                {

                    GameObject pieceInstance = Instantiate(pieceBoard[i,j], hitBoxBoard[i,j].transform.position, Quaternion.identity, gameObject.transform);
                    pieceInstance.gameObject.name = "chessPiece";
                    pieceBoard[i,j] = pieceInstance;
                }
            }
        }

    }

    //input the X and Y of the piece being moved(startX and Y) and the X and Y of the spot being moved to(end X Y)
    //if click impossible move then clear storedmove
    //if click off bord then clear stored item
    public void MovePiece(int startX, int startY, int endX, int endY)//take 2 positions to move a piece
    {
        GameObject tempPiece = pieceBoard[startX, startY];
        GameObject tempEndPiece = pieceBoard[endX, endY];
        //stores move in a list so can be undone at any point
        moveList.Add(new Move(startX, startY, endX, endY, tempPiece, tempEndPiece));
        undoCounter++;

        pieceBoard[endX,endY] = tempPiece;
        pieceBoard[startX, startY] = tempEndPiece;

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

        moveList[undoCounter-1].startObject.transform.position = hitBoxBoard[moveList[undoCounter-1].startX,moveList[undoCounter-1].startY].transform.position;
        if(moveList[undoCounter-1].endObject)
        moveList[undoCounter-1].endObject.transform.position = hitBoxBoard[moveList[undoCounter-1].endX,moveList[undoCounter-1].endY].transform.position;

        UndoMove();

    }
    //if click when movign it breaks
    //fix this later
    IEnumerator VisualMovePiece(int startX, int startY, int endX, int endY, GameObject piece)
    {
        while (piece.transform.position != hitBoxBoard[endX, endY].transform.position)
        {
            piece.transform.position = Vector3.MoveTowards(piece.transform.position, hitBoxBoard[endX, endY].transform.position, pieceMoveSpeed * Time.deltaTime);
            yield return new WaitForSeconds(0.01f);
        }
    }


    // added by jenny to retrieve piece positions
    public static int GetX()
    {
        return pieceX;
    }

    public static int GetY()
    {
        return pieceY;
    }

    // added by jenny to retrieve pieceBoard
    public static GameObject[,] GetPieceBoard()
    {
        return pieceBoard;
    }

    public static int GetSize()
    {
        return boardSize;
    }
}

