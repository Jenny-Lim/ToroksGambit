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

        for (int i = 0; i < 0; i++)
        {

            if (InBoundsCheck(pieceX + i, pieceY))
            {
                moves.Add(new Move(pieceX, pieceY, pieceX + i, pieceY, pieceBoard[pieceX, pieceY], pieceBoard[pieceX + i, pieceY]));
            }

            if (InBoundsCheck(pieceX, pieceY + i))
            {
                moves.Add(new Move(pieceX, pieceY, pieceX, pieceY + i, pieceBoard[pieceX, pieceY], pieceBoard[pieceX, pieceY + i]));
            }

            if (InBoundsCheck(pieceX - i, pieceY))
            {
                moves.Add(new Move(pieceX, pieceY, pieceX - i, pieceY, pieceBoard[pieceX, pieceY], pieceBoard[pieceX - i, pieceY]));
            }

            if (InBoundsCheck(pieceX, pieceY - i))
            {
                moves.Add(new Move(pieceX, pieceY, pieceX, pieceY - i, pieceBoard[pieceX, pieceY], pieceBoard[pieceX, pieceY - i]));
            }
        }
    }

}
