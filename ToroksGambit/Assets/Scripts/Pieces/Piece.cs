using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{

    //probably want an is same piece function

    public bool moved = false;
    // list of possible moves
    public List<Move> moves;

    public bool isTaken;
    public int pieceX;
    public int pieceY;
    public string type;

    public bool isTorok;

    public bool isTough;
    public bool lastChance;
    public float value;
    //promote - not starting


    public GameObject[,] pieceBoard;

    void Awake()
    {
        //isTaken = false;
        pieceBoard = Board.GetPieceBoard();
        moves = new List<Move>();
    }

    public virtual void UpdateMoves()
    {
        // empty to be overwritten -- maybe it can hold default functionality (see pats movePiece)
        //moves.Clear();

        //int endX = Board.GetClickedX();
        //int endY = Board.GetClickedY();

        //checks would go here
        //moves.Add(new Move(pieceX, pieceY, endX, endY, pieceBoard[pieceX, pieceY], pieceBoard[endX, endY]));
    }

    public bool InBoundsCheck(int endX, int endY)
    {
        if (endX >= Board.boardSize || endY >= Board.boardSize || endX < 0 || endY < 0)
        {
            return false;
        }
        return true;
    }

    public virtual int ClearCheck(int endX, int endY) // for new vers, only need the end params and it should return int
    {
        if (pieceBoard[endX, endY] != null)
        {
            Piece p = pieceBoard[endX, endY].GetComponent<Piece>();

            if (p.type == "wall" || p.isTorok == this.isTorok) // if its your own piece / is a wall, can't capture
            {
                return 1;
            }
            else if (p.type == "hole")
            {
                return 2;
            }
            else if (p.isTorok != this.isTorok)//is an enemy -- move there and sop moving
            {
                return 3;
            }

        }

        return 0; // keep going + add move



        //bool isClear = true;
        //Piece thePiece = pieceBoard[pieceX, pieceY].GetComponent<Piece>();

        ////Patrick - check for tough pieces
        //if (type == "pawn" && thePiece.isTough)
        //{
        //    Debug.Log("TOUGHPROTECT");
        //    isClear = false;
        //    return isClear;
        //}

        //// handling this in here for now, how expensive is getcomponent
        //if (pieceBoard[endX, endY] != null)
        //{
        //    Piece p = pieceBoard[endX, endY].GetComponent<Piece>();

        //    if (type == "pawn" && p.isTough)
        //    {
        //        Debug.Log("TOUGHPROTECT");
        //        isClear = false;
        //        return isClear;
        //    }


        //    if (p.type == "wall" || p.type == "hole" || p.isTorok == this.isTorok) // if its your own piece, can't capture
        //    {
        //        isClear = false;
        //        return isClear;
        //    }

        //}

        //if (thePiece.type == "knight") // knights can jump over pieces
        //{
        //    //isClear = true;
        //    return isClear;
        //}
        //// below is if the piece isnt a knight


        //// horizontally
        //int start = pieceX; // to start
        //int end = endX;

        //if (pieceX < endX)
        //{
        //    start = pieceX;
        //    end = endX;
        //}
        //if (pieceX > endX)
        //{
        //    start = endX;
        //    end = pieceX;
        //}

        //for (int i = start; i > end; i++)
        //{
        //    if (pieceBoard[i, pieceY] != null)
        //    {

        //        if (pieceBoard[i, pieceY].GetComponent<Piece>().type != "hole")
        //        {
        //            isClear = false;
        //            return isClear;
        //        }

        //    }
        //}


        //// vertically
        //int startY = pieceY; // to start
        //int enddY = endY;

        //if (pieceY < enddY)
        //{
        //    startY = pieceY;
        //    enddY = endY;
        //}
        //if (pieceY > enddY)
        //{
        //    startY = endY;
        //    enddY = pieceY;
        //}

        //for (int i = startY; i > enddY; i++)
        //{
        //    if (pieceBoard[pieceX, i] != null)
        //    {

        //        if (pieceBoard[pieceX, i].GetComponent<Piece>().type != "hole")
        //        {
        //            isClear = false;
        //            return isClear;
        //        }

        //    }
        //}


        ////diagonally

        //for (int i = start; i > end; i++)
        //{
        //    for (int j = startY; j > enddY; j++)
        //    {

        //        // along movement path diagonal
        //        if (pieceBoard[i, j] != null)
        //        {
        //            if (pieceBoard[i, j].GetComponent<Piece>().type != "hole")
        //            {
        //                isClear = false;
        //                return isClear;
        //            }
        //        }

        //    }
        //}


        //return isClear;
    } // ClearCheck


    public void MovesAdd(int directionX, int directionY)
    {
        for (int i = 0; i < Board.boardSize; i++)
        {
            for (int j = 0; j < Board.boardSize; j++)
            {
                if (InBoundsCheck(pieceX + (i * directionX), pieceY + (j * directionY)))
                {
                    int clearResult = ClearCheck(pieceX + (i * directionX), pieceY + (j * directionY));

                    if (clearResult == 0) // if spot is empty
                    {
                        moves.Add(new Move(pieceX, pieceY, pieceX + (i * directionX), pieceY + (j * directionY), pieceBoard[pieceX, pieceY], pieceBoard[pieceX + (i * directionX), pieceY + (j * directionY)]));
                    }
                    else if (clearResult == 1) // if spot is wall / same color
                    {
                        return;
                    }

                    else if (clearResult == 3) // if spot is capturable
                    {
                        moves.Add(new Move(pieceX, pieceY, pieceX + (i * directionX), pieceY + (j * directionY), pieceBoard[pieceX, pieceY], pieceBoard[pieceX + (i * directionX), pieceY + (j * directionY)]));
                        return;
                    }

                    else if (clearResult == 2) // if spot is a hole
                    {
                        continue;
                    }
                }
            }
        }
    } // MovesAdd

}
