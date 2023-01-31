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
        Debug.Log("Attempt loading board with name " + boardName);

        Board.instance.ClearBoard();

        try
        {
            StreamReader reader = new StreamReader(Application.streamingAssetsPath + "/" + fileName);
            string line = "";

            while (line != boardName)
            {
                line = reader.ReadLine();
                if (reader.EndOfStream)
                {
                    Debug.LogError("Did not find saved board with given name " + boardName);
                    return;
                }
            }

            if (reader.EndOfStream && line.CompareTo(boardName) != 0)// if at the end of the file meaning no name was found
            {
                Debug.LogError("File Error| Could not find board " + boardName + " inside file " + Application.streamingAssetsPath + "/" + fileName);
            }

            line = reader.ReadLine();

            while (line != boardEndText)
            {
                string[] lines = line.Split(char.Parse(","));// will seperate line into mutliple strings, [0] = pieceId, [1] = xPos, [2] = yPos, [3] = isTorok
                if (Convert.ToBoolean(lines[3]))//if is torok piece
                {
                    Board.instance.PlacePieceTorok(int.Parse(lines[1]), int.Parse(lines[2]), int.Parse(lines[0]));
                }
                else//else player piece
                {
                    Board.instance.PlacePiece(int.Parse(lines[1]), int.Parse(lines[2]), int.Parse(lines[0]));
                }
                line = reader.ReadLine();
            }


        }
        catch (Exception)
        {
            Debug.LogError("File Error | Couldn't open file path " + Application.streamingAssetsPath + "/" + fileName);
            return;
        }

        Debug.Log("Finished loading board with name " + boardName);

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

                    if (thisPiece.type == Piece.PieceType.pawn) { pieceString = "" + 0; }
                    else if (thisPiece.type == Piece.PieceType.knight) { pieceString = "" + 1; }
                    else if (thisPiece.type == Piece.PieceType.bishop) { pieceString = "" + 2; }
                    else if (thisPiece.type == Piece.PieceType.rook) { pieceString = "" + 3; }
                    else if (thisPiece.type == Piece.PieceType.queen) { pieceString = "" + 4; }
                    else if (thisPiece.type == Piece.PieceType.king) { pieceString = "" + 5; }
                    else if (thisPiece.type == Piece.PieceType.wall) { pieceString = "" + 6; }
                    else if (thisPiece.type == Piece.PieceType.hole) { pieceString = "" + 7; }


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
