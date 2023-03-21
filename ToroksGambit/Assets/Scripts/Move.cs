using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{
    public int startX;
    public int startY;

    public int endX;
    public int endY;

    //public GameObject startObject;
    //public GameObject endObject;

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
    public int capturedPiece = -1; // this is null
    public int capturingPiece = -1;

    public bool pawnPromote;
        

    public Move(int x1, int y1, int x2, int y2, int pieceIdMoving, int pieceIdTaken, bool promoteCheck, bool movingTorokCheck, bool takingTorokCheck, bool mPro, bool tPro, bool mTough, bool tTough, bool mLC, bool tLC, bool moveCheck, bool takenMoveCheck, bool pawnPromoted)
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

        pawnPromote = pawnPromoted;

    }

    public Move(int x1, int y1, int x2, int y2, GameObject object1, GameObject object2, int capturedPiece, int capturingPiece)//constructor without the need for pieceTaken data
    {
        startX = x1;
        startY = y1;
        endX = x2;
        endY = y2;
        //startObject = object1;
        //endObject = object2;
        this.capturedPiece = capturedPiece;
        this.capturingPiece = capturingPiece;

        if (capturedPiece != -1)
        {
            score = GetScore(capturedPiece, capturingPiece);
        }
    }

    //void Awake()
    //{
    //    if (capturedPiece != -1)
    //    {
    //        score = GetScore(capturedPiece, capturingPiece);
    //    }
    //}

    private float GetScore(int capturedPiece, int capturingPiece)
    {
        return (capturedPiece - capturingPiece) + (capturedPiece * 0.25f);
    }

    public string DisplayMove()
    {
    return "From: (" + startX + "," + startY + ") To: (" + endX + "," + endY + ")";
    }

}
