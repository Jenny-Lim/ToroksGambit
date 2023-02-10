using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{
    public int startX;
    public int startY;

    public int endX;
    public int endY;

    public GameObject startObject;
    public GameObject endObject;

    public bool promoted;

    public int pieceTaken = 0;// 0 is bascially null for this
    public int pieceMoving = 0;//piece type for piece MOVING

    public bool movingTorok;
    public bool takenTorok;

    public bool movingPromote;
    public bool takenPromote;

    public bool movingTough;
    public bool takenTough;

    public bool movingLastChance;
    public bool takenLastChance;

    public bool setFirstMove;
    public bool takenPieceSetFirstMove;

    public float score = 0f;
        

    public Move(int x1, int y1, int x2, int y2, int pieceIdMoving, int pieceIdTaken, bool promoteCheck, bool movingTorokCheck, bool takingTorokCheck, bool mPro, bool tPro, bool mTough, bool tTough, bool mLC, bool tLC, bool moveCheck, bool takenMoveCheck)
    {
        startX = x1;
        startY = y1;
        endX = x2;
        endY = y2;
        pieceTaken = pieceIdTaken;
        pieceMoving = pieceIdMoving;
        promoted = promoteCheck;
        movingTorok = movingTorokCheck;
        takenTorok = takingTorokCheck;

        movingPromote = mPro;
        takenPromote = tPro;

        movingTough = mTough;
        takenTough = tTough;

        movingLastChance = mLC;
        takenLastChance = tLC;

        setFirstMove = moveCheck;
        takenPieceSetFirstMove = takenMoveCheck;

        score = GetScore(pieceTaken, pieceMoving); // jenny

    }

    public Move(int x1, int y1, int x2, int y2, GameObject object1, GameObject object2)//constructor without the need for pieceTaken data
    {
        startX = x1;
        startY = y1;
        endX = x2;
        endY = y2;
        startObject = object1;
        endObject = object2;
    }

    private float GetScore(int pieceTaken, int pieceMoving)
    {
        return (pieceTaken - pieceMoving) + (pieceTaken * 0.2f); // jenny
    }

    public string DisplayMove()
    {
    return "From: (" + startX + "," + startY + ") To: (" + endX + "," + endY + ")";
    }

}
