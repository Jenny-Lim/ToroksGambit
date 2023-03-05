using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Jobs;

public class MinMax : MonoBehaviour
{
    public static MinMax instance;//static instance of this
    private BoardAnalyzer analyzer = new BoardAnalyzer();
    [SerializeField] public int maxDepth = 1;
    MoveComparer mc = new MoveComparer();

    //int numOfUndoCalled = 0;
    //int numOfMovesCalled = 0;

    int totalNumNodesLookedAt = 0;
/*
    private class ScoredMove
    {
        public Move move;
        public float score;

        public ScoredMove(Move newMove, float newScore)
        {
            move = newMove;
            score = newScore;
        }

    }
*/
    public enum playerToMove
    {
        player, torok
    }

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }


    //the recursive wrapper for the minmax call
    public Move GetMinMaxMove(playerToMove toMove)
    {
        totalNumNodesLookedAt = 0;
        float startTime = Time.realtimeSinceStartup;
        //Debug.Log("AI: Looking for move...");
        ScoredMove resultMove = MinMaxRecursive(maxDepth, toMove, float.MinValue, float.MaxValue);
        //print(resultMove.move == null);
        if (resultMove.move != null)
        {
            //Debug.Log("AI: Move found. " + resultMove.move.DisplayMove());
        }
        //print("Moves " + numOfMovesCalled);
        //print("Undos " + numOfUndoCalled);
        //print("Total Number of Nodes Searched: " + totalNumNodesLookedAt);
        //print("Time taken: " + (Time.realtimeSinceStartup - startTime));
        //print("Move history list count " + Board.instance.moveList.Count);
        return resultMove.move;
    }

    //the recursive functionality of the minmax call
    private ScoredMove MinMaxRecursive(int depth, playerToMove whosMoving, float alpha, float beta)
    {

        //recursive termination
        if (depth == 0)
        {
            //print("Move List count: " + Board.instance.moveList.Count);
            totalNumNodesLookedAt++;
            return new ScoredMove(null, analyzer.Analyze(Board.pieceBoard));
        }

        ScoredMove bestMove;//holder for the best/most likely move to make

        if (whosMoving == playerToMove.player)//max
        {

            List<Move> allAvailableMoves = Board.instance.GetAllMoves(false);//get list of all possible moves

            //check if allavailableMoves has no moves
            if (allAvailableMoves.Count < 1)
            {
                return new ScoredMove(null, float.NegativeInfinity);
            }

            allAvailableMoves.Sort(mc); // jenny

            bestMove = new ScoredMove(allAvailableMoves[0], float.NegativeInfinity);//set best move score to be as low as possible);
            //print("Amount of moves player moves available: " + allAvailableMoves.Count);

            foreach (Move move in allAvailableMoves)
            {
                Board.instance.MovePiece(move.startX, move.startY, move.endX, move.endY);//move piece
                ScoredMove recursiveResult = MinMaxRecursive(depth - 1, playerToMove.torok, alpha, beta);//recursive call
                Board.instance.UndoMove();//undo previous move

                if (recursiveResult.score > bestMove.score)//if subtree result is better make best move equal to that
                {
                    bestMove.move = move;
                    bestMove.score = recursiveResult.score;
                }

                alpha = Mathf.Max(alpha, bestMove.score);//update alpha value if needed

                if (alpha >= beta)
                {
                    //print("broke in max");
                    break;
                }
            }
        }
        else//min
        {
            List<Move> allAvailableMoves = Board.instance.GetAllMoves(true);// get list of all possible moves

            //check if allavailableMoves has no moves
            if (allAvailableMoves.Count < 1)
            {
                return new ScoredMove(null, float.PositiveInfinity);
            }

            allAvailableMoves.Sort(mc); // jenny

            bestMove = new ScoredMove(allAvailableMoves[0], float.PositiveInfinity);
            //print("Amount of moves torok moves available: " + allAvailableMoves.Count);
            foreach (Move move in allAvailableMoves)
            {
                Board.instance.MovePiece(move.startX, move.startY, move.endX, move.endY);//move piece
                ScoredMove recursiveResult = MinMaxRecursive(depth - 1, playerToMove.player, alpha, beta);//recursive call
                Board.instance.UndoMove();//undo previous move

                if (recursiveResult.score < bestMove.score)// if subtree is better make best move equal to that
                {
                    bestMove.move = move;
                    bestMove.score = recursiveResult.score;
                }

                beta = Mathf.Min(beta, bestMove.score);//update beta if needed

                if (alpha >= beta)
                {
                    //print("broke in main");
                    break;
                }

            }
        }


        //return resulting move
        return bestMove;
    }

    public void SetNewDepth(int newDepth)
    {
        maxDepth = newDepth;
    }
}

public struct ScoredMove
{
    public Move move;
    public float score;

    public ScoredMove(Move newMove, float newScore)
    {
        move = newMove;
        score = newScore;
    }

}

public class MoveComparer : IComparer<Move> // jenny -- makes it better yippee :]
{
    public int Compare(Move moveA, Move moveB)
    {

        if (moveA.score == moveB.score) // if equal
        {
            return 0;
        }
        else if (moveB.score > moveA.score) // descending order
        {
            return 1;
        }
        else
        {
            return -1;
        }

    }
}


