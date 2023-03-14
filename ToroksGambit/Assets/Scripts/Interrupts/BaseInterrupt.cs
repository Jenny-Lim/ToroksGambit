using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public abstract class BaseInterrupt : ScriptableObject
{

    public enum PieceType
    {
        Pawn,
        Knight,
        Bishop,
        Rook,
        Queen,
        Wall,
        Hole
    }

    public bool hasTriggered = false;
    public InterruptManager.InterruptTrigger triggerType;
    protected GameObject coroutineHolder;

    [Tooltip("What piece will be spawned.")]
    public PieceType piece;
    [Tooltip("The location on the board in which the piece will be placed.")]
    public Vector2Int placeAt;
    [Tooltip("On what turn will the piece be placed. Counts both player and torok turns. Turn 1 means player's first turn, Turn 2 means Torok's first turn, etc.")]
    public int afterTurn = 1;

    public abstract void Enact();

    public abstract bool ShouldTrigger();

    public void ResetHasTrigger()
    {
        hasTriggered = false;
    }
}
