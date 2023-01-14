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

        public void UpdateData(int x1, int y1, int x2, int y2, GameObject object1, GameObject object2)
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

    private GameObject[,] hitBoxBoard;

    private GameObject[,] pieceBoard;

    [SerializeField]
    private GameObject boardSquare;

    [SerializeField]
    private GameObject chessPiece;

    [SerializeField]
    private Camera camera;

    private GameObject storedPiece; 

    private static UndoStorage[] undoStorage;

    private int undoCounter = 0;

    private int pieceX;
    private int pieceY;

    // Start is called before the first frame update
    void Start()
    {
        hitBoxBoard = new GameObject[boardSize,boardSize];
        pieceBoard = new GameObject[boardSize, boardSize];
        undoStorage = new UndoStorage[20];
        for(int i=0;i<20;i++)
        {
            undoStorage[i] = new UndoStorage(0,0,0,0,null,null);
        }

        Debug.Log(transform.position);

        BuildBoard();

        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {

        if (Physics.Raycast(ray, out hit)) 
        {
            Debug.Log(hit.transform.gameObject.name);
            if(hit.transform.tag == "Chess Piece")
            {
                storedPiece = hit.transform.gameObject;

                for(int i=0;i<boardSize;i++)
                {
                    for(int j=0;j<boardSize;j++)
                    {
                        if(hit.transform.gameObject == pieceBoard[i,j])
                        {
                            pieceX = i;
                            Debug.Log(pieceX);
                            pieceY = j;
                            Debug.Log(pieceY);
                        }
                    }
                }

            }
            if(hit.transform.tag == "Chess Board" && storedPiece)
            {
                int clickedX = 0;
                int clickedY = 0;

                for(int i=0;i<boardSize;i++)
                {
                    for(int j=0;j<boardSize;j++)
                    {
                        if(hit.transform.gameObject == hitBoxBoard[i,j])
                        {
                            clickedX = i;
                            Debug.Log(clickedX);
                            clickedY = j;
                            Debug.Log(clickedY);
                        }
                    }
                }

                MovePiece(pieceX, pieceY, clickedX, clickedY);
                storedPiece.transform.position = hit.transform.position + new Vector3(0,0,0);

            }
        }
        }
        if (Input.GetMouseButtonDown(1))
        {
            UndoMove();
            RefreshVisualBoard();
        }



    }

    private void BuildBoard()
    {
        for(int i=0;i<boardSize;i++)
        {
            for(int j=0;j<boardSize;j++)
            {
                GameObject instance = Instantiate(boardSquare,(transform.position + new Vector3(i,0,j)), Quaternion.identity);
                instance.gameObject.name = i + "_" + j;
                hitBoxBoard[i, j] = instance;
            }

            GameObject pieceInstance = Instantiate(chessPiece,(transform.position + new Vector3(i,0,0)), Quaternion.identity);
            pieceInstance.gameObject.name = "chessPiece"; 
            pieceBoard[i,0] = pieceInstance; 

        }
        
    }

    public void RefreshVisualBoard()
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

                    GameObject pieceInstance = Instantiate(pieceBoard[i,j], hitBoxBoard[i,j].transform.position, Quaternion.identity);
                    pieceInstance.gameObject.name = "chessPiece";
                    pieceBoard[i,j] = pieceInstance;
                }
            }
        }

    }


    public void MovePiece(int startX, int startY, int endX, int endY)
    {
        GameObject tempPiece = pieceBoard[startX, startY];
        GameObject tempEndPiece = pieceBoard[endX, endY];
        undoCounter++;
        undoStorage[undoCounter].UpdateData(startX, startY, endX, endY, tempPiece, tempEndPiece);

        pieceBoard[endX,endY] = tempPiece;
        pieceBoard[startX, startY] = tempEndPiece;
        //take original position

        //place gameobject at new position

        //update board state


    }

    public void UndoMove()
    { 
        pieceBoard[undoStorage[undoCounter].startX,undoStorage[undoCounter].startY] = undoStorage[undoCounter].startObject;
        pieceBoard[undoStorage[undoCounter].endX,undoStorage[undoCounter].endY] = undoStorage[undoCounter].endObject;

        undoCounter--;
    }

}

