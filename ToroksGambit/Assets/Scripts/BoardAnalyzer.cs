using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class BoardAnalyzer
{

    //***__  Negative values means better for Torok, Positive better for Player  __***

    /*
    **Base eval function** 

     materialScore = kingWt* (wK-bK)
              + queenWt* (wQ-bQ)
              + rookWt* (wR-bR)
              + knightWt* (wN-bN)
              + bishopWt* (wB-bB)
              + pawnWt* (wP-bP)

    mobilityScore = mobilityWt* (wMobility-bMobility)
    
    Eval  = (materialScore + mobilityScore) * who2Move
     */

    //piece_value_base_mid_game = [100, 290, 320, 490, 900, 60000]
    //p,n,b,r,q,k,

    //(PST -> piece square table)
    private static int[][] posPST = new int[6][];

    private static int[] kingPST =    {0, 0, 0, 0, 0, 0, 0, 0,
                                0, 0, 0, 0, 0, 0, 0, 0,
                                0, 0, 0, 0, 0, 0, 0, 0,
                                0, 0, 0, 0, 0, 0, 0, 0,
                                0, 0, 0, 0, 0, 0, 0, 0,
                                0, 0, 0, 0, 0, 0, 0, 0,
                                0, 0, 0, -5, -5, -5, 0, 0,
                                0, 0, 10, -5, -5, -5, 10, 0 };

    private static int[] queenPST =    {-20, -10, -10, -5, -5, -10, -10, -20,
                                 -10, 0, 0, 0, 0, 0, 0, -10,
                                 -10, 0, 5, 5, 5, 5, 0, -10,
                                  -5, 0, 5, 5, 5, 5, 0, -5,
                                  -5, 0, 5, 5, 5, 5, 0, -5,
                                 -10, 5, 5, 5, 5, 5, 0, -10,
                                 -10, 0, 5, 0, 0, 0, 0, -10,
                                 -20, -10, -10, 0, 0, -10, -10, -20 };

    private static int[] rookPST =    {10,  10,  10,  10,  10,  10,  10,  10,
                                10,  10,  10,  10,  10,  10,  10,  10,
                                 0,   0,   0,   0,   0,   0,   0,   0,
                                 0,   0,   0,   0,   0,   0,   0,   0,
                                 0,   0,   0,   0,   0,   0,   0,   0,
                                 0,   0,   0,   0,   0,   0,   0,   0,
                                 0,   0,   0,  10,  10,   0,   0,   0,
                                 0,   0,   0,  10,  10,   5,   0,   0 };

    private static int[] bishopPST =      {0,   0,   0,   0,   0,   0,   0,   0,
                                    0,   0,   0,   0,   0,   0,   0,   0,
                                    0,   0,   0,   0,   0,   0,   0,   0,
                                    0,  10,   0,   0,   0,   0,  10,   0,
                                    5,   0,  10,   0,   0,  10,   0,   5,
                                    0,  10,   0,  10,  10,   0,  10,   0,
                                    0,  10,   0,  10,  10,   0,  10,   0,
                                    0,   0, -10,   0,   0, -10,   0,   0 };

    private static int[] knightPST =    {-5,  -5, -5, -5, -5, -5,  -5, -5,
                                  -5,   0,  0, 10, 10,  0,   0, -5,
                                  -5,   5, 10, 10, 10, 10,   5, -5,
                                  -5,   5, 10, 15, 15, 10,   5, -5,
                                  -5,   5, 10, 15, 15, 10,   5, -5,
                                  -5,   5, 10, 10, 10, 10,   5, -5,
                                  -5,   0,  0,  5,  5,  0,   0, -5,
                                  -5, -10, -5, -5, -5, -5, -10, -5 };

    private static int[] pawnPST =       { 0,   0,   0,   0,   0,   0,   0,   0,
                                30,  30,  30,  40,  40,  30,  30,  30,
                                20,  20,  20,  30,  30,  30,  20,  20,
                                10,  10,  15,  25,  25,  15,  10,  10,
                                 5,   5,   5,  20,  20,   5,   5,   5,
                                 5,   0,   0,   5,   5,   0,   0,   5,
                                 5,   5,   5, -10, -10,   5,   5,   5,
                                 0,   0,   0,   0,   0,   0,   0,   0 };
     

    public static BoardAnalyzer instance;

    public float attackWeight { get; private set; } = 1.0f;
    public float defendWeight { get; private set; } = 1.0f;
    public float positionWeight { get; private set; } = 0.1f;
    public float mobilityWeight { get; private set; } = 0.1f;

    public float materialWeight { get; private set; } = 1.0f;

    public float toughtTraitWeight = 0.1f;
    public float promoteTraitWeight = 0.1f;
    public float lastChanceTraitWeight = 0.1f;
    public float lateGamePieceSackWeight = 0.5f;
    public BoardAnalyzer()
    {
        posPST[0] = pawnPST;
        posPST[1] = knightPST;
        posPST[2] = bishopPST;
        posPST[3] = rookPST;
        posPST[4] = queenPST;
        posPST[5] = kingPST;
        if (instance == null)
        {
            instance = this;
        }
    }

    public float Analyze(GameObject[,] board, int turnNum)
    {
        /*float result = (materialWeight * MaterialCount(board)) + //material value count
            (mobilityWeight * CalcMobility()) + //how mobile the pieces are
            (positionWeight * CalcPositioning(board) + //how good the positioning of pieces are
            (turnNum * lateGamePieceSackWeight * LateGamePieceSack(board)))// bias towards the player having less pieces as the game goes on
            ;*/
        //Debug.Log("Analyzing Board got: " + result);
        return AnalyzeNew(board, turnNum);
    }

    public float AnalyzeNew(GameObject[,] board, int turnNum)
    {
        float materialCountScore = 0;
        float positioningScore = 0;
        float lateGameSackScore = 0;

        for (int col = 0; col < board.GetLength(0)-1; col++)
        {
            for (int row = 0; row < board.GetLength(0) - 1; row++)
            {
                if (board[col, row] == null) continue;

                Piece targetPiece = board[col, row].GetComponent<Piece>();

                if (targetPiece.type == Piece.PieceType.wall || targetPiece.type == Piece.PieceType.wall) continue;

                //score material
                float pieceMatScore = targetPiece.value;
                if (targetPiece.isTorok) pieceMatScore *= -1;
                materialCountScore += pieceMatScore;

                //score position
                if (targetPiece.isTorok)
                {
                    positioningScore -= posPST[(int)targetPiece.type][(row * Board.boardSize) + col];
                }
                else
                {
                    lateGameSackScore += targetPiece.value;
                    positioningScore += posPST[(int)targetPiece.type][((Board.boardSize - row) * Board.boardSize) - (Board.boardSize - col)];//((bs - y) * bs) - (bs - x) = arrayIndex, bs - boardsize, x - xPos, y - yPos
                }


            }
        }


        return (materialWeight * materialCountScore) + //score for material amounts
            (mobilityWeight * CalcMobility()) + //scoer for piece mobility
            (positionWeight * positioningScore) + //score for piece positioning
            (lateGamePieceSackWeight * -turnNum * (Mathf.Pow(2, -0.001f * (lateGameSackScore - 7000))));//bias later in game to sack pieces so player has less
    }

    public float LateGamePieceSack(GameObject[,] board)
    {
        float sum = 0;
        for (int i = 0; i < board.GetLength(0)-1; i++)
        {
            for (int j = 0; j < board.GetLength(0) - 1; j++)
            {
                if (board[i,j] == null) { continue; }

                Piece target = board[i, j].GetComponent<Piece>();

                if (target.type > Piece.PieceType.king || target.isTorok) { continue; }

                sum += target.value;

            }
        }

        return -20*Mathf.Pow(2, -0.001f*(sum-7000));
    }

    public float CalcMobility()
    {
        return (float)(Board.instance.GetAllMoves(false).Count - Board.instance.GetAllMoves(true).Count);
    }

    public float CalcPositioning(GameObject[,] board)
    { 
        float resultsTorok = 0;
        float resultsPlayer = 0;
      
        for (int i = 0; i < Board.boardSize; i++)
        {
            for (int j = 0; j < Board.boardSize; j++)
            {
                if (board[i, j] != null)
                {
                    Piece targetPiece = board[i, j].GetComponent<Piece>();
                    if (targetPiece.type == Piece.PieceType.hole || targetPiece.type == Piece.PieceType.wall) { continue; }
                    if (targetPiece.isTorok)
                    {
                        resultsTorok += posPST[(int)targetPiece.type][(j * Board.boardSize) + i];
                    }
                    else
                    {
                        resultsPlayer += posPST[(int)targetPiece.type][((Board.boardSize - j) * Board.boardSize) - (Board.boardSize - i)];//((bs - y) * bs) - (bs - x) = arrayIndex, bs - boardsize, x - xPos, y - yPos

                    }
                }
            }
        }

        return resultsPlayer - resultsTorok;
    }

    public float MaterialCount(GameObject[,] board)
    {
        float result = 0;

        for (int i = 0; i < Board.boardSize; i++)
        {
            for (int j = 0; j < Board.boardSize; j++)
            {
                if (board[i,j] != null)
                {
                    Piece targetPiece = board[i, j].GetComponent<Piece>();
                    if (targetPiece.type == Piece.PieceType.hole || targetPiece.type == Piece.PieceType.wall) { continue; }

                    float value = targetPiece.value;

                    //add slightly more if piece has trait
                    if (targetPiece.isTough) { value += toughtTraitWeight; }
                    if (targetPiece.promote) { value += promoteTraitWeight; }
                    if (targetPiece.lastChance) { value += lastChanceTraitWeight; }

                    if (targetPiece.isTorok) { value *= -1; }

                    result += value;
                }
                
            }
        }
        return result;
    }

    public float AttacksCount(GameObject[,] board)
    {
        return 0;
    }

    public float DefendsCount(GameObject[,] board)
    {
        return 0;
    }
}
