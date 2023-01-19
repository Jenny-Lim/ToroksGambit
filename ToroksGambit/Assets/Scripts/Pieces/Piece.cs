using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{

    // list of possible moves--validation is going through the list and seeing if the attempted is in the list. moveList in board is the actual move that is going to be performed
    public List<Move> moves;

    public bool isTaken;
    public int pieceX;
    public int pieceY;
    public string type;
    public bool isTorok;

    //private GameObject board;
    //private Board b;

    public GameObject[,] pieceBoard;
    public int boardSize;

    void Awake()
    {
        isTaken = false;
        //board = GameObject.FindWithTag("Chess Board");
        //b = board.GetComponent<Board>();
        pieceBoard = Board.GetPieceBoard();
        pieceX = Board.GetX();
        pieceY = Board.GetY();
        boardSize = Board.GetSize();
        moves = new List<Move>();
    }

    public virtual void UpdateMoves()
    {
        // empty to be overwritten -- maybe it can hold default functionality (see pats movePiece)
        //moves.Clear();
        //int endX = Board.GetClickedX();
        //int endY = Board.GetClickedY();

        //moves.Add(new Move(pieceX, pieceY, endX, endY, pieceBoard[pieceX, pieceY], pieceBoard[endX, endY]));
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

    public bool ClearCheck(int pieceX, int pieceY, int endX, int endY) // not called on yet so it won't fuck with other things if its wack
    {
        bool isClear = true;

        // horizontally
        int start = pieceX; // to start
        int end = endX;

        if (pieceX < endX)
        {
            start = pieceX;
            end = endX;
        }
        if (pieceX > endX)
        {
            start = endX;
            end = pieceX;
        }

            for (int i = start; i > end; i++)
            {
                if (pieceBoard[i, pieceY] != null)
                {

                    if (pieceBoard[i, pieceY].GetComponent<Piece>().type != "hole")
                    {
                        isClear = false;
                    }

                }
            }
            

        // vertically
        int startY = pieceY; // to start
        int enddY = endY;

        if (pieceY < enddY)
        {
            startY = pieceY;
            enddY = endY;
        }
        if (pieceY > enddY)
        {
            startY = endY;
            enddY = pieceY;
        }

        for (int i = startY; i > enddY; i++)
        {
            if (pieceBoard[pieceX, i] != null)
            {

                if (pieceBoard[pieceX, i].GetComponent<Piece>().type != "hole")
                {
                    isClear = false;
                }

            }
        }


        return isClear;
    }

    void Update()
    {
       if (isTaken)
        {
            this.gameObject.SetActive(false);
        } 
    }

}
