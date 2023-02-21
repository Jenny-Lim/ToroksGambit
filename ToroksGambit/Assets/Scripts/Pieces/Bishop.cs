using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Piece
{
    // Start is called before the first frame update
    void Awake()
    {
        pieceBoard = Board.GetPieceBoard();
        moves = new List<Move>();
        type = PieceType.bishop;
        value = 320;
    }

    public override void UpdateMoves()
    {
        moves.Clear();

        Vector2Int pos = Board.instance.GetPieceLocation(this.gameObject);
        pieceX = pos.x;
        pieceY = pos.y;

        MovesAdd(1, 1);
        MovesAdd(-1, -1);
        MovesAdd(1, -1);
        MovesAdd(-1, 1);


    }
}
