using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckmateWinCondition : BaseCondition
{
    public override Condition IsWinCondition()
    {
        return Condition.None;
    }
}
