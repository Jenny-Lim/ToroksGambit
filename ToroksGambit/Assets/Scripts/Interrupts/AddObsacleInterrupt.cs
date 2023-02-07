using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AddObstacleInterrupt", menuName = "ScriptableObjects/AddObstacleInterrupt", order = 2)]
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
        if (Board.GetPieceBoard()[placeAt.x,placeAt.y] == null)//place obstacle only if no piece is in the placing location (can be changed to places over player pieces but idk if we want that)
        {
            Board.instance.PlaceObstacle(placeAt.x, placeAt.y, (int)obstacle);
        }
    }

    public override bool ShouldTrigger()
    {
        return GameStateManager.GetTurnCount() == placeAfterTurn && !hasTriggered;
    }
}
