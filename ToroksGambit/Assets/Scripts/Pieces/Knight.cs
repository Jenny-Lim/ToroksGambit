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
        value = 3;
    }

    public override void UpdateMoves() // knights can jump over pieces
    {
        moves.Clear();
        int clearResult;

        Vector2Int pos = Board.instance.GetPieceLocation(this.gameObject);
        pieceX = pos.x;
        pieceY = pos.y;

        if (InBoundsCheck(pieceX + 1, pieceY + 2))
        {
            clearResult = ClearCheck(pieceX + 1, pieceY + 2);
            if (clearResult == 0 || clearResult == 3)
            {
                moves.Add(new Move(pieceX, pieceY, pieceX + 1, pieceY + 2, pieceBoard[pieceX, pieceY], pieceBoard[pieceX + 1, pieceY + 2]));
            }
        }

        if (InBoundsCheck(pieceX + 2, pieceY + 1))
        {
            clearResult = ClearCheck(pieceX + 2, pieceY + 1);
            if (clearResult == 0 || clearResult == 3)
            {
                moves.Add(new Move(pieceX, pieceY, pieceX + 2, pieceY + 1, pieceBoard[pieceX, pieceY], pieceBoard[pieceX + 2, pieceY + 1]));
            }
        }

        if (InBoundsCheck(pieceX - 1, pieceY - 2))
        {
            clearResult = ClearCheck(pieceX - 1, pieceY - 2);
            if (clearResult == 0 || clearResult == 3)
            {
                moves.Add(new Move(pieceX, pieceY, pieceX - 1, pieceY - 2, pieceBoard[pieceX, pieceY], pieceBoard[pieceX - 1, pieceY - 2]));
            }
        }

        if (InBoundsCheck(pieceX - 2, pieceY - 1))
        {
            clearResult = ClearCheck(pieceX - 2, pieceY - 1);
            if (clearResult == 0 || clearResult == 3)
            {
                moves.Add(new Move(pieceX, pieceY, pieceX - 2, pieceY - 1, pieceBoard[pieceX, pieceY], pieceBoard[pieceX - 2, pieceY - 1]));
            }
        }

        if (InBoundsCheck(pieceX + 1, pieceY - 2))
        {
            clearResult = ClearCheck(pieceX + 1, pieceY - 2);
            if (clearResult == 0 || clearResult == 3)
            {
                moves.Add(new Move(pieceX, pieceY, pieceX + 1, pieceY - 2, pieceBoard[pieceX, pieceY], pieceBoard[pieceX + 1, pieceY - 2]));
            }
        }

        if (InBoundsCheck(pieceX + 2, pieceY - 1))
        {
            clearResult = ClearCheck(pieceX + 2, pieceY - 1);
            if (clearResult == 0 || clearResult == 3)
            {
                moves.Add(new Move(pieceX, pieceY, pieceX + 2, pieceY - 1, pieceBoard[pieceX, pieceY], pieceBoard[pieceX + 2, pieceY - 1]));
            }

        }

        if (InBoundsCheck(pieceX - 1, pieceY + 2))
        {
            clearResult = ClearCheck(pieceX - 1, pieceY + 2);
            if (clearResult == 0 || clearResult == 3)
            {
                moves.Add(new Move(pieceX, pieceY, pieceX - 1, pieceY + 2, pieceBoard[pieceX, pieceY], pieceBoard[pieceX - 1, pieceY + 2]));
            }
        }

        if (InBoundsCheck(pieceX - 2, pieceY + 1))
        {
            clearResult = ClearCheck(pieceX - 2, pieceY + 1);
            if (clearResult == 0 || clearResult == 3)
            {
                moves.Add(new Move(pieceX, pieceY, pieceX - 2, pieceY + 1, pieceBoard[pieceX, pieceY], pieceBoard[pieceX - 2, pieceY + 1]));
            }
        }

    }

}
