using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingOfTheHillWinCondition : BaseCondition
{
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
            return Condition.None;
        }
    }
}
