using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Piece
{

    void Awake()
    {
        pieceBoard = Board.GetPieceBoard();
        moves = new List<Move>();
        type = PieceType.rook;
        value = 490;
    }

    public override void UpdateMoves()
    {
        moves.Clear();

        if (isInvulnerable)
        {
            return;
        }

        Vector2Int pos = Board.instance.GetPieceLocation(this.gameObject);
        pieceX = pos.x;
        pieceY = pos.y;

        MovesAdd(1, 0);
        MovesAdd(-1, 0);
        MovesAdd(0, 1);
        MovesAdd(0, -1);

        MoveFiltering(moves);

    }

}
