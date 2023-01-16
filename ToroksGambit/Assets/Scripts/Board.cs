using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
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


    [SerializeField]
    private int boardSize = 8;//size of 2D array

    private GameObject[,] hitBoxBoard;//array for hitboxes for raycasting

    private GameObject[,] pieceBoard;//array for storing pieces and piece location

    [SerializeField]
    private GameObject boardSquare;

    [SerializeField]
    private GameObject chessPiece;

    private Camera camera;

    private GameObject storedPiece; 

    private List<UndoStorage> undoStorageList = new List<UndoStorage>();

    private int undoCounter = 0;

    private int pieceX;
    private int pieceY;

    void Start()
    {   camera = Camera.main;
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
            if(hit.transform.tag == "Chess Piece")//if mouse is clicked on chess piece
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

                MovePiece(pieceX, pieceY, clickedX, clickedY);
                storedPiece.transform.position = hit.transform.position + new Vector3(0,0,0);

            }
        }
        }
        if (Input.GetMouseButtonDown(1))//right click mouse to undo moves
        {
            UndoMoveVisual();
        }


    }

    private void BuildBoard()//generates hitbox for chess board
    {
        for(int i=0;i<boardSize;i++)
        {
            for(int j=0;j<boardSize;j++)
            {
                GameObject instance = Instantiate(boardSquare,(transform.position + new Vector3(i,0,j)), Quaternion.identity, gameObject.transform);
                instance.gameObject.name = i + "_" + j;
                hitBoxBoard[i, j] = instance;
            }

            GameObject pieceInstance = Instantiate(chessPiece,(transform.position + new Vector3(i,0,0)), Quaternion.identity, gameObject.transform);
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
    public void MovePiece(int startX, int startY, int endX, int endY)//take 2 positions to move a piece
    {
        GameObject tempPiece = pieceBoard[startX, startY];
        GameObject tempEndPiece = pieceBoard[endX, endY];
        //stores move in a list so can be undone at any point
        undoStorageList.Add(new UndoStorage(startX, startY, endX, endY, tempPiece, tempEndPiece));
        undoCounter++;

        pieceBoard[endX,endY] = tempPiece;
        pieceBoard[startX, startY] = tempEndPiece;

    }

    //undoes most recent move
    //repeatedly calling will undo moves until beggining
    public void UndoMove()
    { 


        pieceBoard[undoStorageList[undoCounter-1].startX,undoStorageList[undoCounter-1].startY] = undoStorageList[undoCounter-1].startObject;
        pieceBoard[undoStorageList[undoCounter-1].endX,undoStorageList[undoCounter-1].endY] = undoStorageList[undoCounter-1].endObject;

        undoStorageList.RemoveAt(undoCounter-1);
        Debug.Log("list legnth "+undoStorageList.Count);

        undoCounter--;
    }

    public void UndoMoveVisual()//visually show undo moves
    {
        undoStorageList[undoCounter-1].startObject.transform.position = hitBoxBoard[undoStorageList[undoCounter-1].startX,undoStorageList[undoCounter-1].startY].transform.position;
        if(undoStorageList[undoCounter-1].endObject)
        undoStorageList[undoCounter-1].endObject.transform.position = hitBoxBoard[undoStorageList[undoCounter-1].endX,undoStorageList[undoCounter-1].endY].transform.position;

        UndoMove();

    }

}
