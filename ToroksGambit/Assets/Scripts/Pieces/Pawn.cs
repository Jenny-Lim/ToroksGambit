using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
{
    private bool isFirstMove;
    private bool capturing;

    // Start is called before the first frame update
    void Start()
    {
        type = "pawn";
        isFirstMove = true;
        capturing = false; // capturing == true if theres an enemy piece
    }

    void UpdateMove() // wont need this with undostorage -> move presumably
    {
        //validMoves[pieceX, pieceY + 1] = true;
        moves.Add(new Move(pieceX, pieceY + 1));

        if (capturing)
        {
            //validMoves[pieceX + 1, pieceY + 1] = true;
            moves.Add(new Move(pieceX + 1, pieceY + 1));
        }
        if (isFirstMove)
        {
            //validMoves[pieceX, pieceY + 2] = true;
            moves.Add(new Move(pieceX, pieceY + 2));

            // somewhere we have to say isFirstMove = false;
        }
    }
}
