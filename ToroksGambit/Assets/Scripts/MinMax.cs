using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinMax : MonoBehaviour
{
    public static MinMax instance;//static instance of this
    private BoardAnalyzer analyzer = new BoardAnalyzer();

    int numOfUndoCalled = 0;
    int numOfMovesCalled = 0;

    private class ScoredMove {
        public Move move;
        public float score;

        public ScoredMove(Move newMove, float newScore)
        {
            move = newMove;
            score = newScore;
        }
    }

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
    public Move GetMinMaxMove(int maxDepth, playerToMove toMove)
    {
        //Debug.Log("AI: Looking for move...");
        ScoredMove resultMove = MinMaxRecursive(maxDepth, toMove, float.MaxValue, float.MinValue);
        //print(resultMove.move == null);
        if (resultMove.move != null)
        {
            Debug.Log("AI: Move found. " + resultMove.move.DisplayMove());
        }
        print("Moves " + numOfMovesCalled);
        print("Undos " + numOfUndoCalled);
        return resultMove.move;
    }

    //the recursive functionality of the minmax call
    private ScoredMove MinMaxRecursive(int depth, playerToMove whosMoving, float alpha, float beta)
    {
        //recursive termination
        if (depth == 0)
        {
            print("reached depth 0");
            return new ScoredMove(null, analyzer.Analyze(Board.pieceBoard));
        }

        ScoredMove bestMove = new ScoredMove(null, 0);//holder for the best/most likely move to make

        if (whosMoving == playerToMove.player)//max
        {
            bestMove.score = float.NegativeInfinity;//set best move score to be as low as possible
            List<Move> allAvailableMoves = Board.instance.GetAllMoves(false);//get list of all possible moves
            
            /*if (allAvailableMoves.Count <= 0)
            {
                if (whosMoving == playerToMove.player)
                {
                    return new ScoredMove(null, float.NegativeInfinity);
                }
                else
                {
                    return new ScoredMove(null, float.PositiveInfinity);
                }
            }*/


            foreach (Move move in allAvailableMoves)
            {
                Board.instance.MovePiece(move.startX, move.startY, move.endX, move.endY);//move piece
                numOfMovesCalled++;
                ScoredMove recursiveResult = MinMaxRecursive(depth-1, playerToMove.torok, alpha , beta);//recursive call
                Board.instance.UndoMove();//undo previous move
                numOfUndoCalled++;

                if (recursiveResult.score > bestMove.score)//if subtree result is better make best move equal to that
                {
                    bestMove.move = move;
                    bestMove.score = recursiveResult.score;
                }

                alpha = Mathf.Max(alpha, bestMove.score);//update alpha value if needed

                /*if (beta <= alpha)//prune tree if applicable
                {
                    print("got into break");
                    break;
                }*/

            }
        }
        else//min
        {
            bestMove.score = float.PositiveInfinity;//set best move score to be as high as possible
            List<Move> allAvailableMoves = Board.instance.GetAllMoves(true);// get list of all possible moves

            foreach (Move move in allAvailableMoves)
            {
                Board.instance.MovePiece(move.startX, move.startY, move.endX, move.endY);//move piece
                numOfMovesCalled++;
                ScoredMove recursiveResult = MinMaxRecursive(depth - 1, playerToMove.torok, alpha, beta);//recursive call
                Board.instance.UndoMove();//undo previous move
                numOfUndoCalled++;

                if (recursiveResult.score < bestMove.score)// if subtree is better make best move equal to that
                {
                    bestMove.move = move;
                    bestMove.score = recursiveResult.score;
                }

                beta = Mathf.Min(beta, bestMove.score);//update beta if needed

                /*if (beta <= alpha)//prune tree if applicable
                {
                    print("got into break");
                    break;
                }*/

            }
        }


        //return resulting move
        return bestMove;

    }
}
