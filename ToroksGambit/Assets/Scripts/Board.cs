using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField]
    private int boardSize = 8;//size of 2D array

    private GameObject[,] chessBoard;

    [SerializeField]
    private GameObject boardSquare;

    [SerializeField]
    private GameObject chessPiece;

    [SerializeField]
    private Camera camera;

    private GameObject storedPiece;

    // Start is called before the first frame update
    void Start()
    {
        chessBoard = new GameObject[boardSize,boardSize];

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
            }
            if(hit.transform.tag == "Chess Board" && storedPiece)
            {
                storedPiece.transform.position = hit.transform.position + new Vector3(0,0,0);

            }
        }
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
                chessBoard[i, j] = instance;
            }
        }

            GameObject pieceInstance = Instantiate(chessPiece,(transform.position + new Vector3(0,0,3)), Quaternion.identity);
            pieceInstance.gameObject.name = "chessPiece"; 
        
    }

    public void BoardState()
    {
        //track where each piece in the board

        //update board after each move

    }

    public void MovePiece(int pieceStartPos, int pieceEndPos, GameObject chessPiece)
    {
        //take original position

        //place gameobject at new position

        //update board state


    }

    public void UndoMove()
    {
        //sva eeach board state during each move

        //when undo is called, move back in the list of board states
    }

}
