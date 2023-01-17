using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{

    public List<Move> moves;

    public bool isTaken;
    public int pieceX;
    public int pieceY;
    public string type;
    //public bool isTorok; // does this mean we need team variants of the same piece

    //private GameObject board;
    //private Board b;

    public GameObject[,] pieceBoard;

    void Awake()
    {
        isTaken = false;
        //board = GameObject.FindWithTag("Chess Board");
        //b = board.GetComponent<Board>();
        pieceBoard = Board.GetPieceBoard();
        moves = new List<Move>();

        //pieceX = getX();
        pieceX = Board.GetX();
        //pieceY = getY();
        pieceY = Board.GetY();
    }

}
