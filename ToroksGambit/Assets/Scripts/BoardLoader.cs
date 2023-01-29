using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.IO;
using System.IO;
using System;
using UnityEditor.PackageManager;
using UnityEditor.UIElements;
using Unity.VisualScripting;

public class BoardLoader : MonoBehaviour
{
    private const string boardStartText = "boardstart";
    private const string boardEndText = "boardend";
    private string fileName = "StartingBoards.txt";
    public string boardName = "";

    /* this class stores boards in a text file for later use
     * it stores them in the following configuration
     * 
     * boardstart
     * "boardName"
     * piece information per line (piece type, pieceX, pieceY, who owns piece, abilities)
     * piece information per line (piece type, pieceX, pieceY, who owns piece, abilities)
     * piece information per line (piece type, pieceX, pieceY, who owns piece, abilities)
     * boardend
     * boardstart 
     * "boardName"
     * piece information per line (piece type, pieceX, pieceY, who owns piece, abilities)
     * piece information per line (piece type, pieceX, pieceY, who owns piece, abilities)
     * piece information per line (piece type, pieceX, pieceY, who owns piece, abilities)
     * piece information per line (piece type, pieceX, pieceY, who owns piece, abilities)
     * boardend
     * etc...
     */

    public void LoadBoard(string boardName)
    {
        Board.instance.ClearBoard();
    }

    public void WriteCurrentBoard(string givenName)
    {
        Debug.Log("Attempting board write.");
        try
        {
            StreamWriter writer = new StreamWriter(Application.streamingAssetsPath + "/" + fileName, append: true);

            writer.WriteLine(boardStartText);
            writer.WriteLine(givenName);

            for (int i = 0; i < Board.boardSize; i++)
            {
                for (int j = 0; j < Board.boardSize; j++)
                {
                    if (Board.pieceBoard[i, j] == null) { continue; }

                    Piece thisPiece = Board.pieceBoard[i, j].GetComponent<Piece>();

                    string pieceString = "";

                    if (thisPiece.type == "pawn") { pieceString = "" + 0; }
                    else if (thisPiece.type == "knight") { pieceString = "" + 1; }
                    else if (thisPiece.type == "bishop") { pieceString = "" + 2; }
                    else if (thisPiece.type == "rook") { pieceString = "" + 3; }
                    else if (thisPiece.type == "queen") { pieceString = "" + 4; }
                    else if (thisPiece.type == "king") { pieceString = "" + 5; }
                    else if (thisPiece.type == "wall") { pieceString = "" + 6; }
                    else if (thisPiece.type == "hole") { pieceString = "" + 7; }


                    pieceString += "," + thisPiece.pieceX + "," + thisPiece.pieceY + "," + thisPiece.isTorok;
                    writer.WriteLine(pieceString);
                }
            }
            writer.WriteLine(boardEndText);

            writer.Close();
        }
        catch (Exception) {
            Debug.LogError("File Error | Couldn't open file path " + Application.streamingAssetsPath + "/" + fileName);
        }

        Debug.Log("Board write completed to " + Application.streamingAssetsPath + fileName + ".");
    }
}
