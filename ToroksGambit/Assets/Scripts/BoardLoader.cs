using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;


public class BoardLoader : MonoBehaviour
{
    private const string boardStartText = "boardstart";
    private const string boardEndText = "boardend";
    private string fileName = "StartingBoards.txt";
    public string boardName = "";
    public List<string> savedBoardNames;
    public static BoardLoader instance;

    /* this class stores boards in a text file for later use
     * it stores them in the following configuration
     * 
     * boardstart
     * "boardName"
     * Win Condition **
     * Active Interrupts **
     * piece information per line (piece type, pieceX, pieceY, who owns piece, abilities)
     * piece information per line (piece type, pieceX, pieceY, who owns piece, abilities)
     * piece information per line (piece type, pieceX, pieceY, who owns piece, abilities)
     * boardend
     * boardstart 
     * "boardName"
     * Win Condition **
     * Active Interrupts **
     * piece information per line (piece type, pieceX, pieceY, who owns piece, abilities)
     * piece information per line (piece type, pieceX, pieceY, who owns piece, abilities)
     * piece information per line (piece type, pieceX, pieceY, who owns piece, abilities)
     * piece information per line (piece type, pieceX, pieceY, who owns piece, abilities)
     * boardend
     * etc...
     */

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        savedBoardNames = GetAllSavedBoardNames();
    }

    public void LoadBoard(string boardName)
    {
        if (!File.Exists(Application.streamingAssetsPath + "/" + fileName))
        {
            Debug.LogError("No file found at location " + Application.streamingAssetsPath + "/" + fileName);
            return;
        }

        Debug.Log("Attempt loading board with name " + boardName);

        Board.instance.ClearBoard();
        FileStream fs = null;
        StreamReader reader = null;
        try
        {
            fs = new FileStream(Application.streamingAssetsPath + "/" + fileName, FileMode.Open, FileAccess.Read);
            reader = new StreamReader(fs);
            string line = "";

            //Debug.Log("find start");
            while (line.CompareTo(boardName) != 0)
            {
                line = reader.ReadLine();
                print(line);
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

            //Debug.Log("Load to end");
            while (line.CompareTo(boardEndText) != 0)
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
            //Debug.Log("reached end");
        }
        catch (Exception e)
        { 
            Debug.Log(e.Message);
            Debug.LogError("File Error | Couldn't open file path " + Application.streamingAssetsPath + "/" + fileName);
            return;
        }
        finally
        {
            if (fs != null) { fs.Close(); }
            if (reader != null) { reader.Close(); }
        }

        Debug.Log("Finished loading board with name " + boardName);

    }

    public void WriteCurrentBoard(string givenName)
    {
        Debug.Log("Attempting board write.");
        DeleteSavedBoard(boardName);
        StreamWriter writer = null;
        FileStream fs = null;
        try
        {
            if (File.Exists(Application.streamingAssetsPath + "/" + fileName))
            {
                fs = new FileStream(Application.streamingAssetsPath + "/" + fileName, FileMode.Append, FileAccess.Write);
            }
            else
            {
                fs = new FileStream(Application.streamingAssetsPath + "/" + fileName, FileMode.OpenOrCreate, FileAccess.Write);
            }
            
            writer = new StreamWriter(fs);

            writer.WriteLine(boardStartText);
            writer.WriteLine(givenName);

            //write victory condition
            BaseCondition winCondition = GameStateManager.instance.winCondition;
            string winConditionString = "WinCondition";
            //0 -> captureNonPawn, 1 -> CaptureTheFlag, 2 -> Checkmate, 3 -> KingOfTheHill
            if (winCondition.GetType() == typeof(CaptureNonPawnWinCondition))
            {
                winConditionString += "," + 0;
            }
            else if (winCondition.GetType() == typeof(CaptureTheFlagWinCondition))
            {
                winConditionString += "," + 1;
                CaptureTheFlagWinCondition capFlagCon = (CaptureTheFlagWinCondition)winCondition;
                foreach (Vector2Int location in capFlagCon.locations)
                {
                    winConditionString += "," + location.x + "," + location.y;
                }
            }
            else if (winCondition.GetType() == typeof(CheckmateWinCondition))
            {
                winConditionString += "," + 2;
            }
            else if (winCondition.GetType() == typeof(KingOfTheHillWinCondition))
            {
                winConditionString += "," + 3;
                KingOfTheHillWinCondition capFlagCon = (KingOfTheHillWinCondition)winCondition;
                foreach (Vector2Int location in capFlagCon.locations)
                {
                    winConditionString += "," + location.x + "," + location.y;
                }
            }
            writer.WriteLine(winConditionString);

            //write active interrupts
            foreach (BaseInterrupt interrupt in InterruptManager.instance.GetActiveInterrupts())
            {
                //interrupt type, specific interrupt type data, Trigger Type ,afterTurn

                // 0 -> AddPiece, 1 -> MovePiece
                string printingString = "Interrupt";
                //interrupt type
                if (interrupt.GetType() == typeof(AddPieceInterrupt)) {

                    printingString += "," + 0;//interrupt type

                    printingString += "," + (int)interrupt.piece;//piece to place

                    printingString += "," + interrupt.placeAt.x + "," + interrupt.placeAt.y;//where
                }
                else if (interrupt.GetType() == typeof(MovePieceInterrupt)){
                    printingString += "," + 1;//interrupt type

                    MovePieceInterrupt moveP = (MovePieceInterrupt)interrupt;//cast

                    printingString += "," + moveP.moveFrom.x + ", " + moveP.moveFrom.y + "," + moveP.moveTo.x + ", " + moveP.moveTo.y;//location to move from and to

                }

                printingString += "," + (int)interrupt.triggerType;//trigger type

                printingString += "," + interrupt.afterTurn;//when

                writer.WriteLine(printingString);//write line to file
            }


            //write pieces
            for (int i = 0; i < Board.boardSize; i++)
            {
                for (int j = 0; j < Board.boardSize; j++)
                {
                    if (Board.pieceBoard[i, j] == null) { continue; }

                    Piece thisPiece = Board.pieceBoard[i, j].GetComponent<Piece>();

                    string pieceString = "Piece";

                    if (thisPiece.type == Piece.PieceType.pawn) { pieceString += "," + 0; }
                    else if (thisPiece.type == Piece.PieceType.knight) { pieceString += "," + 1; }
                    else if (thisPiece.type == Piece.PieceType.bishop) { pieceString += "," + 2; }
                    else if (thisPiece.type == Piece.PieceType.rook) { pieceString += "," + 3; }
                    else if (thisPiece.type == Piece.PieceType.queen) { pieceString += "," + 4; }
                    else if (thisPiece.type == Piece.PieceType.king) { pieceString += "," + 5; }
                    else if (thisPiece.type == Piece.PieceType.wall) { pieceString += "," + 6; }
                    else if (thisPiece.type == Piece.PieceType.hole) { pieceString += "," + 7; }


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
        finally
        {
            if (fs != null) { fs.Dispose(); }
            if (writer != null) { writer.Close();}
        }

        Debug.Log("Board write completed to " + Application.streamingAssetsPath + fileName + ".");
    }

    public void DeleteSavedBoard(string name)
    {
        StreamReader reader = null;
        StreamWriter writer = null;
        FileStream fs = null;
        try {
            fs = new FileStream(Application.streamingAssetsPath + "/" + fileName, FileMode.Open, FileAccess.ReadWrite);
            reader = new StreamReader(fs);

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
                    Debug.Log("found EOF will searching for board name");
                    reader.Close();
                    return; 
                }
                lineNumber++;
                line = reader.ReadLine();
            }
            //print("found at " + lineNumber);
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
            endLine = lineNumber;//+1

            if (endLine < 0 || startLine < 0)// if either is not found, return
            {
                Debug.Log("did not find board start or end: results start: " + startLine + " end: " + endLine);
                reader.Close();
                return;
            }

            //Debug.Log("Found start at line " + startLine + " and end at line " + endLine);
            reader.Close();


            //****CREATE A STRING THAT HAS ALL DATA BUT DATA TO BE REPALCED****
            reader = new StreamReader(Application.streamingAssetsPath + "/" + fileName);
            //print("getting whole file");
            string wholeFile = reader.ReadToEnd();

            string[] fileByLine = wholeFile.Split("\n");

            /*for (int i = 0; i < fileByLine.Length; i++)
            {
                if (fileByLine[i].CompareTo("\n") == 0)
                {
                    print("newline at " + i);
                }
            }*/

            string resultString = "";

            for (int i = 0; i < fileByLine.Length; i++)
            {
                if (fileByLine[i].CompareTo("\n") == 0) { continue;  }

                if (i < startLine || i > endLine)
                {
                    resultString += fileByLine[i];
                }
            }

            reader.Close();

            //****WRITE RESULTING STRING INTO FILE****
            writer = new StreamWriter(Application.streamingAssetsPath + "/" + fileName);

            writer.Write(resultString);


        }
        catch (Exception e) {

            Debug.Log(e.Message);
            Debug.LogError("Error opening file to delete saved board");
            
        }
        finally {
            if (writer != null) { writer.Close(); }
            if (reader != null) { reader.Close(); }
        }
        

    }

    public List<string> GetAllSavedBoardNames()
    {
        List<string> returnList = new List<string>();
        if (!File.Exists(Application.streamingAssetsPath + "/" + fileName))
        {
            return new List<string>();
        }
        try
        {
            StreamReader reader = new StreamReader(Application.streamingAssetsPath + "/" + fileName);

            string line = "";

            while (!reader.EndOfStream)
            {
                line = reader.ReadLine();
                if (line.CompareTo(boardStartText) == 0)
                {
                    returnList.Add(reader.ReadLine());
                }
            }
            reader.Close();

        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
            Debug.LogError("Could not open file " + Application.streamingAssetsPath + "/" + fileName);
        }

        return returnList;
    }

    public string GetBoardName()
    {
        return boardName;
    }
}
