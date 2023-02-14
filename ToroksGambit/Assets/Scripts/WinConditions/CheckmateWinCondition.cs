using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
