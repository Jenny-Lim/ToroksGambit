using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AddPieceInterrupt", menuName = "ScriptableObjects/AddPieceInterrupt", order = 1)]
public class AddPieceInterrupt : BaseInterrupt
{
    public enum PieceType
    {
        Pawn,
        Knight,
        Bishop,
        Rook,
        Queen
    }

    [Tooltip("What piece will be spawned.")]
    public PieceType piece;
    [Tooltip("The location on the board in which the piece will be placed.")]
    public Vector2Int placeAt;
    [Tooltip("On what turn will the piece be placed. Counts both player and torok turns. Turn 1 means player's first turn, Turn 2 means Torok's first turn, etc.")]
    public int placeAfterTurn = 1;

    public override void Enact()
    {
        Board.instance.PlacePiece((int)piece, placeAt.x, placeAt.y);
        hasTriggered = true;
    }

    public override bool ShouldTrigger()
    {
        return GameStateManager.GetTurnCount() == placeAfterTurn && !hasTriggered;
    }
}
