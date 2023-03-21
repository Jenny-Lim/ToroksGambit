using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Piece
{
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

        if (isInvulnerable)
        {
            return;
        }

        Vector2Int pos = Board.instance.GetPieceLocation(this.gameObject);
        pieceX = pos.x;
        pieceY = pos.y;

        MovesAdd(1, 1);
        MovesAdd(-1, -1);
        MovesAdd(1, -1);
        MovesAdd(-1, 1);

        MoveFiltering(moves, isTorok);

    }
}
