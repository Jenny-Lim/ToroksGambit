using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Piece
{
    // Start is called before the first frame update
    void Start()
    {
        type = "rook";
    }

    public override void UpdateMoves()
    {
        moves.Clear();

        for (int i = 0; i < Board.boardSize; i++)
        {

            if (InBoundsCheck(pieceX + i, pieceY) && ClearCheck(pieceX, pieceY, pieceX + i, pieceY))
            {
                moves.Add(new Move(pieceX, pieceY, pieceX + i, pieceY, pieceBoard[pieceX, pieceY], pieceBoard[pieceX + i, pieceY]));
            }

            if (InBoundsCheck(pieceX, pieceY + i) && ClearCheck(pieceX, pieceY, pieceX, pieceY + i))
            {
                moves.Add(new Move(pieceX, pieceY, pieceX, pieceY + i, pieceBoard[pieceX, pieceY], pieceBoard[pieceX, pieceY + i]));
            }

            if (InBoundsCheck(pieceX - i, pieceY) && ClearCheck(pieceX, pieceY, pieceX-i, pieceY))
            {
                moves.Add(new Move(pieceX, pieceY, pieceX - i, pieceY, pieceBoard[pieceX, pieceY], pieceBoard[pieceX - i, pieceY]));
            }

            if (InBoundsCheck(pieceX, pieceY - i) && ClearCheck(pieceX, pieceY, pieceX, pieceY-i))
            {
                moves.Add(new Move(pieceX, pieceY, pieceX, pieceY - i, pieceBoard[pieceX, pieceY], pieceBoard[pieceX, pieceY - i]));
            }
        }
    }


    public override bool ClearCheck(int pieceX, int pieceY, int endX, int endY) // rook only moves updownleftright -- WIP, **why for loops in here? - jordan
    {
        bool isClear = true;
        Piece thePiece = pieceBoard[pieceX, pieceY].GetComponent<Piece>();

        if (pieceBoard[endX, endY] != null)
        {
            Piece p = pieceBoard[endX, endY].GetComponent<Piece>();

            if (p.type == "wall" || p.type == "hole" || p.isTorok == this.isTorok) // if its your own piece, can't capture
            {
                isClear = false;
                return isClear;
            }

        }

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
                    return isClear;
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
                    return isClear;
                }

            }
        }

        return isClear;
    }

}
