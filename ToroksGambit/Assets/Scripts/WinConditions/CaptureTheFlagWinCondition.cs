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
        if (PlayerLoseCheck())
        {
            return Condition.Torok;
        }

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

        return Condition.Player;
    }

    public override bool PlayerLoseCheck()
    {
        int playerPieces = 0;
        int playerPiecesOnObjective = 0;

        //count player pieces on objective spots
        foreach (Vector2Int pos in locations)
        {
            if (Board.pieceBoard[pos.x,pos.y] == null) { continue; }//no piece get keep going

            Piece targetPiece = Board.pieceBoard[pos.x, pos.y].GetComponent<Piece>();

            if (targetPiece.isTorok) { continue; }// if torok piece keep going

            if ((int)targetPiece.type > (int)Piece.PieceType.king) { continue; }//obstacle, keep going

            playerPiecesOnObjective++;

        }

        //count total player pieces
        for (int i = 0; i < Board.boardSize; i++)
        {
            for (int j = 0; j < Board.boardSize; j++)
            {
                if (Board.pieceBoard[i,j] == null) { continue; }//no piece get keep going

                Piece targetPiece = Board.pieceBoard[i, j].GetComponent<Piece>();

                if (targetPiece.isTorok) { continue; }// if torok piece keep going

                if ((int)targetPiece.type > (int)Piece.PieceType.king) { continue; }//obstacle, keep going

                playerPieces++;

            }
        }

        int playerPiecesNotOnObjective = playerPieces - playerPiecesOnObjective;
        int requiredNumPiecesToWin = locations.Count - playerPiecesOnObjective;

        if (playerPiecesNotOnObjective < requiredNumPiecesToWin) {
            return true;
        }

        return false;
    }

    public override void ProgressConditionState()
    {
        //turns pieces invincibleif in the designated spots
        foreach (Vector2Int location in locations)
        {
            if (Board.pieceBoard[location.x, location.y] != null)
            {
                Piece piece = Board.pieceBoard[location.x, location.y].GetComponent<Piece>();

                if (piece.isTorok) continue; 
                //turn that piece invuln
                piece.isInvulnerable = true;
                //probably apply some type of effect to symbolize invuln
            }
        }
    }

    public override string GetObjectiveText()
    {
        return "Land on the marked safe spaces.";
    }
}
