using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BaseCondition: ScriptableObject
{
    public enum Condition
    {
        None,
        Player,
        Torok
    }

    public int playerScore = 0;
    public int torokScore = 0;
    public int scoreToWin = 3;


    public virtual Condition IsWinCondition()
    {
        return Condition.None;
    } 

    public void IncreasePlayerScore()
    {
        playerScore++;
    }

    public void IncreaseTorokScore() {
        torokScore++;
    }

    public virtual void ProgressConditionState()
    {

    }

    public bool PlayerLoseCheck()
    {
        for (int i = 0; i < Board.boardSize; i++)
        {
            for (int j = 0; j < Board.boardSize; j++)
            {
                if (Board.pieceBoard[i, j] != null)
                {
                    Piece piece = Board.pieceBoard[i, j].GetComponent<Piece>();

                    if (!piece.isTorok)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    public virtual string GetObjectiveText()
    {
        return "";
    }
}
