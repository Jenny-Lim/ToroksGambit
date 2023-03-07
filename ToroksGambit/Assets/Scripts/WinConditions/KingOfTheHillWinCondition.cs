using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KingOfTheHillWinCondition", menuName = "ScriptableObjects/WinConditions/KingOfTheHillWinCondition", order = 3)]
public class KingOfTheHillWinCondition : BaseCondition
{
    public List<Vector2Int> locations = new List<Vector2Int>();

    public override Condition IsWinCondition()
    {
        if (playerScore >= scoreToWin)
        {
            return Condition.Player;
        }
        else if (torokScore >= scoreToWin) {
            return Condition.Torok;
        }
        else
        {
            if (PlayerLoseCheck())
            {
                return Condition.Torok;
            }

            return Condition.None;
        }
    }

    public override void ProgressConditionState()
    {
        if (GameStateManager.instance.GetIsPlayersTurn())//if players turn
        {
            foreach (Vector2Int location in locations)
            {
                if (Board.pieceBoard[location.x, location.y] != null)
                {
                    Piece targetLocationPiece = Board.pieceBoard[location.x, location.y].GetComponent<Piece>();

                    if (!targetLocationPiece.isTorok)
                    {
                        IncreasePlayerScore();
                    }
                }
            }
        }
        else//toroks turn
        {
            foreach (Vector2Int location in locations)
            {
                if (Board.pieceBoard[location.x, location.y] != null)
                {
                    Piece targetLocationPiece = Board.pieceBoard[location.x, location.y].GetComponent<Piece>();

                    if (targetLocationPiece.isTorok)
                    {
                        IncreasePlayerScore();
                    }
                }
            }
        }
        
        
    }

    public override string GetObjectiveText()
    {
        return "Have a piece on the marked tile(s) for " + playerScore + "/" + scoreToWin + " turns.";
    }
}
