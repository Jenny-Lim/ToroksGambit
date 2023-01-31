using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Piece
{
    // Start is called before the first frame update
    void Start()
    {
        type = PieceType.queen;
        value = 8;
    }

    public override void UpdateMoves()
    {
        moves.Clear();

        //for (int i = 0; i < Board.boardSize; i++)
        //{
            // does what bishop does

            MovesAdd(1, 1);
            MovesAdd(-1, -1);
            MovesAdd(1, -1);
            MovesAdd(-1, 1);

            //if (InBoundsCheck(pieceX + i, pieceY + i) && ClearCheck(pieceX, pieceY, pieceX + i, pieceY + i))
            //{
            //    moves.Add(new Move(pieceX, pieceY, pieceX + i, pieceY + i, pieceBoard[pieceX, pieceY], pieceBoard[pieceX + i, pieceY + i]));
            //}
            //if (InBoundsCheck(pieceX - i, pieceY - i) && ClearCheck(pieceX, pieceY, pieceX - i, pieceY - i))
            //{
            //    moves.Add(new Move(pieceX, pieceY, pieceX - i, pieceY - i, pieceBoard[pieceX, pieceY], pieceBoard[pieceX - i, pieceY - i]));
            //}
            //if (InBoundsCheck(pieceX + i, pieceY - i) && ClearCheck(pieceX, pieceY, pieceX + i, pieceY - i))
            //{
            //    moves.Add(new Move(pieceX, pieceY, pieceX + i, pieceY - i, pieceBoard[pieceX, pieceY], pieceBoard[pieceX + i, pieceY - i]));
            //}
            //if (InBoundsCheck(pieceX - i, pieceY + i) && ClearCheck(pieceX, pieceY, pieceX - i, pieceY + i))
            //{
            //    moves.Add(new Move(pieceX, pieceY, pieceX - i, pieceY + i, pieceBoard[pieceX, pieceY], pieceBoard[pieceX - i, pieceY + i]));
            //}


            // does what rook does

            MovesAdd(1, 0);
            MovesAdd(-1, 0);
            MovesAdd(0, 1);
            MovesAdd(0, -1);

            //if (InBoundsCheck(pieceX + i, pieceY) && ClearCheck(pieceX, pieceY, pieceX + i, pieceY))
            //{
            //    moves.Add(new Move(pieceX, pieceY, pieceX + i, pieceY, pieceBoard[pieceX, pieceY], pieceBoard[pieceX + i, pieceY]));
            //}

            //if (InBoundsCheck(pieceX, pieceY + i) && ClearCheck(pieceX, pieceY, pieceX, pieceY + i))
            //{
            //    moves.Add(new Move(pieceX, pieceY, pieceX, pieceY + i, pieceBoard[pieceX, pieceY], pieceBoard[pieceX, pieceY + i]));
            //}

            //if (InBoundsCheck(pieceX - i, pieceY) && ClearCheck(pieceX, pieceY, pieceX - i, pieceY))
            //{
            //    moves.Add(new Move(pieceX, pieceY, pieceX - i, pieceY, pieceBoard[pieceX, pieceY], pieceBoard[pieceX - i, pieceY]));
            //}

            //if (InBoundsCheck(pieceX, pieceY - i) && ClearCheck(pieceX, pieceY, pieceX, pieceY - i))
            //{
            //    moves.Add(new Move(pieceX, pieceY, pieceX, pieceY - i, pieceBoard[pieceX, pieceY], pieceBoard[pieceX, pieceY - i]));
            //}
        //}
    }

}
