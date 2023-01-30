using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardAnalyzer: MonoBehaviour
{
    public static BoardAnalyzer instance;

    public float attackWeight { get; private set; } = 1.0f;
    public float defendWeight { get; private set; } = 1.0f;
    public float positionWeight { get; private set; } = 1.0f;

    private void Awake()
    {
        if (instance == null) { instance = this; }
    }

    public float Analyze(GameObject[,] board)
    {
        return MaterialCount(board);
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
                    if (targetPiece.type == "hole" || targetPiece.type == "wall") { continue; }
                    //float value = targetPiece.value;
                    //if (!targetPiece.isTorok) { value *= -1; }
                    //result += value;
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
