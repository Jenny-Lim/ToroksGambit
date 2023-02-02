using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.IO;
using System.IO;
using System;
using UnityEditor.PackageManager;
using UnityEditor.UIElements;
using Unity.VisualScripting;
using UnityEngine.Analytics;

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

            reader.Close();
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
            DeleteSavedBoard(boardName);

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
        catch (Exception e) {
            Debug.Log(e.Message);
            Debug.LogError("File Error | Couldn't open file path " + Application.streamingAssetsPath + "/" + fileName);
            return;
        }

        Debug.Log("Board write completed to " + Application.streamingAssetsPath + fileName + ".");
    }

    private void DeleteSavedBoard(string name)
    {
        try {
            StreamReader reader = new StreamReader(Application.streamingAssetsPath + "/" + fileName);

            if (reader.BaseStream.Length <= 0)
            {
                //file is empty
                return;
            }


            int lineNumber = -1;
            int startLine = -1;
            int endLine = -1;
            string line = "";


            //****FIND WHAT LINES HAVE THE DATA DO BE REPLACED****
            while (line.CompareTo(name) != 0)//find where board name starts
            {
                if (reader.EndOfStream) {
                    Debug.Log("found EOF will searching for board start");
                    reader.Close();
                    return; 
                }
                lineNumber++;
                line = reader.ReadLine();
            }
            startLine = lineNumber - 1;

            while (line.CompareTo(boardEndText) != 0)//find where board name ends
            {
                if (reader.EndOfStream)
                {
                    Debug.Log("found EOF will searching for board end");
                    reader.Close();
                    return;
                }
                lineNumber++;
                line = reader.ReadLine();
            }
            endLine = lineNumber + 1;

            if (endLine < 0 || startLine < 0)// if either is not found, return
            {
                Debug.Log("did not find board start or end: results start: " + startLine + " end: " + endLine);
                reader.Close();
                return;
            }

            Debug.Log("Found start at line " + startLine + " and end at line " + endLine);
            reader.Close();


            //****CREATE A STRING THAT HAS ALL DATA BUT DATA TO BE REPALCED****
            reader = new StreamReader(Application.streamingAssetsPath + "/" + fileName);
            print("getting whole file");
            string wholeFile = reader.ReadToEnd();

            string[] fileByLine = wholeFile.Split("\n");

            string resultString = "";

            for (int i = 0; i < fileByLine.Length; i++)
            {
                if (i < startLine || i > endLine)
                {
                    resultString += fileByLine[i];
                }
            }

            reader.Close();

            //****WRITE RESULTING STRING INTO FILE****
            StreamWriter writer = new StreamWriter(Application.streamingAssetsPath + "/" + fileName);

            writer.WriteLine(resultString);

            writer.Close();

        }
        catch (Exception e) {
            Debug.Log(e.Message);
            Debug.LogError("Error opening file to delete saved board");
        }

        
    }

    
}
