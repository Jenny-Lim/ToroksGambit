using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
{

    // Start is called before the first frame update
    void Start()
    {
        type = "pawn";
        value = 1;
    }

    public override void UpdateMoves()
    {

        int dir;
        int clearResult;

        moves.Clear();

        if (isTorok)
        {
            dir = -1;
        }
        else
        {
            dir = 1;
        }


        if (InBoundsCheck(pieceX, pieceY + dir))
        {
            clearResult = ClearCheck(pieceX, pieceY + dir); // can just check if the spot is null instead

            if (clearResult == 0)
            {
                moves.Add(new Move(pieceX, pieceY, pieceX, pieceY + dir, pieceBoard[pieceX, pieceY], pieceBoard[pieceX, pieceY + dir]));
            }
        }

        Piece p = pieceBoard[pieceX + dir, pieceY + dir].GetComponent<Piece>();
        if (InBoundsCheck(pieceX + dir, pieceY + dir) && pieceBoard[pieceX + dir, pieceY + dir] != null && !p.isTorok && p.isTough) // if can capture another piece
        {
            moves.Add(new Move(pieceX, pieceY, pieceX + dir, pieceY + dir, pieceBoard[pieceX, pieceY], pieceBoard[pieceX + dir, pieceY + dir]));
        }

        if (!moved)
        {
            if (InBoundsCheck(pieceX, pieceY + dir * 2))
            {
                clearResult = ClearCheck(pieceX, pieceY + dir * 2);
                if (clearResult == 0 && pieceBoard[pieceX + dir, pieceY + dir] != null) // checks if the spot before is empty as well
                {
                    moves.Add(new Move(pieceX, pieceY, pieceX, pieceY + dir * 2, pieceBoard[pieceX, pieceY], pieceBoard[pieceX, pieceY + dir * 2]));
                }
            }
        }




        //if (isTorok)//can make this smaller if you use a variable as the +- 1 instead 
        //{
        //    if (InBoundsCheck(pieceX, pieceY - 1) && ClearCheck(pieceX, pieceY, pieceX, pieceY - 1))
        //    {
        //        moves.Add(new Move(pieceX, pieceY, pieceX, pieceY - 1, pieceBoard[pieceX, pieceY], pieceBoard[pieceX, pieceY - 1]));
        //    }

        //    if (InBoundsCheck(pieceX - 1, pieceY - 1) && pieceBoard[pieceX - 1, pieceY - 1] != null && !pieceBoard[pieceX - 1, pieceY - 1].GetComponent<Piece>().isTorok) // if can capture another piece
        //    {
        //        if (ClearCheck(pieceX, pieceY, pieceX - 1, pieceY - 1))
        //        {
        //            moves.Add(new Move(pieceX, pieceY, pieceX - 1, pieceY - 1, pieceBoard[pieceX, pieceY], pieceBoard[pieceX - 1, pieceY - 1]));
        //        }
        //    }
        //    if (!moved)
        //    {
        //        if (InBoundsCheck(pieceX, pieceY - 2) && ClearCheck(pieceX, pieceY, pieceX, pieceY - 2))
        //        {
        //            moves.Add(new Move(pieceX, pieceY, pieceX, pieceY - 2, pieceBoard[pieceX, pieceY], pieceBoard[pieceX, pieceY - 2]));
        //            // somewhere we have to say moved = true/false, likely in the board when the piece is being moved
        //        }
        //    }
        //}

        //else
        //{
        //    if (InBoundsCheck(pieceX, pieceY + 1) && ClearCheck(pieceX, pieceY, pieceX, pieceY + 1))
        //    {
        //        moves.Add(new Move(pieceX, pieceY, pieceX, pieceY + 1, pieceBoard[pieceX, pieceY], pieceBoard[pieceX, pieceY + 1]));
        //    }

        //    if (InBoundsCheck(pieceX + 1, pieceY + 1) && pieceBoard[pieceX + 1, pieceY + 1] != null && pieceBoard[pieceX + 1, pieceY + 1].GetComponent<Piece>().isTorok) // if can capture another piece
        //    {
        //        if (ClearCheck(pieceX, pieceY, pieceX + 1, pieceY + 1))
        //        {
        //            moves.Add(new Move(pieceX, pieceY, pieceX + 1, pieceY + 1, pieceBoard[pieceX, pieceY], pieceBoard[pieceX + 1, pieceY + 1]));
        //        }
        //    }
        //    if (!moved)
        //    {
        //        if (InBoundsCheck(pieceX, pieceY + 2) && ClearCheck(pieceX, pieceY, pieceX, pieceY + 2))
        //        {
        //            moves.Add(new Move(pieceX, pieceY, pieceX, pieceY + 2, pieceBoard[pieceX, pieceY], pieceBoard[pieceX, pieceY + 2]));
        //        }
        //    }
        //}
    }
}
