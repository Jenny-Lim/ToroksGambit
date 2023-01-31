using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Piece
{
    // Start is called before the first frame update
    void Start()
    {
        type = PieceType.bishop;
        value = 3f;
    }

    public override void UpdateMoves()
    {
        moves.Clear();

        MovesAdd(1, 1);
        //MovesAdd(-1, -1);
        //MovesAdd(1, -1);
        //MovesAdd(-1, 1);

        //for (int i = 0; i < Board.boardSize; i++)
        //{
        //    if (InBoundsCheck(pieceX + i, pieceY + i) && ClearCheck(pieceX, pieceY, pieceX + i, pieceY + i))
        //    {
        //        moves.Add(new Move(pieceX, pieceY, pieceX + i, pieceY + i, pieceBoard[pieceX, pieceY], pieceBoard[pieceX + i, pieceY + i]));
        //    }
        //    if (InBoundsCheck(pieceX - i, pieceY - i) && ClearCheck(pieceX, pieceY, pieceX - i, pieceY - i))
        //    {
        //        moves.Add(new Move(pieceX, pieceY, pieceX - i, pieceY - i, pieceBoard[pieceX, pieceY], pieceBoard[pieceX - i, pieceY - i]));
        //    }
        //    if (InBoundsCheck(pieceX + i, pieceY - i) && ClearCheck(pieceX, pieceY, pieceX + i, pieceY - i))
        //    {
        //        moves.Add(new Move(pieceX, pieceY, pieceX + i, pieceY - i, pieceBoard[pieceX, pieceY], pieceBoard[pieceX + i, pieceY - i]));
        //    }
        //    if (InBoundsCheck(pieceX - i, pieceY + i) && ClearCheck(pieceX, pieceY, pieceX - i, pieceY + i))
        //    {
        //        moves.Add(new Move(pieceX, pieceY, pieceX - i, pieceY + i, pieceBoard[pieceX, pieceY], pieceBoard[pieceX - i, pieceY + i]));
        //    }
        //}
    }

    
    //public override bool ClearCheck(int pieceX, int pieceY, int endX, int endY) // bishop only moves diagonally -- WIP
    //{
    //    bool isClear = true;
    //    Piece thePiece = pieceBoard[pieceX, pieceY].GetComponent<Piece>();

    //    if (pieceBoard[endX, endY] != null)
    //    {
    //        Piece p = pieceBoard[endX, endY].GetComponent<Piece>();

    //        if (p.type == "wall" || p.type == "hole" || p.isTorok == this.isTorok) // if its your own piece, can't capture
    //        {
    //            isClear = false;
    //            return isClear;
    //        }

    //    }

    //    int start = pieceX; // to start
    //    int end = endX;

    //    if (pieceX < endX)
    //    {
    //        start = pieceX;
    //        end = endX;
    //    }
    //    if (pieceX > endX)
    //    {
    //        start = endX;
    //        end = pieceX;
    //    }

    //    int startY = pieceY; // to start
    //    int enddY = endY;

    //    if (pieceY < enddY)
    //    {
    //        startY = pieceY;
    //        enddY = endY;
    //    }
    //    if (pieceY > enddY)
    //    {
    //        startY = endY;
    //        enddY = pieceY;
    //    }


    //    //diagonally -- along the diagonal movement, blockers would be along that movement, and +1+1, -1-1, +1-1, -1+1 -- WIP

    //    for (int i = start; i > end; i++)
    //    {
    //        for (int j = startY; j > enddY; j++)
    //        {

    //            // along movement path diagonal
    //            if (pieceBoard[i, j] != null)
    //            {
    //                if (pieceBoard[i, j].GetComponent<Piece>().type != "hole")
    //                {
    //                    isClear = false;
    //                    return isClear;
    //                }
    //            }


            //    if (endX > pieceX && endY > pieceY)
            //    {
            //        if (pieceBoard[i + 1, j + 1] != null)
            //        {
            //            if (pieceBoard[i + 1, j + 1].GetComponent<Piece>().type != "hole")
            //            {
            //                isClear = false;
            //                return isClear;
            //            }
            //        }
            //    }

            //    if (endX > pieceX && endY < pieceY)
            //    {
            //        if (pieceBoard[i + 1, j - 1] != null)
            //        {
            //            if (pieceBoard[i + 1, j - 1].GetComponent<Piece>().type != "hole")
            //            {
            //                isClear = false;
            //                return isClear;
            //            }
            //        }
            //    }

            //    if (endX < pieceX && endY > pieceY)
            //    {
            //        if (pieceBoard[i - 1, j + 1] != null)
            //        {
            //            if (pieceBoard[i - 1, j + 1].GetComponent<Piece>().type != "hole")
            //            {
            //                isClear = false;
            //                return isClear;
            //            }
            //        }
            //    }

            //    if (endX < pieceX && endY < pieceY)
            //    {
            //        if (pieceBoard[i - 1, j - 1] != null)
            //        {
            //            if (pieceBoard[i - 1, j - 1].GetComponent<Piece>().type != "hole")
            //            {
            //                isClear = false;
            //                return isClear;
            //            }
            //        }
            //    }

            //}
        //}

        //return isClear;
    //}

}
