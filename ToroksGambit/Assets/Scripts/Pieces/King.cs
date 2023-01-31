using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class King : Piece
{
    // Start is called before the first frame update
    void Start()
    {
        type = "king";
        value = 1000;
    }

    public override void UpdateMoves()
    {
        int clearResult;
        moves.Clear();


        if (InBoundsCheck(pieceX + 1, pieceY + 1))
        {
            clearResult = ClearCheck(pieceX + 1, pieceY + 1);
            if (clearResult == 0 || clearResult == 3)
            {
                moves.Add(new Move(pieceX, pieceY, pieceX + 1, pieceY + 1, pieceBoard[pieceX, pieceY], pieceBoard[pieceX + 1, pieceY + 1]));
            }
        }

        if (InBoundsCheck(pieceX - 1, pieceY - 1))
        {
            clearResult = ClearCheck(pieceX - 1, pieceY - 1);
            if (clearResult == 0 || clearResult == 3)
            {
                moves.Add(new Move(pieceX, pieceY, pieceX - 1, pieceY - 1, pieceBoard[pieceX, pieceY], pieceBoard[pieceX - 1, pieceY - 1]));
            }
        }

        if (InBoundsCheck(pieceX + 1, pieceY))
        {
            clearResult = ClearCheck(pieceX + 1, pieceY);
            if (clearResult == 0 || clearResult == 3)
            {
                moves.Add(new Move(pieceX, pieceY, pieceX + 1, pieceY, pieceBoard[pieceX, pieceY], pieceBoard[pieceX + 1, pieceY]));
            }
        }

        if (InBoundsCheck(pieceX, pieceY + 1))
        {
            clearResult = ClearCheck(pieceX, pieceY + 1);
            if (clearResult == 0 || clearResult == 3)
            {
                moves.Add(new Move(pieceX, pieceY, pieceX, pieceY + 1, pieceBoard[pieceX, pieceY], pieceBoard[pieceX, pieceY + 1]));
            }
        }

        if (InBoundsCheck(pieceX - 1, pieceY))
        {
            clearResult = ClearCheck(pieceX - 1, pieceY);
            if (clearResult == 0 || clearResult == 3)
            {
                moves.Add(new Move(pieceX, pieceY, pieceX - 1, pieceY, pieceBoard[pieceX, pieceY], pieceBoard[pieceX - 1, pieceY]));
            }
        }

        if (InBoundsCheck(pieceX, pieceY - 1))
        {
            clearResult = ClearCheck(pieceX, pieceY - 1);
            if (clearResult == 0 || clearResult == 3)
            {
                moves.Add(new Move(pieceX, pieceY, pieceX, pieceY - 1, pieceBoard[pieceX, pieceY], pieceBoard[pieceX, pieceY - 1]));
            }
        }





        //if (InBoundsCheck(pieceX + 1, pieceY + 1) && ClearCheck(pieceX, pieceY, pieceX + 1, pieceY + 1))
        //{
        //    moves.Add(new Move(pieceX, pieceY, pieceX + 1, pieceY + 1, pieceBoard[pieceX, pieceY], pieceBoard[pieceX + 1, pieceY + 1]));
        //}

        //if (InBoundsCheck(pieceX - 1, pieceY - 1) && ClearCheck(pieceX, pieceY, pieceX - 1, pieceY - 1))
        //{
        //    moves.Add(new Move(pieceX, pieceY, pieceX - 1, pieceY - 1, pieceBoard[pieceX, pieceY], pieceBoard[pieceX - 1, pieceY - 1]));
        //}

        //if (InBoundsCheck(pieceX + 1, pieceY) && ClearCheck(pieceX, pieceY, pieceX + 1, pieceY))
        //{
        //    moves.Add(new Move(pieceX, pieceY, pieceX + 1, pieceY, pieceBoard[pieceX, pieceY], pieceBoard[pieceX + 1, pieceY]));
        //}

        //if (InBoundsCheck(pieceX, pieceY + 1) && ClearCheck(pieceX, pieceY, pieceX, pieceY + 1))
        //{
        //    moves.Add(new Move(pieceX, pieceY, pieceX, pieceY + 1, pieceBoard[pieceX, pieceY], pieceBoard[pieceX, pieceY + 1]));
        //}

        //if (InBoundsCheck(pieceX - 1, pieceY) && ClearCheck(pieceX, pieceY, pieceX - 1, pieceY))
        //{
        //    moves.Add(new Move(pieceX, pieceY, pieceX - 1, pieceY, pieceBoard[pieceX, pieceY], pieceBoard[pieceX - 1, pieceY]));
        //}

        //if (InBoundsCheck(pieceX, pieceY - 1) && ClearCheck(pieceX, pieceY, pieceX, pieceY - 1))
        //{
        //    moves.Add(new Move(pieceX, pieceY, pieceX, pieceY - 1, pieceBoard[pieceX, pieceY], pieceBoard[pieceX, pieceY - 1]));
        //}
    }
}
