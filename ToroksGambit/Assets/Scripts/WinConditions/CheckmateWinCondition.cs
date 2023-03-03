using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CheckmateWinCondition", menuName = "ScriptableObjects/WinConditions/CheckmateWinCondition", order = 3)]
public class CheckmateWinCondition : BaseCondition
{

    public override Condition IsWinCondition()
    {
        if (Board.instance.InCheckMate(true))//is torok in checkmate
        {
            return Condition.Player;
        }
        else if (Board.instance.InCheckMate(false))
        {
            return Condition.Player;
        }

        if (PlayerLoseCheck())
        {
            return Condition.Torok;
        }

        return Condition.None;
    }

    public override string GetObjectiveText()
    {
        return "Checkmate Torok's king.";
    }
}
