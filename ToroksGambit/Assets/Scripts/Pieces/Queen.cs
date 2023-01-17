using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Piece
{
    // Start is called before the first frame update
    void Start()
    {
        type = "queen";
    }

    void UpdateBoard()
    {
        for (int i = 0; i < 0; i++)
        {
            moves.Add(new Move(pieceX, pieceY, pieceX + i, pieceY + i, pieceBoard[pieceX, pieceY], pieceBoard[pieceX + i, pieceY + i]));
            moves.Add(new Move(pieceX, pieceY, pieceX - i, pieceY - i, pieceBoard[pieceX, pieceY], pieceBoard[pieceX - i, pieceY - i]));

            moves.Add(new Move(pieceX, pieceY, pieceX + i, pieceY, pieceBoard[pieceX, pieceY], pieceBoard[pieceX + i, pieceY]));
            moves.Add(new Move(pieceX, pieceY, pieceX - i, pieceY, pieceBoard[pieceX, pieceY], pieceBoard[pieceX - i, pieceY]));
            moves.Add(new Move(pieceX, pieceY, pieceX, pieceY + i, pieceBoard[pieceX, pieceY], pieceBoard[pieceX, pieceY + i]));
            moves.Add(new Move(pieceX, pieceY, pieceX, pieceY - i, pieceBoard[pieceX, pieceY], pieceBoard[pieceX, pieceY - i]));
        }
    }

}
