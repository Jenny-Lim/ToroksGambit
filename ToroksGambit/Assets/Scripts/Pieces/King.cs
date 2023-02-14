using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class King : Piece
{
    // Start is called before the first frame update
    void Awake()
    {
        pieceBoard = Board.GetPieceBoard();
        moves = new List<Move>();
        type = PieceType.king;
        value = 1000;
    }

    public override void UpdateMoves()
    {
        //int clearResult;
        moves.Clear();

        Vector2Int pos = Board.instance.GetPieceLocation(this.gameObject);
        pieceX = pos.x;
        pieceY = pos.y;

        //if (InBoundsCheck(pieceX + 1, pieceY + 1))
        //{
        //    clearResult = ClearCheck(pieceX + 1, pieceY + 1);
        //    if (clearResult == 0 || clearResult == 3) // if its 3 then do the thing
        //    {
        //        moves.Add(new Move(pieceX, pieceY, pieceX + 1, pieceY + 1, pieceBoard[pieceX, pieceY], pieceBoard[pieceX + 1, pieceY + 1]));
        //    }
        //}

        //if (InBoundsCheck(pieceX - 1, pieceY - 1))
        //{
        //    clearResult = ClearCheck(pieceX - 1, pieceY - 1);
        //    if (clearResult == 0 || clearResult == 3)
        //    {
        //        moves.Add(new Move(pieceX, pieceY, pieceX - 1, pieceY - 1, pieceBoard[pieceX, pieceY], pieceBoard[pieceX - 1, pieceY - 1]));
        //    }
        //}

        //if (InBoundsCheck(pieceX + 1, pieceY))
        //{
        //    clearResult = ClearCheck(pieceX + 1, pieceY);
        //    if (clearResult == 0 || clearResult == 3)
        //    {
        //        moves.Add(new Move(pieceX, pieceY, pieceX + 1, pieceY, pieceBoard[pieceX, pieceY], pieceBoard[pieceX + 1, pieceY]));
        //    }
        //}

        //if (InBoundsCheck(pieceX, pieceY + 1))
        //{
        //    clearResult = ClearCheck(pieceX, pieceY + 1);
        //    if (clearResult == 0 || clearResult == 3)
        //    {
        //        moves.Add(new Move(pieceX, pieceY, pieceX, pieceY + 1, pieceBoard[pieceX, pieceY], pieceBoard[pieceX, pieceY + 1]));
        //    }
        //}

        //if (InBoundsCheck(pieceX - 1, pieceY))
        //{
        //    clearResult = ClearCheck(pieceX - 1, pieceY);
        //    if (clearResult == 0 || clearResult == 3)
        //    {
        //        moves.Add(new Move(pieceX, pieceY, pieceX - 1, pieceY, pieceBoard[pieceX, pieceY], pieceBoard[pieceX - 1, pieceY]));
        //    }
        //}

        //if (InBoundsCheck(pieceX, pieceY - 1))
        //{
        //    clearResult = ClearCheck(pieceX, pieceY - 1);
        //    if (clearResult == 0 || clearResult == 3)
        //    {
        //        moves.Add(new Move(pieceX, pieceY, pieceX, pieceY - 1, pieceBoard[pieceX, pieceY], pieceBoard[pieceX, pieceY - 1]));
        //    }
        //}

        MovesAdd(pieceX + 1, pieceY + 1);
        MovesAdd(pieceX - 1, pieceY - 1);
        MovesAdd(pieceX + 1, pieceY);
        MovesAdd(pieceX, pieceY + 1);
        MovesAdd(pieceX - 1, pieceY);
        MovesAdd(pieceX, pieceY - 1);
    }

    public override void MovesAdd(int endX, int endY)
    {
        int clearResult;
        if (InBoundsCheck(endX, endY))
        {
            clearResult = ClearCheck(endX, endY);
            if (pieceBoard[endX, endY] != null)
            {
                Piece p = pieceBoard[endX, endY].GetComponent<Piece>();
                if (p != null)
                {
                    if (clearResult == 3) // if its 3 then do the thing
                    {
                        moves.Add(new Move(pieceX, pieceY, endX, endY, pieceBoard[pieceX, pieceY], pieceBoard[endX, endY], (int)this.type, (int)p.type));
                    }
                }
            }

            else
            {
                if (clearResult == 0)
                {
                    moves.Add(new Move(pieceX, pieceY, endX, endY, pieceBoard[pieceX, pieceY], pieceBoard[endX, endY], -1, -1));
                }
            }
        }
    }
}
