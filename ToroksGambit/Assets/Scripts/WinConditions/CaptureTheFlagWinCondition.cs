using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CaptureTheFlagWinCondition", menuName = "ScriptableObjects/WinConditions/CaptureTheFlagWinCondition", order = 3)]
public class CaptureTheFlagWinCondition : BaseCondition
{
    public List<Vector2Int> locations = new List<Vector2Int>();

    private void Awake()
    {
        conditionType = 1;
    }
    public override Condition IsWinCondition()
    {
        foreach (Vector2Int location in locations)
        {
            if (Board.pieceBoard[location.x, location.y] == null)
            {
                return Condition.None;
            }

            Piece piece = Board.pieceBoard[location.x, location.y].GetComponent<Piece>();
            if (piece.isTorok)
            {
                return Condition.None;
            }
        }

        if (PlayerLoseCheck())
        {
            return Condition.Torok;
        }

        return Condition.Player;
    }

    public override void ProgressConditionState()
    {
        //turns pieces invincibleif in the designated spots
        foreach (Vector2Int location in locations)
        {
            if (Board.pieceBoard[location.x, location.y] != null)
            {
                Piece piece = Board.pieceBoard[location.x, location.y].GetComponent<Piece>();

                //turn that piece invuln
                piece.isInvulnerable= true;
                //probably apply some type of effect to symbolize invuln
            }
        }
    }

    public override string GetObjectiveText()
    {
        return "Land on the marked safe spaces.";
    }
}
