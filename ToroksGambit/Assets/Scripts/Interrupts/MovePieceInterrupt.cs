using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MovePieceInterrupt", menuName = "ScriptableObjects/MovePieceInterrupt", order = 3)]
public class MovePieceInterrupt : BaseInterrupt
{
    [SerializeField] private Vector2Int moveFrom;
    [SerializeField] private Vector2Int moveTo;
    [SerializeField] private int moveAfterTurn;
    public override void Enact()
    {
        if (Board.pieceBoard[moveFrom.x, moveFrom.y] == null)//return if piece is null
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
        Board.instance.MovePieceVisual(moveFrom.x, moveFrom.y, moveTo.x, moveTo.y, Board.pieceBoard[moveFrom.x, moveFrom.y]);
        Board.instance.MovePiece(moveFrom.x, moveFrom.y, moveTo.x, moveTo.y);

        hasTriggered = true;
    }

    public override bool ShouldTrigger()
    {
        return moveAfterTurn == GameStateManager.GetTurnCount() && !hasTriggered;
    }
}
