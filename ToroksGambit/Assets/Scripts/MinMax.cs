using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinMax : MonoBehaviour
{
    public static MinMax instance;//static instance of this
    private BoardAnalyzer analyzer = new BoardAnalyzer();
    [SerializeField] private int maxDepth = 1;

    //int numOfUndoCalled = 0;
    //int numOfMovesCalled = 0;

    int totalNumNodesLookedAt = 0;

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


    // jenny start -- this isnt called on rn, still doing

    private void PickMove(ref List<Move> allAvailableMoves, int startIndex) // supposed to sort as we go through the moves in minmax, so we dont sort unnescessarily?? - need to test
    {
        for (int i = startIndex + 1; i > allAvailableMoves.Count; i++)
        {
            if (allAvailableMoves[i].score > allAvailableMoves[startIndex].score)
            {
                // swap
                Move temp = allAvailableMoves[startIndex];
                allAvailableMoves[startIndex] = allAvailableMoves[i];
                allAvailableMoves[i] = temp;
            }
        }
    }

    //jenny end


    //the recursive wrapper for the minmax call
    public Move GetMinMaxMove(playerToMove toMove)
    {
        totalNumNodesLookedAt = 0;
        //Debug.Log("AI: Looking for move...");
        ScoredMove resultMove = MinMaxRecursive(maxDepth, toMove, float.MaxValue, float.MinValue);
        //print(resultMove.move == null);
        if (resultMove.move != null)
        {
            Debug.Log("AI: Move found. " + resultMove.move.DisplayMove());
        }
        //print("Moves " + numOfMovesCalled);
        //print("Undos " + numOfUndoCalled);
        print("Total Number of Nodes Searched: " + totalNumNodesLookedAt);
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

            bestMove = new ScoredMove(allAvailableMoves[0],  float.NegativeInfinity);//set best move score to be as low as possible);
            //print("Amount of moves player moves available: " + allAvailableMoves.Count);

            //int i = 0;
            foreach (Move move in allAvailableMoves)
            {
                // pick move
                //PickMove(ref allAvailableMoves, i);

                Board.instance.MovePiece(move.startX, move.startY, move.endX, move.endY);//move piece
                //print("Move from MinMax Depth: " + (maxDepth - depth));
                //numOfMovesCalled++;
                ScoredMove recursiveResult = MinMaxRecursive(depth-1, playerToMove.torok, alpha , beta);//recursive call
                Board.instance.UndoMove();//undo previous move
                //print("Undo from MinMax Depth: " + (maxDepth - depth));
                //numOfUndoCalled++;

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
                //i++;

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

            bestMove = new ScoredMove(allAvailableMoves[0], float.PositiveInfinity);
            //print("Amount of moves torok moves available: " + allAvailableMoves.Count);
            //int i = 0;
            foreach (Move move in allAvailableMoves)
            {
                // pick move
                //PickMove(ref allAvailableMoves, i);

                Board.instance.MovePiece(move.startX, move.startY, move.endX, move.endY);//move piece
                //print("Move from MinMax Depth: " + (maxDepth - depth));
                //numOfMovesCalled++;
                ScoredMove recursiveResult = MinMaxRecursive(depth - 1, playerToMove.player, alpha, beta);//recursive call
                Board.instance.UndoMove();//undo previous move
                //print("Undo from MinMax Depth: " + (maxDepth - depth));
                //numOfUndoCalled++;

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
                //i++;
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
