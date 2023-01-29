using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{

    //probably want an is same piece function

    public bool moved = false;
    // list of possible moves
    public List<Move> moves;

    public bool isTaken;
    public int pieceX;
    public int pieceY;
    public string type;

    public bool isTorok;


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

    public virtual bool ClearCheck(int pieceX, int pieceY, int endX, int endY)
    {
        /*if (pieceBoard[endX, endY] != null)
        {
            Piece p = pieceBoard[endX, endY].GetComponent<Piece>();

            if (p.type == "wall" || p.type == "hole") // if its your own piece, can't capture
            {
                return false;
            }
            else if (p.isTorok != this.isTorok)//is an enemy
            {
                return true;
            }
        }
        
         return false;
         */



        bool isClear = true;
        Piece thePiece = pieceBoard[pieceX, pieceY].GetComponent<Piece>();

        // handling this in here for now, how expensive is getcomponent
        if (pieceBoard[endX, endY] != null)
        {
            Piece p = pieceBoard[endX, endY].GetComponent<Piece>();

            if (p.type == "wall" || p.type == "hole" || p.isTorok == this.isTorok) // if its your own piece, can't capture
            {
                isClear = false;
                return isClear;
            }

            //if (pieceBoard[endX, endY].GetComponent<Piece>().type == "hole")
            //{
            //    isClear = false;
            //    return isClear;
            //}
        }

        // did the below for knight and rook, but bishop only moves diagonally (maybe i make this virtual and override for bishop and rook)

        if (thePiece.type == "knight") // knights can jump over pieces
        {
            //isClear = true;
            return isClear;
        }
        // below is if the piece isnt a knight


        // maybe i can make these seperate fxns that i call on from clearchecks-- WIP

        // horizontally
        int start = pieceX; // to start
        int end = endX;

        if (pieceX < endX)
        {
            start = pieceX;
            end = endX;
        }
        if (pieceX > endX)
        {
            start = endX;
            end = pieceX;
        }

        for (int i = start; i > end; i++)
        {
            if (pieceBoard[i, pieceY] != null)
            {

                if (pieceBoard[i, pieceY].GetComponent<Piece>().type != "hole")
                {
                    isClear = false;
                return isClear;
                }

            }
        }
            

        // vertically
        int startY = pieceY; // to start
        int enddY = endY;

        if (pieceY < enddY)
        {
            startY = pieceY;
            enddY = endY;
        }
        if (pieceY > enddY)
        {
            startY = endY;
            enddY = pieceY;
        }

        for (int i = startY; i > enddY; i++)
        {
            if (pieceBoard[pieceX, i] != null)
            {

                if (pieceBoard[pieceX, i].GetComponent<Piece>().type != "hole")
                {
                    isClear = false;
                    return isClear;
                }

            }
        }


        //if (thePiece.type == "rook") // rook is the only piece to not move diagonally
        //{
        //    //isClear = true;
        //    return isClear;
        //}
        //// below is if the piece isnt a rook


        //diagonally -- along the diagonal movement, blockers would be along that movement, and NOT +1+1, -1-1, +1-1, -1+1 -- WIP

        for (int i = start; i > end; i++)
        {
            for (int j = startY; j > enddY; j++)
            {

                // along movement path diagonal
                if (pieceBoard[i, j] != null)
                {
                    if (pieceBoard[i, j].GetComponent<Piece>().type != "hole")
                    {
                        isClear = false;
                        return isClear;
                    }
                }


                //if (endX > pieceX && endY > pieceY) {
                //    if (pieceBoard[i + 1, j + 1] != null)
                //    {
                //        if (pieceBoard[i + 1, j + 1].GetComponent<Piece>().type != "hole")
                //        {
                //            isClear = false;
                //            return isClear;
                //        }
                //    }
                //}

                //if (endX > pieceX && endY < pieceY) {
                //    if (pieceBoard[i + 1, j - 1] != null)
                //    {
                //        if (pieceBoard[i + 1, j - 1].GetComponent<Piece>().type != "hole")
                //        {
                //            isClear = false;
                //            return isClear;
                //        }
                //    }
                //}

                //if (endX < pieceX && endY > pieceY)
                //{
                //    if (pieceBoard[i - 1, j + 1] != null)
                //    {
                //        if (pieceBoard[i - 1, j + 1].GetComponent<Piece>().type != "hole")
                //        {
                //            isClear = false;
                //            return isClear;
                //        }
                //    }
                //}

                //if (endX < pieceX && endY < pieceY)
                //{
                //    if (pieceBoard[i - 1, j - 1] != null)
                //    {
                //        if (pieceBoard[i - 1, j - 1].GetComponent<Piece>().type != "hole")
                //        {
                //            isClear = false;
                //            return isClear;
                //        }
                //    }
                //}

            }
        }


        return isClear;
    }

    //void Update()
    //{
    //   if (isTaken)
    //    {
    //        this.gameObject.SetActive(false);
    //    } 
    //}

}
