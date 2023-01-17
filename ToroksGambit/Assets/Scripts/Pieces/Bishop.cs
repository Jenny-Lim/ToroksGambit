using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Piece
{
    // Start is called before the first frame update
    void Start()
    {
        type = "bishop";
    }

    void UpdateMoves()
    {
        for (int i = 0; i < 0; i++)
        {
            moves.Add(new Move(pieceX, pieceY, pieceX + i, pieceY + i, pieceBoard[pieceX, pieceY], pieceBoard[pieceX + i, pieceY + i]));
            moves.Add(new Move(pieceX, pieceY, pieceX - i, pieceY - i, pieceBoard[pieceX, pieceY], pieceBoard[pieceX - i, pieceY - i]));
        }
    }

}
