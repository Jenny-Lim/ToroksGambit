using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Piece : MonoBehaviour
{

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

    public virtual void UpdateMoves()
    {

        // empty to be overwritten -- maybe it can hold default functionality (see pats movePiece)
        //moves.Clear();

        //int endX = Board.GetClickedX();
        //int endY = Board.GetClickedY();

        //checks would go here
        //moves.Add(new Move(pieceX, pieceY, endX, endY, pieceBoard[pieceX, pieceY], pieceBoard[endX, endY]));
    }

    public bool InBoundsCheck(int endX, int endY)
    {
        if (endX >= Board.boardSize || endY >= Board.boardSize || endX < 0 || endY < 0)
        {
            return false;
        }
        return true;
    }

    public virtual int ClearCheck(int endX, int endY) // for new vers, only need the end params and it should return int
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


    public void MovesAdd(int directionX, int directionY)
    {
        for (int i = 1; i < Board.boardSize; i++)
        {
            if (InBoundsCheck(pieceX + (i * directionX), pieceY + (i * directionY)))
            {
                int clearResult = ClearCheck(pieceX + (i * directionX), pieceY + (i * directionY));

                if (clearResult == 0) // if spot is empty
                {
                    moves.Add(new Move(pieceX, pieceY, pieceX + (i * directionX), pieceY + (i * directionY), pieceBoard[pieceX, pieceY], pieceBoard[pieceX + (i * directionX), pieceY + (i * directionY)]));
                }
                else if (clearResult == 1) // if spot is wall / same color
                {
                    return;
                }

                else if (clearResult == 3) // if spot is capturable -- need to score these ones + add to the capture list
                {
                    //moves.Add(new Move(pieceX, pieceY, pieceX + (i * directionX), pieceY + (i * directionY), pieceBoard[pieceX, pieceY], pieceBoard[pieceX + (i * directionX), pieceY + (i * directionY)], pieceBoard[pieceX, pieceY].type, pieceBoard[pieceX + (i * directionX), pieceY + (i * directionY)].type));
                    moves.Add(new Move(pieceX, pieceY, pieceX + (i * directionX), pieceY + (i * directionY), pieceBoard[pieceX, pieceY], pieceBoard[pieceX + (i * directionX), pieceY + (i * directionY)]));
                    return;
                }

                else if (clearResult == 2) // if spot is a hole
                {
                    continue;
                }
            }
        }
    } // MovesAdd

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
