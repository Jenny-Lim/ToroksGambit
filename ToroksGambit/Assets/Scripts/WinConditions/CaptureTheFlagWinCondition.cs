using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureTheFlagWinCondition : BaseCondition
{
    private Piece[] conditionPieces;//goal pieces
    public override Condition IsWinCondition()
    {
        return Condition.None;
    }
}
