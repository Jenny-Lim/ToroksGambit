using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Piece
{
    void Awake()
    {
        pieceBoard = Board.GetPieceBoard();
        moves = new List<Move>();
        type = PieceType.queen;
        value = 900;
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


        // does what rook does

        MovesAdd(1, 0);
        MovesAdd(-1, 0);
        MovesAdd(0, 1);
        MovesAdd(0, -1);

        //and a bishop

        MovesAdd(1, 1);
        MovesAdd(-1, -1);
        MovesAdd(1, -1);
        MovesAdd(-1, 1);

        MoveFiltering(moves, isTorok);
    }

}
