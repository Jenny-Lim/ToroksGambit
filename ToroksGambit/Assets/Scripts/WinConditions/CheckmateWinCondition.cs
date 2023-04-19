using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CheckmateWinCondition", menuName = "ScriptableObjects/WinConditions/CheckmateWinCondition", order = 3)]
public class CheckmateWinCondition : BaseCondition
{

    private void Awake()
    {
        conditionType = 2;
    }
    public override Condition IsWinCondition()
    {
        if (Board.instance.IsKingInCheck(true) && Board.instance.GetAllMoves(true).Count < 1 && !GameStateManager.instance.GetIsPlayersTurn())//is torok in checkmate
        {
            return Condition.Player;
        }
        else if (Board.instance.IsKingInCheck(false) && Board.instance.GetAllMoves(false).Count < 1 && GameStateManager.instance.GetIsPlayersTurn())
        {
            return Condition.Torok;
        }

        if (PlayerLoseCheck())
        {
            return Condition.Torok;
        }

        return Condition.None;
    }

    public override bool PlayerLoseCheck()
    {
        bool hasMoves = base.PlayerLoseCheck();
        if (!hasMoves && Board.instance.IsKingInCheck(false))
        {
            return true;
        }

        return false;
    }

    public override string GetObjectiveText()
    {
        return "Checkmate Torok's king.";
    }
}
