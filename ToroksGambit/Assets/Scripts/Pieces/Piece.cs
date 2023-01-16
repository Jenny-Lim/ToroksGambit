using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    // wont need this with undostorage -> move
    public class Move
    {
        public int newX;
        public int newY;

        public Move(int newX, int newY)
        {
            this.newX = newX;
            this.newY = newY;
        }
    }

    public List<Move> moves; // wont need this with undostorage -> move

    public bool isTaken;
    public int pieceX;
    public int pieceY;
    public string type;
    //public bool isTorok; // does this mean we need team variants of the same piece

    void Awake()
    {
        isTaken = false;

        moves = new List<Move>();

        pieceX = getX();
        //pieceX = Board.getX();
        pieceY = getY();
        //pieceY = Board.getY();
    }


    // won't need these later -- need fxn to return vals from board
    int getX()
    {
        // get current location of piece -- ask patrick
        return 0;
        // 0 for now
    }

    int getY()
    {
        // get current location of piece
        return 0;
        // 0 for now
    }
}
