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
        moves.Clear();

        if (isTorok)//can make this smaller if you use a variable as the +- 1 instead 
        {
            if (InBoundsCheck(pieceX, pieceY - 1) && ClearCheck(pieceX, pieceY, pieceX, pieceY-1))
            {
                moves.Add(new Move(pieceX, pieceY, pieceX, pieceY - 1, pieceBoard[pieceX, pieceY], pieceBoard[pieceX, pieceY - 1]));
            }

            if (InBoundsCheck(pieceX - 1, pieceY - 1) && pieceBoard[pieceX - 1, pieceY - 1]!=null && !pieceBoard[pieceX-1, pieceY-1].GetComponent<Piece>().isTorok) // if can capture another piece
            {
                if (ClearCheck(pieceX, pieceY, pieceX - 1, pieceY - 1))
                {
                    moves.Add(new Move(pieceX, pieceY, pieceX - 1, pieceY - 1, pieceBoard[pieceX, pieceY], pieceBoard[pieceX - 1, pieceY - 1]));
                }
            }
            if (!moved)
            {
                if (InBoundsCheck(pieceX, pieceY - 2) && ClearCheck(pieceX, pieceY, pieceX, pieceY - 2))
                {
                    moves.Add(new Move(pieceX, pieceY, pieceX, pieceY - 2, pieceBoard[pieceX, pieceY], pieceBoard[pieceX, pieceY - 2]));
                    // somewhere we have to say moved = true/false, likely in the board when the piece is being moved
                }
            }
        }

        else {
            if (InBoundsCheck(pieceX, pieceY + 1) && ClearCheck(pieceX, pieceY, pieceX, pieceY + 1))
            {
                moves.Add(new Move(pieceX, pieceY, pieceX, pieceY + 1, pieceBoard[pieceX, pieceY], pieceBoard[pieceX, pieceY + 1]));
            }

            if (InBoundsCheck(pieceX + 1, pieceY + 1) && pieceBoard[pieceX + 1, pieceY + 1] != null && pieceBoard[pieceX + 1, pieceY + 1].GetComponent<Piece>().isTorok) // if can capture another piece
            {
                if (ClearCheck(pieceX, pieceY, pieceX + 1, pieceY + 1))
                {
                    moves.Add(new Move(pieceX, pieceY, pieceX + 1, pieceY + 1, pieceBoard[pieceX, pieceY], pieceBoard[pieceX + 1, pieceY + 1]));
                }
            }
            if (!moved)
            {
                if (InBoundsCheck(pieceX, pieceY + 2) && ClearCheck(pieceX, pieceY, pieceX, pieceY + 2))
                {
                    moves.Add(new Move(pieceX, pieceY, pieceX, pieceY + 2, pieceBoard[pieceX, pieceY], pieceBoard[pieceX, pieceY + 2]));
            }
            }
        }
    }
}
