using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{

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

    public List<Move> moves;

    public bool isTaken;
    public int pieceX;
    public int pieceY;
    public string type;

    void Awake()
    {
        moves = new List<Move>();
        pieceX = getX();
        pieceY = getY();
    }

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
