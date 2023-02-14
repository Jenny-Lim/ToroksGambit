using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

[CreateAssetMenu(fileName = "AddPieceInterrupt", menuName = "ScriptableObjects/Interrupts/AddPieceInterrupt", order = 1)]
public class AddPieceInterrupt : BaseInterrupt
{
    public override void Enact()
    {
        if (Board.GetPieceBoard()[placeAt.x,placeAt.y] == null)
        {
            Board.instance.PlacePieceTorok(placeAt.x, placeAt.y, (int)piece);
            hasTriggered = true;
        }
        else
        {
            Debug.Log("InterruptError| Didn't place Piece at " + placeAt + "because space was taken.");
        }

    }

    public override bool ShouldTrigger()
    {
        return GameStateManager.GetTurnCount() == afterTurn && !hasTriggered;
    }
}
