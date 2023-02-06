using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : Piece // pieces can jump over these, just cannot capture
{
    // Start is called before the first frame update
    void Awake()
    {
        type = PieceType.hole;
        pieceBoard = Board.GetPieceBoard();
        moves = new List<Move>();
    }

    public override void UpdateMoves()
    {
        //empty
    }

}
