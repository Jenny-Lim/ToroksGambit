using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddObsacleInterrupt : BaseInterrupt
{
    public enum ObstacleType
    {
        Wall,
        Hole
    }
    [SerializeField] private ObstacleType obstacle;
    [SerializeField] private Vector2Int placeAt;
    [SerializeField] private int placeAfterTurn = 1;

    public override void Enact()
    {
        //place down obstacle at location
        if (Board.GetPieceBoard()[placeAt.x,placeAt.y] == null)
        {
            //place obstacle
        }
    }

    public override bool ShouldTrigger()
    {
        return GameStateManager.GetTurnCount() == placeAfterTurn && !hasTriggered;
    }
}