public struct MinMaxJob : IJob
{
    /*public class ScoredMove
    {
        public Move move;
        public float score;

        public ScoredMove(Move newMove, float newScore)
        {
            move = newMove;
            score = newScore;
        }

    }
    public class MoveComparer : IComparer<Move> // jenny -- makes it better yippee :]
    {
        public int Compare(Move moveA, Move moveB)
        {

            if (moveA.score == moveB.score) // if equal
            {
                return 0;
            }
            else if (moveB.score > moveA.score) // descending order
            {
                return 1;
            }
            else
            {
                return -1;
            }

        }
    }
    */

    public int totalNumNodesLookedAt;
    public BoardAnalyzer analyzer;
    public MoveComparer mc;
    public int maxDepth;
    public enum playerToMove
    {
        player, torok
    }
    public playerToMove toMove;
    public Move selectedMove;

    public MinMaxJob(int depth, playerToMove FirstToMove)
    {
        maxDepth = depth;
        toMove = FirstToMove;
        mc = new MoveComparer();
        totalNumNodesLookedAt = 0;
        selectedMove = null;
        analyzer= new BoardAnalyzer();

    }

    public void Execute()
    {
        totalNumNodesLookedAt = 0;
        float startTime = Time.realtimeSinceStartup;
        //Debug.Log("AI: Looking for move...");
        ScoredMove resultMove = MinMaxRecursive(maxDepth, toMove, float.MinValue, float.MaxValue);
        //print(resultMove.move == null);
        if (resultMove.move != null)
        {
            Debug.Log("AI: Move found. " + resultMove.move.DisplayMove());
            selectedMove = resultMove.move;
        }
        //print("Moves " + numOfMovesCalled);
        //print("Undos " + numOfUndoCalled);
        Debug.Log("Total Number of Nodes Searched: " + totalNumNodesLookedAt);
        Debug.Log("Time taken: " + (Time.realtimeSinceStartup - startTime));
        //print("Move history list count " + Board.instance.moveList.Count);
        //return resultMove.move;
    }

    private ScoredMove MinMaxRecursive(int depth, playerToMove whosMoving, float alpha, float beta)
    {
        //recursive termination
        if (depth == 0)
        {
            //print("Move List count: " + Board.instance.moveList.Count);
            totalNumNodesLookedAt++;
            return new ScoredMove(null, analyzer.Analyze(Board.pieceBoard));
        }

        ScoredMove bestMove;//holder for the best/most likely move to make

        if (whosMoving == playerToMove.player)//max
        {

            List<Move> allAvailableMoves = Board.instance.GetAllMoves(false);//get list of all possible moves

            //check if allavailableMoves has no moves
            if (allAvailableMoves.Count < 1)
            {
                return new ScoredMove(null, float.NegativeInfinity);
            }

            allAvailableMoves.Sort(mc); // jenny

            bestMove = new ScoredMove(allAvailableMoves[0], float.NegativeInfinity);//set best move score to be as low as possible);
            //print("Amount of moves player moves available: " + allAvailableMoves.Count);

            foreach (Move move in allAvailableMoves)
            {
                Board.instance.MovePiece(move.startX, move.startY, move.endX, move.endY);//move piece
                ScoredMove recursiveResult = MinMaxRecursive(depth - 1, playerToMove.torok, alpha, beta);//recursive call
                Board.instance.UndoMove();//undo previous move

                if (recursiveResult.score > bestMove.score)//if subtree result is better make best move equal to that
                {
                    bestMove.move = move;
                    bestMove.score = recursiveResult.score;
                }

                alpha = Mathf.Max(alpha, bestMove.score);//update alpha value if needed

                if (alpha >= beta)
                {
                    //print("broke in max");
                    break;
                }
            }
        }
        else//min
        {
            List<Move> allAvailableMoves = Board.instance.GetAllMoves(true);// get list of all possible moves

            //check if allavailableMoves has no moves
            if (allAvailableMoves.Count < 1)
            {
                return new ScoredMove(null, float.PositiveInfinity);
            }

            allAvailableMoves.Sort(mc); // jenny

            bestMove = new ScoredMove(allAvailableMoves[0], float.PositiveInfinity);
            //print("Amount of moves torok moves available: " + allAvailableMoves.Count);
            foreach (Move move in allAvailableMoves)
            {
                Board.instance.MovePiece(move.startX, move.startY, move.endX, move.endY);//move piece
                ScoredMove recursiveResult = MinMaxRecursive(depth - 1, playerToMove.player, alpha, beta);//recursive call
                Board.instance.UndoMove();//undo previous move

                if (recursiveResult.score < bestMove.score)// if subtree is better make best move equal to that
                {
                    bestMove.move = move;
                    bestMove.score = recursiveResult.score;
                }

                beta = Mathf.Min(beta, bestMove.score);//update beta if needed

                if (alpha >= beta)
                {
                    //print("broke in main");
                    break;
                }

            }
        }


        //return resulting move
        return bestMove;
    }
}