using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
{
    void Awake()
    {
        pieceBoard = Board.GetPieceBoard();
        moves = new List<Move>();
        type = PieceType.pawn;
        value = 100;
    }

    public override void UpdateMoves()
    {
        moves.Clear();
        if (isInvulnerable)
        {
            return;
        }

        int dir = 1;
        int clearResult;

        Vector2Int pos = Board.instance.GetPieceLocation(this.gameObject);

        int pieceX = pos.x;
        int pieceY = pos.y;

        if (isTorok)
        {
            dir = -1;
        }


        if (InBoundsCheck(pieceX, pieceY + dir))
        {
            clearResult = ClearCheck(pieceX, pieceY + dir); // can just check if the spot is null instead

            if (clearResult == 0)
            {
                //moves.Add(new Move(pieceX, pieceY, pieceX, pieceY + dir, pieceBoard[pieceX, pieceY], pieceBoard[pieceX, pieceY + dir]));
                moves.Add(new Move(pieceX, pieceY, pieceX, pieceY + dir, pieceBoard[pieceX, pieceY], pieceBoard[pieceX, pieceY + dir], -1, -1));
            }
        }

        if (InBoundsCheck(pieceX + 1, pieceY + dir) && pieceBoard[pieceX + 1, pieceY + dir] != null)
        {
            Piece pRight = pieceBoard[pieceX + 1, pieceY + dir].GetComponent<Piece>();
            //print(IsOnSameTeam(pieceBoard[pieceX + 1, pieceY + dir]));
            //print(pRight.isTorok);
            //print(isTorok);
            //if (pRight != null && InBoundsCheck(pieceX + 1, pieceY + dir) && pieceBoard[pieceX + 1, pieceY + dir] != null && !IsOnSameTeam(pieceBoard[pieceX + 1, pieceY + dir]) && !pRight.isTough && (int)pRight.type < 6) // if can capture another piece
            if (pRight != null && InBoundsCheck(pieceX + 1, pieceY + dir) && pieceBoard[pieceX + 1, pieceY + dir] != null && isTorok != pRight.isTorok && !pRight.isTough && (int)pRight.type < 6)
            {
                moves.Add(new Move(pieceX, pieceY, pieceX + 1, pieceY + dir, pieceBoard[pieceX, pieceY], pieceBoard[pieceX + 1, pieceY + dir], (int)this.type, (int)pRight.type));
                //moves.Add(new Move(pieceX, pieceY, pieceX + 1, pieceY + dir, pieceBoard[pieceX, pieceY], pieceBoard[pieceX + 1, pieceY + dir]));
            }
        }
        

        if (InBoundsCheck(pieceX - 1, pieceY + dir) && pieceBoard[pieceX - 1, pieceY + dir] != null)
        {
            Piece pLeft = pieceBoard[pieceX - 1, pieceY + dir].GetComponent<Piece>();
            //print(IsOnSameTeam(pieceBoard[pieceX - 1, pieceY + dir]));
            //print(pLeft.isTorok);
            //print(isTorok);
            //if (pLeft != null && InBoundsCheck(pieceX - 1, pieceY + dir) && pieceBoard[pieceX - 1, pieceY + dir] != null && !IsOnSameTeam(pieceBoard[pieceX - 1, pieceY + dir]) && !pLeft.isTough && (int)pLeft.type < 6) // if can capture another piece
            if (pLeft != null && InBoundsCheck(pieceX - 1, pieceY + dir) && pieceBoard[pieceX - 1, pieceY + dir] != null && isTorok != pLeft.isTorok && !pLeft.isTough && (int)pLeft.type < 6)
            {
                moves.Add(new Move(pieceX, pieceY, pieceX -1, pieceY + dir, pieceBoard[pieceX, pieceY], pieceBoard[pieceX - 1, pieceY + dir], (int)this.type, (int)pLeft.type));
                //moves.Add(new Move(pieceX, pieceY, pieceX - 1, pieceY + dir, pieceBoard[pieceX, pieceY], pieceBoard[pieceX - 1, pieceY + dir]));
            }
        }
        

        if (!moved)
        {
            if (InBoundsCheck(pieceX, pieceY + dir * 2))
            {
                
                clearResult = ClearCheck(pieceX, pieceY + dir * 2);
                if (clearResult == 0 && pieceBoard[pieceX, pieceY + dir] == null) // checks if the spot before is empty as well **changed pieceX + dir -> pieceX**
                {
                    //moves.Add(new Move(pieceX, pieceY, pieceX, pieceY + dir * 2, pieceBoard[pieceX, pieceY], pieceBoard[pieceX, pieceY + dir * 2]));
                    moves.Add(new Move(pieceX, pieceY, pieceX, pieceY + dir * 2, pieceBoard[pieceX, pieceY], pieceBoard[pieceX, pieceY + dir * 2], -1, -1));
                }
            }
        }

        MoveFiltering(moves);
    }
}
