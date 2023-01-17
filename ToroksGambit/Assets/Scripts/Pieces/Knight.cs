using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece
{
    // Start is called before the first frame update
    void Start()
    {
        type = "knight";
    }

    void UpdateMoves()
    {
        if (InBoundsCheck(pieceX + 1, pieceY + 2))
        {
            moves.Add(new Move(pieceX, pieceY, pieceX + 1, pieceY + 2, pieceBoard[pieceX, pieceY], pieceBoard[pieceX + 1, pieceY + 2]));
        }
        if (InBoundsCheck(pieceX + 2, pieceY + 1))
        {
            moves.Add(new Move(pieceX, pieceY, pieceX + 2, pieceY + 1, pieceBoard[pieceX, pieceY], pieceBoard[pieceX + 2, pieceY + 1]));
        }
        if (InBoundsCheck(pieceX - 1, pieceY - 2))
        {
            moves.Add(new Move(pieceX, pieceY, pieceX - 1, pieceY - 2, pieceBoard[pieceX, pieceY], pieceBoard[pieceX - 1, pieceY - 2]));
        }
        if (InBoundsCheck(pieceX - 2, pieceY - 1))
        {
            moves.Add(new Move(pieceX, pieceY, pieceX - 2, pieceY - 1, pieceBoard[pieceX, pieceY], pieceBoard[pieceX - 2, pieceY - 1]));
        }
    }

}
