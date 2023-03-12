using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "CaptureNonPawnWinCondition", menuName = "ScriptableObjects/WinConditions/CaptureNonPawnWinConditiont", order = 3)]
public class CaptureNonPawnWinCondition : BaseCondition
{
    private void Awake()
    {
        conditionType = 0;
    }
    public override Condition IsWinCondition()
    {
        if (PlayerLoseCheck())//check if player has no pieces
        {
            return Condition.Torok;
        }

        bool foundTorokPiece = false;
        bool foundPlayerPiece = false;
        for (int i = 0; i < Board.boardSize; i++)//loop x pos
        {
            for (int j = 0; j < Board.boardSize; j++)//loop y pos
            {

                if (Board.pieceBoard[i, j] == null)//null guard
                {
                    continue;
                }

                Piece targetPiece = Board.pieceBoard[i, j].GetComponent<Piece>();//get piece

                if ((int)targetPiece.type < 6 && (int)targetPiece.type > 0)//is not an obstacle or pawn
                {
                    if (targetPiece.isTorok)
                    {
                        foundTorokPiece = true;
                    }
                    else
                    {
                        foundPlayerPiece = true;
                    }
                }
                
            }
        }

        if (foundPlayerPiece && foundTorokPiece)
            return Condition.None;
        else if (foundPlayerPiece && !foundTorokPiece)
            return Condition.Player;
        else if (!foundPlayerPiece && foundTorokPiece)
            return Condition.Torok;

        return Condition.None;
    }

    public override string GetObjectiveText()
    {
        return "Take all non pawn pieces.";
    }
}
