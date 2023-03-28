using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
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
    public int conditionType;

    //aIDAN WAS HERE & TOOK CONTROL OF THE GAME
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

    public virtual bool PlayerLoseCheck()
    {
        //this part counts player pieces but im thinking that if the player has no pieces that also means they have no moves, which will be calculated later regardless so i think this part can be skipped
        /*int totalPlayerPieces = 0;
        for (int i = 0; i < Board.boardSize; i++)
        {
            for (int j = 0; j < Board.boardSize; j++)
            {
                if (Board.pieceBoard[i, j] != null)
                {
                    Piece piece = Board.pieceBoard[i, j].GetComponent<Piece>();

                    if (!piece.isTorok)
                    {
                        totalPlayerPieces++;
                    }
                }
            }
        }
        if (totalPlayerPieces < 1)
        {
            return true;
        }*/

        if (Board.instance.GetAllMoves(false).Count < 1)
        {
            Debug.Log("is player lose got 0 avail moves");
            return true;
        }

        return false;
    }

    public virtual string GetObjectiveText()
    {
        return "";
    }
}
