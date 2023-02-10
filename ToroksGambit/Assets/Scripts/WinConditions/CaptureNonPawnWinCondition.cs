using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureNonPawnWinCondition : BaseCondition
{
    public override Condition IsWinCondition()
    {
        return Condition.None;
    }
}
