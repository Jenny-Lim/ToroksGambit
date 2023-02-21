using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece
{
    // Start is called before the first frame update
    void Awake()
    {
        pieceBoard = Board.GetPieceBoard();
        moves = new List<Move>();
        type = PieceType.knight;
        value = 290;
    }

    public override void UpdateMoves() // knights can jump over pieces
    {
        moves.Clear();
        //int clearResult;

        Vector2Int pos = Board.instance.GetPieceLocation(this.gameObject);
        pieceX = pos.x;
        pieceY = pos.y;

        MovesAdd(pieceX + 1, pieceY + 2);
        MovesAdd(pieceX + 2, pieceY + 1);
        MovesAdd(pieceX - 1, pieceY - 2);
        MovesAdd(pieceX - 2, pieceY - 1);
        MovesAdd(pieceX + 1, pieceY - 2);
        MovesAdd(pieceX + 2, pieceY - 1);
        MovesAdd(pieceX - 1, pieceY + 2);
        MovesAdd(pieceX - 2, pieceY + 1);

        //if (InBoundsCheck(pieceX + 1, pieceY + 2))
        //{
        //    clearResult = ClearCheck(pieceX + 1, pieceY + 2);
        //    if (clearResult == 3) // if its 3 then do the thing
        //    {
        //        //moves.Add(new Move(pieceX, pieceY, pieceX + 1, pieceY + 2, pieceBoard[pieceX, pieceY], pieceBoard[pieceX + 1, pieceY + 2]));
        //        Piece p = pieceBoard[pieceX + 1, pieceY + 2].GetComponent<Piece>();
        //        if (p != null)
        //        {
        //            moves.Add(new Move(pieceX, pieceY, pieceX + 1, pieceY + 2, pieceBoard[pieceX, pieceY], pieceBoard[pieceX + 1, pieceY + 2], (int)this.type, (int)p.type));
        //        }
        //    }
        //    else if (clearResult == 0)
        //    {
        //        moves.Add(new Move(pieceX, pieceY, pieceX + 1, pieceY + 2, pieceBoard[pieceX, pieceY], pieceBoard[pieceX + 1, pieceY + 2], -1, -1));
        //    }
        //}

        //if (InBoundsCheck(pieceX + 2, pieceY + 1))
        //{
        //    clearResult = ClearCheck(pieceX + 2, pieceY + 1);
        //    if (clearResult == 3)
        //    {
        //        Piece p = pieceBoard[pieceX + 1, pieceY + 2].GetComponent<Piece>();
        //        if (p != null)
        //        {
        //            //moves.Add(new Move(pieceX, pieceY, pieceX + 2, pieceY + 1, pieceBoard[pieceX, pieceY], pieceBoard[pieceX + 2, pieceY + 1]));
        //            moves.Add(new Move(pieceX, pieceY, pieceX + 2, pieceY + 1, pieceBoard[pieceX, pieceY], pieceBoard[pieceX + 2, pieceY + 1], (int)this.type, (int)p.type));
        //        }
        //    }
        //    else if (clearResult == 0)
        //    {
        //        moves.Add(new Move(pieceX, pieceY, pieceX + 2, pieceY + 1, pieceBoard[pieceX, pieceY], pieceBoard[pieceX + 2, pieceY + 1], -1, -1));
        //    }
        //}

        //if (InBoundsCheck(pieceX - 1, pieceY - 2))
        //{
        //    clearResult = ClearCheck(pieceX - 1, pieceY - 2);
        //    if (clearResult == 3)
        //    {
        //        moves.Add(new Move(pieceX, pieceY, pieceX - 1, pieceY - 2, pieceBoard[pieceX, pieceY], pieceBoard[pieceX - 1, pieceY - 2], (int)this.type, (int)pieceBoard[pieceX - 1, pieceY - 2].type));
        //    }
        //    else if (clearResult == 0)
        //    {
        //        moves.Add(new Move(pieceX, pieceY, pieceX - 1, pieceY - 2, pieceBoard[pieceX, pieceY], pieceBoard[pieceX - 1, pieceY - 2], -1, -1));
        //    }
        //}

        //if (InBoundsCheck(pieceX - 2, pieceY - 1))
        //{
        //    clearResult = ClearCheck(pieceX - 2, pieceY - 1);
        //    if (clearResult == 0 || clearResult == 3)
        //    {
        //        moves.Add(new Move(pieceX, pieceY, pieceX - 2, pieceY - 1, pieceBoard[pieceX, pieceY], pieceBoard[pieceX - 2, pieceY - 1], ));
        //    }
        //    else if ()
        //    {

        //    }
        //}

        //if (InBoundsCheck(pieceX + 1, pieceY - 2))
        //{
        //    clearResult = ClearCheck(pieceX + 1, pieceY - 2);
        //    if (clearResult == 0 || clearResult == 3)
        //    {
        //        moves.Add(new Move(pieceX, pieceY, pieceX + 1, pieceY - 2, pieceBoard[pieceX, pieceY], pieceBoard[pieceX + 1, pieceY - 2]));
        //    }
        //}

        //if (InBoundsCheck(pieceX + 2, pieceY - 1))
        //{
        //    clearResult = ClearCheck(pieceX + 2, pieceY - 1);
        //    if (clearResult == 0 || clearResult == 3)
        //    {
        //        moves.Add(new Move(pieceX, pieceY, pieceX + 2, pieceY - 1, pieceBoard[pieceX, pieceY], pieceBoard[pieceX + 2, pieceY - 1]));
        //    }
        //}

        //if (InBoundsCheck(pieceX - 1, pieceY + 2))
        //{
        //    clearResult = ClearCheck(pieceX - 1, pieceY + 2);
        //    if (clearResult == 0 || clearResult == 3)
        //    {
        //        moves.Add(new Move(pieceX, pieceY, pieceX - 1, pieceY + 2, pieceBoard[pieceX, pieceY], pieceBoard[pieceX - 1, pieceY + 2]));
        //    }
        //}

        //if (InBoundsCheck(pieceX - 2, pieceY + 1))
        //{
        //    clearResult = ClearCheck(pieceX - 2, pieceY + 1);
        //    if (clearResult == 0 || clearResult == 3)
        //    {
        //        moves.Add(new Move(pieceX, pieceY, pieceX - 2, pieceY + 1, pieceBoard[pieceX, pieceY], pieceBoard[pieceX - 2, pieceY + 1]));
        //    }
        //}

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
               if (clearResult == 0) {
                    moves.Add(new Move(pieceX, pieceY, endX, endY, pieceBoard[pieceX, pieceY], pieceBoard[endX, endY], -1, -1));
                }
            }
        }
    }
}

