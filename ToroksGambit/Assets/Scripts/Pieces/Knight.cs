using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece
{
    void Awake()
    {
        pieceBoard = Board.GetPieceBoard();
        moves = new List<Move>();
        type = PieceType.knight;
        value = 290;
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

        MovesAdd_K(pieceX + 1, pieceY + 2);
        MovesAdd_K(pieceX + 2, pieceY + 1);
        MovesAdd_K(pieceX - 1, pieceY - 2);
        MovesAdd_K(pieceX - 2, pieceY - 1);
        MovesAdd_K(pieceX + 1, pieceY - 2);
        MovesAdd_K(pieceX + 2, pieceY - 1);
        MovesAdd_K(pieceX - 1, pieceY + 2);
        MovesAdd_K(pieceX - 2, pieceY + 1);

        MoveFiltering(moves, isTorok);

        //base.UpdateMoves();
    }

    //public override void MovesAdd(int endX, int endY)
    //{
    //    int clearResult;
    //    if (InBoundsCheck(endX, endY))
    //    {
    //        clearResult = ClearCheck(endX, endY);
    //        if (pieceBoard[endX, endY] != null)
    //        {
    //            Piece p = pieceBoard[endX, endY].GetComponent<Piece>();
    //            if (p != null)
    //            {
    //                if (clearResult == 3) // if its 3 then do the thing
    //                {
    //                    moves.Add(new Move(pieceX, pieceY, endX, endY, pieceBoard[pieceX, pieceY], pieceBoard[endX, endY], (int)this.type, (int)p.type));
    //                }
    //            }
    //        }

    //        else
    //        {
    //            if (clearResult == 0)
    //            {
    //                moves.Add(new Move(pieceX, pieceY, endX, endY, pieceBoard[pieceX, pieceY], pieceBoard[endX, endY], -1, -1));
    //            }
    //        }
    //    }
    //}
}

