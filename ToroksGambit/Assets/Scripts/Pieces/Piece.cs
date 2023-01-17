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
    public bool isTorok; // does this mean we need team variants of the same piece

    //private GameObject board;
    //private Board b;

    public GameObject[,] pieceBoard;
    private int boardSize;

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
        boardSize = Board.GetSize();
    }

    public bool InBoundsCheck(int endX, int endY)
    {
        bool inBounds = true;
        if (endX > boardSize || endY > boardSize || endX < 0 || endY < 0)
        {
            inBounds = false;
        }
        return inBounds;
    }

    public bool ClearCheck(int pieceX, int pieceY, int endX, int endY)
    {
        bool isClear = true;

        // cont
        if (pieceY < endY) {

            for (int i = pieceY; i > endY; i++) {
 
                if (pieceBoard[pieceX, i]!=null)
                {

                    isClear = false;

                }
            }
        }


        if (pieceY > endY)
        {
            for (int i = endY; i > pieceY; i++)
            {

                if (pieceBoard[pieceX, i] != null)
                {

                    isClear = false;

                }
            }
        }


        if (pieceX < endX)
        {
            for (int i = pieceX; i > endX; i++)
            {

                if (pieceBoard[i, pieceY] != null)
                {

                    isClear = false;

                }
            }
        }


        if (pieceX > endX)
        {
            for (int i = endX; i > pieceX; i++)
            {

                if (pieceBoard[i, pieceY] != null)
                {

                    isClear = false;

                }
            }
        }


        return isClear;
    }

}
