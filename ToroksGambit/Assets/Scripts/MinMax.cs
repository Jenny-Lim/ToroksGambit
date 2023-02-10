using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinMax : MonoBehaviour
{
    public static MinMax instance;//static instance of this
    private BoardAnalyzer analyzer = new BoardAnalyzer();
    [SerializeField] private int maxDepth = 1;

    private List<ScoredMove> scoredMoves = new List<ScoredMove>();

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


    // jenny start -- these aren't called on rn
    private void ScoreMoves(List<Move> allAvailableMoves) // could have score in move, could have score in scored move, could sort before, could sort during, could even sort as we put moves into their list
    {
        //List<ScoredMove> scoredMoves = new List<ScoredMove>();
        float score;
        foreach (Move m in allAvailableMoves)
        {
            if (m.pieceTaken == 0)
            {
                score = analyzer.Analyze(Board.pieceBoard);
            }
            else // if capturing
            {
                score = (m.pieceTaken - m.pieceMoving) + (m.pieceTaken / 5); // formula may have 'conflicts' idk yet--also i think toroks score would have to be negative to be better for him so *-1?
            }

            scoredMoves.Add(new ScoredMove(m, score));
        }
        //scoredMoves.Sort((x, y) => y.score.CompareTo(x.score));
        //return scoredMoves;
    }

    private void PickMove(int startIndex) // have yet to thonk about it more because im very tired today.
                                          // if we're lucky this just works (its supposed to be a sort as we go thru the tree, so we dont necessarily have to sort everything)
    {
        for (int i = startIndex + 1; i > scoredMoves.Count; i++)
        {
            if (scoredMoves[i].score > scoredMoves[startIndex].score)
            {
                Swap(startIndex, i);
            }
        }
    }

    private void Swap(int startIndex, int i)
    {
        ScoredMove temp = scoredMoves[startIndex];
        scoredMoves[startIndex] = scoredMoves[i];
        scoredMoves[i] = temp;
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

            // clear scoredMoves
            //scoredMoves.Clear();
            // score moves
            //ScoreMoves(allAvailableMoves); // from here on out, would use scoredMoves if i go with this implementation

            //check if allavailableMoves has no moves
            if (allAvailableMoves.Count < 1)
            {
                return new ScoredMove(null, float.NegativeInfinity);
            }

            bestMove = new ScoredMove(allAvailableMoves[0],  float.NegativeInfinity);//set best move score to be as low as possible);
            //print("Amount of moves player moves available: " + allAvailableMoves.Count);


            foreach (Move move in allAvailableMoves)
            {
                // pick move
                //PickMove(i);

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


            }
        }
        else//min
        {
            List<Move> allAvailableMoves = Board.instance.GetAllMoves(true);// get list of all possible moves

            // clear scoredMoves
            //scoredMoves.Clear();
            // score moves
            //ScoreMoves(allAvailableMoves); // from here on out, would use scoredMoves if i go with this implementation

            //check if allavailableMoves has no moves
            if (allAvailableMoves.Count < 1)
            {
                return new ScoredMove(null, float.PositiveInfinity);
            }

            bestMove = new ScoredMove(allAvailableMoves[0], float.PositiveInfinity);
            //print("Amount of moves torok moves available: " + allAvailableMoves.Count);
            foreach (Move move in allAvailableMoves)
            {
                // pick move
                //PickMove(i);

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
