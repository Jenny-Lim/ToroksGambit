using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Piece : MonoBehaviour
{
    //piece_value_base_mid_game = [100, 290, 320, 490, 900, 60000]
    //p,n,b,r,q,k,

    public enum PieceType
    {
        pawn,
        knight,
        bishop,
        rook,
        queen,
        king,
        wall,
        hole
    }

    //probably want an is same piece function

    public bool moved = false;
    // list of possible moves
    public List<Move> moves;

    public bool isTaken;
    public int pieceX;
    public int pieceY;
    public PieceType type;

    public bool isTorok;

    public bool isTough;
    public bool lastChance;
    public float value;
    public bool promote;


    public GameObject[,] pieceBoard;

    void Awake()
    {
        //isTaken = false;
        pieceBoard = Board.GetPieceBoard();
        moves = new List<Move>();
    }

    public void Start()
    {
        //if (isTorok)
        //{
          //  value *= -1;
        //}
    }

    public virtual void UpdateMoves()
    {

        // empty to be overwritten
        //moves.Clear();

        //int endX = Board.GetClickedX();
        //int endY = Board.GetClickedY();

        //checks would go here
        //moves.Add(new Move(pieceX, pieceY, endX, endY, pieceBoard[pieceX, pieceY], pieceBoard[endX, endY]));

        // move filtering for king check
    }

    public void MoveFiltering(List<Move> moves) // commented out where this was called, -- InvalidOperationException: Collection was modified; enumeration operation may not execute.
    {
        int i = 0;
        foreach (Move m in moves)
        {
            Board.instance.MovePiece(m.startX, m.startY, m.endX, m.endY);
            if (!Board.instance.IsKingInCheck(isTorok))
            {
                moves.RemoveAt(i);
            }
            Board.instance.UndoMove();
            i++;
        }
    }

    public static bool InBoundsCheck(int endX, int endY)
    {
        if (endX >= Board.boardSize || endY >= Board.boardSize || endX < 0 || endY < 0)
        {
            return false;
        }
        return true;
    }

    public int ClearCheck(int endX, int endY) // not virtual anymore
    {
        if (pieceBoard[endX, endY] != null)
        {
            Piece p = pieceBoard[endX, endY].GetComponent<Piece>();

            if (p.type == PieceType.wall || p.isTorok == this.isTorok) // if its your own piece / is a wall, can't capture
            {
                return 1;
            }
            else if (p.type == PieceType.hole)
            {
                return 2;
            }
            else if (p.isTorok != this.isTorok)//is an enemy -- move there and sop moving
            {
                return 3;
            }

        }

        return 0; 
    } // ClearCheck


    public void MovesAdd(int directionX, int directionY) // not virtual anymore
    {
        for (int i = 1; i < Board.boardSize; i++)
        {
            if (InBoundsCheck(pieceX + (i * directionX), pieceY + (i * directionY)))
            {
                int clearResult = ClearCheck(pieceX + (i * directionX), pieceY + (i * directionY));

                if (clearResult == 0) // if spot is empty
                {
                    //Board.instance.MovePiece(pieceX, pieceY, pieceX + (i * directionX), pieceY + (i * directionY));
                    //if (!Board.instance.IsKingInCheck(isTorok))
                    //{
                        moves.Add(new Move(pieceX, pieceY, pieceX + (i * directionX), pieceY + (i * directionY), pieceBoard[pieceX, pieceY], pieceBoard[pieceX + (i * directionX), pieceY + (i * directionY)], -1, -1));
                    //}
                    //Board.instance.UndoMove();
                }
                else if (clearResult == 1) // if spot is wall / same color
                {
                    return;
                }

                else if (clearResult == 3) // if spot is capturable -- need to score these ones + add to the capture list
                {
                    Piece p = pieceBoard[pieceX + (i * directionX), pieceY + (i * directionY)].GetComponent<Piece>();

                    //Board.instance.MovePiece(pieceX, pieceY, pieceX + (i * directionX), pieceY + (i * directionY));
                    //if (!Board.instance.IsKingInCheck(isTorok))
                    //{
                        moves.Add(new Move(pieceX, pieceY, pieceX + (i * directionX), pieceY + (i * directionY), pieceBoard[pieceX, pieceY], pieceBoard[pieceX + (i * directionX), pieceY + (i * directionY)], (int)this.type, (int)p.type));
                    //}
                    //Board.instance.UndoMove();
                    //print(this.type.ToString());
                    //moves.Add(new Move(pieceX, pieceY, pieceX + (i * directionX), pieceY + (i * directionY), pieceBoard[pieceX, pieceY], pieceBoard[pieceX + (i * directionX), pieceY + (i * directionY)]));
                    return;
                }

                else if (clearResult == 2) // if spot is a hole
                {
                    continue;
                }
            }
        }
    } // MovesAdd


    public void MovesAdd_K(int endX, int endY) // K for [k]ing, [k]night
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
                        //Board.instance.MovePiece(pieceX, pieceY, endX, endY);
                        //if (!Board.instance.IsKingInCheck(isTorok))
                        //{
                            moves.Add(new Move(pieceX, pieceY, endX, endY, pieceBoard[pieceX, pieceY], pieceBoard[endX, endY], (int)this.type, (int)p.type));
                        //}
                        //Board.instance.UndoMove();
                    }
                }
            }

            else
            {
                if (clearResult == 0)
                {
                    //Board.instance.MovePiece(pieceX, pieceY, endX, endY);
                    //if (!Board.instance.IsKingInCheck(isTorok))
                    //{
                        moves.Add(new Move(pieceX, pieceY, endX, endY, pieceBoard[pieceX, pieceY], pieceBoard[endX, endY], -1, -1));
                    //}
                    //Board.instance.UndoMove();
                }
            }
        }
    } // MovesAdd_K


    public bool IsOnSameTeam(GameObject compareTo)
    {
        Piece with = compareTo.GetComponent<Piece>();

        if (!with)
        {
            return false;
        }

        if (isTorok && with.isTorok)
        {
            return true;
        }
        return false;
    }

}
