using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
{
    private bool moved;
    private bool capturing;

    // Start is called before the first frame update
    void Start()
    {
        type = "pawn";
        moved = false;
        capturing = false; // capturing == true if theres an enemy piece
    }

    void UpdateMoves() // have different moves for black and white
    {
        moves.Add(new Move(pieceX, pieceY, pieceX, pieceY + 1, pieceBoard[pieceX, pieceY], pieceBoard[pieceX, pieceY+1]));

        if (capturing)
        {
            moves.Add(new Move(pieceX, pieceY, pieceX, pieceY + 1, pieceBoard[pieceX, pieceY], pieceBoard[pieceX+1, pieceY+1]));
        }
        if (moved)
        {
            moves.Add(new Move(pieceX, pieceY, pieceX, pieceY + 2, pieceBoard[pieceX, pieceY], pieceBoard[pieceX, pieceY+2]));
            // somewhere we have to say moved = true/false;
        }
    }
}
