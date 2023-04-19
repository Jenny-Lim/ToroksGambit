using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MovePieceInterrupt", menuName = "ScriptableObjects/Interrupts/MovePieceInterrupt", order = 3)]
public class MovePieceInterrupt : BaseInterrupt
{
    public Vector2Int moveFrom;
    public Vector2Int moveTo;
    public override void Enact()
    {

        if (!Piece.InBoundsCheck(moveFrom.x, moveFrom.y) || !Piece.InBoundsCheck(moveTo.x, moveTo.y))
        {
            Debug.LogError("InterruptError| Move location " + moveFrom + " or " + moveTo + " did not point to a valid board location");
            return;
        }
        if (Board.pieceBoard[moveFrom.x, moveFrom.y] == null)
        {
            Debug.LogError("InterruptError| Piece location " + moveFrom + " did not point to a piece");
            return;
        }

        if (Board.pieceBoard[moveTo.x, moveTo.y] != null)
        {
            Piece piece = Board.pieceBoard[moveTo.x, moveTo.y].GetComponent<Piece>();
            if (piece.type == Piece.PieceType.wall)
            {
                return;
            }
        }

        CameraHeadMovements.canScroll = false;
        MainMenu.instance.pauseFxn.enabled = false;
        if (coroutineHolder != null) { Destroy(coroutineHolder); }
        coroutineHolder =  InterruptManager.instance.CreateCoRoHolder();
        InterruptCoroutineHolder holder = coroutineHolder.AddComponent<InterruptCoroutineHolder>();
        holder.Initialize(this);
        holder.RunCoroutine(InterruptCoroutineHolder.Coroutines.MovePiece);


        /*if (Board.pieceBoard[moveFrom.x, moveFrom.y] == null)//return if piece is null
        {
            Debug.Log("InterruptError| Did not move piece: No piece at " + moveFrom);
            return;
        }

        if (Board.pieceBoard[moveTo.x, moveTo.y] != null) {

            Piece toPiece = Board.pieceBoard[moveTo.x, moveTo.y].GetComponent<Piece>();

            if (toPiece.type == Piece.PieceType.wall || toPiece.type == Piece.PieceType.hole)//return if end location is an obstacle (can change this if want to replace walls or something like that)
            {
                Debug.Log("InterruptError| Did not move piece: Location to move to was blocked by obstacle");
                return;
            }
            else if (toPiece.isTorok)//return if ending position is a torok piece, (i assume we dont want him to kill his own guys but can change this if we want)
            {
                Debug.Log("InterruptError| Did not move piece: final position was Torok piece");
                return;
            }
        }

        //move piece
        //Board.instance.MovePieceVisual(moveFrom.x, moveFrom.y, moveTo.x, moveTo.y, Board.pieceBoard[moveFrom.x, moveFrom.y], false);
        //Board.instance.MovePieceVisualTeleport(moveFrom.x, moveFrom.y, moveTo.x, moveTo.y);
        //Board.instance.MovePiece(moveFrom.x, moveFrom.y, moveTo.x, moveTo.y);

        //hasTriggered = true;*/
    }

    public override bool ShouldTrigger()
    {
        return afterTurn == GameStateManager.GetTurnCount() && !hasTriggered;
    }
}
