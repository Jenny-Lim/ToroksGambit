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

    public int pieceTaken = 0;// 0 is bascially null for this

    public bool promoted;
        

    public Move(int x1, int y1, int x2, int y2, GameObject object1, GameObject object2, int pieceIdTaken, bool promoteCheck)
    {
        startX = x1;
        startY = y1;
        endX = x2;
        endY = y2;
        startObject = object1;
        endObject = object2;
        pieceTaken = pieceIdTaken;
        promoted = promoteCheck;
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


    public string DisplayMove()
    {
    return "From: (" + startX + "," + startY + ") To: (" + endX + "," + endY + ")";
    }

}
