using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;
using System.Threading.Tasks;

public class MinMax : MonoBehaviour
{
    public static MinMax instance;//static instance of this
    private BoardAnalyzer analyzer = new BoardAnalyzer();
    [SerializeField] public int maxDepth = 1;
    MoveComparer mc = new MoveComparer();
    private int initDepth;

    //int numOfUndoCalled = 0;
    //int numOfMovesCalled = 0;

    int totalNumNodesLookedAt = 0;
    int searchCounter = 0;
    private bool initRunning = false;
    [HideInInspector] public bool lookingForMove = false;
    [HideInInspector] public bool finishedSearch = false;
    [SerializeField] private int maxSearchPerFrame = 100;
    private int numFramesSearched = 0;

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
        initDepth = maxDepth;
    }
    //old version
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
        print("Total Number of Nodes Searched: " + totalNumNodesLookedAt);
        print("Time taken: " + (Time.realtimeSinceStartup - startTime));
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
            return new ScoredMove(null, analyzer.Analyze(Board.pieceBoard, GameStateManager.turnCount + maxDepth));
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
    

    public void ResetDepth()
    {
        maxDepth = initDepth;
    }
    //---------async version
    /*
    public async Task GetMinMaxMoveAsync(DataHolder<Move> resultHolder ,playerToMove toMove)
    {
        finishedSearch = false;
        totalNumNodesLookedAt = 0;
        float startTime = Time.realtimeSinceStartup;
        //Debug.Log("AI: Looking for move...");

        
        return await MinMaxRecursiveAsync(maxDepth, toMove, float.MinValue, float.MaxValue);
        resultHolder.data = resultMove.move;
        finishedSearch = true;
    }

    private async Task<ScoredMove> MinMaxRecursiveAsync(int depth, playerToMove whosMoving, float alpha, float beta)
    {

        //recursive termination
        if (depth == 0)
        {
            //print("Move List count: " + Board.instance.moveList.Count);
            totalNumNodesLookedAt++;
            return new ScoredMove(null, analyzer.Analyze(Board.pieceBoard, GameStateManager.turnCount + maxDepth));
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
            //resultHolder.data = new ScoredMove(allAvailableMoves[0], float.NegativeInfinity);

            //print("Amount of moves player moves available: " + allAvailableMoves.Count);
            foreach (Move move in allAvailableMoves)
            {
                searchCounter++;
                if (searchCounter >= maxSearchPerFrame)
                {
                    searchCounter = 0;
                    await Task.Yield();
                }

                Board.instance.MovePiece(move.startX, move.startY, move.endX, move.endY);//move piece

                ScoredMove recursiveResult = await MinMaxRecursiveAsync(depth - 1, playerToMove.torok, alpha, beta);//recursive call

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
                searchCounter++;
                if (searchCounter >= maxSearchPerFrame)
                {
                    searchCounter = 0;
                    await Task.Yield();
                }

                Board.instance.MovePiece(move.startX, move.startY, move.endX, move.endY);//move piece

                ScoredMove recursiveResult = await MinMaxRecursiveAsync(depth - 1, playerToMove.player, alpha, beta);//recursive call

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
    */
    //-----------

    public void SetNewDepth(int newDepth)
    {
        maxDepth = newDepth;
    }

    //----------coroutine version

    private IEnumerator MinMaxRecursiveCo(DataHolder<ScoredMove> resultData, int depth, playerToMove whosMoving, float alpha, float beta)
    {
        //termination
        if (depth <= 0)
        {
            totalNumNodesLookedAt++;
            resultData.data = new ScoredMove(null, analyzer.Analyze(Board.pieceBoard, GameStateManager.turnCount + maxDepth));
            yield break;
        }

        if (whosMoving == playerToMove.player)//max
        {
            List<Move> allAvailableMoves = Board.instance.GetAllMoves(false);//get list of all possible moves

            //check if allavailableMoves has no moves
            if (allAvailableMoves.Count < 1)
            {
                resultData.data = new ScoredMove(null, float.NegativeInfinity);
                yield break;
            }

            allAvailableMoves.Sort(mc); // jenny

            resultData.data = new ScoredMove(allAvailableMoves[0], float.NegativeInfinity);//set best move score to be as low as possible);
            //print("Amount of moves player moves available: " + allAvailableMoves.Count);

            foreach (Move move in allAvailableMoves)
            {
                totalNumNodesLookedAt++;
                searchCounter++;//increment how many searchs you have done this frame
                if (searchCounter >= maxSearchPerFrame)//if done max searchs yield for next frame
                {
                    searchCounter = 0;
                    numFramesSearched++;
                    yield return null;
                }

                Board.instance.MovePiece(move.startX, move.startY, move.endX, move.endY);//move piece

                DataHolder<ScoredMove> recursiveResult = new DataHolder<ScoredMove>(); //= MinMaxRecursive(depth - 1, playerToMove.torok, alpha, beta);//recursive call data storage

                //yield return StartCoroutine(MinMaxRecursiveCo(recursiveResult, depth - 1, playerToMove.torok, alpha, beta));//recursive call
                yield return MinMaxRecursiveCo(recursiveResult, depth - 1, playerToMove.torok, alpha, beta);

                Board.instance.UndoMove();//undo previous move

                if (recursiveResult.data.score > resultData.data.score)//if subtree result is better make best move equal to that
                {
                    resultData.data.move = move;
                    resultData.data.score = recursiveResult.data.score;
                }

                alpha = Mathf.Max(alpha, resultData.data.score);//update alpha value if needed

                if (alpha >= beta)
                {
                    //print("broke in max");
                    break;
                }
            }
        }
        else
        {
            List<Move> allAvailableMoves = Board.instance.GetAllMoves(true);// get list of all possible moves

            //check if allavailableMoves has no moves
            if (allAvailableMoves.Count < 1)
            {
                resultData.data = new ScoredMove(null, float.PositiveInfinity);
                yield break;
            }

            allAvailableMoves.Sort(mc); // jenny

            resultData.data = new ScoredMove(allAvailableMoves[0], float.PositiveInfinity);
            //print("Amount of moves torok moves available: " + allAvailableMoves.Count);
            foreach (Move move in allAvailableMoves)
            {
                totalNumNodesLookedAt++;
                searchCounter++;//increment how many searchs you have done this frame
                if (searchCounter >= maxSearchPerFrame)//if done max searchs yield for next frame
                {
                    searchCounter = 0;
                    numFramesSearched++;
                    yield return null;
                }

                Board.instance.MovePiece(move.startX, move.startY, move.endX, move.endY);//move piece

                DataHolder<ScoredMove> recursiveResult = new DataHolder<ScoredMove>(); //= MinMaxRecursive(depth - 1, playerToMove.player, alpha, beta);//recursive call data storage

                //yield return StartCoroutine(MinMaxRecursiveCo(recursiveResult, depth - 1, playerToMove.player, alpha, beta));//recursive call
                yield return MinMaxRecursiveCo(recursiveResult, depth - 1, playerToMove.torok, alpha, beta);

                Board.instance.UndoMove();//undo previous move

                if (recursiveResult.data.score < resultData.data.score)// if subtree is better make best move equal to that
                {
                    resultData.data.move = move;
                    resultData.data.score = recursiveResult.data.score;
                }

                beta = Mathf.Min(beta, resultData.data.score);//update beta if needed

                if (alpha >= beta)
                {
                    //print("broke in main");
                    break;
                }

            }
        }
    }

    private IEnumerator MinMaxRecursiveCoInit(DataHolder<ScoredMove> resultData, int depth, playerToMove whosMoving, float alpha, float beta)
    {
        initRunning = true;

        //termination
        if (depth <= 0)
        {
            totalNumNodesLookedAt++;
            resultData.data = new ScoredMove(null, analyzer.Analyze(Board.pieceBoard, GameStateManager.turnCount + maxDepth));
            initRunning = false;
            yield break;
        }

        if (whosMoving == playerToMove.player)//max
        {
            List<Move> allAvailableMoves = Board.instance.GetAllMoves(false);//get list of all possible moves

            //check if allavailableMoves has no moves
            if (allAvailableMoves.Count < 1)
            {
                resultData.data = new ScoredMove(null, float.NegativeInfinity);
                initRunning = false;
                yield break;
            }

            allAvailableMoves.Sort(mc); // jenny

            resultData.data = new ScoredMove(allAvailableMoves[0], float.NegativeInfinity);//set best move score to be as low as possible);
            //print("Amount of moves player moves available: " + allAvailableMoves.Count);

            foreach (Move move in allAvailableMoves)
            {
                totalNumNodesLookedAt++;
                Board.instance.MovePiece(move.startX, move.startY, move.endX, move.endY);//move piece

                DataHolder<ScoredMove> recursiveResult = new DataHolder<ScoredMove>(); //= MinMaxRecursive(depth - 1, playerToMove.torok, alpha, beta);//recursive call data storage

                //yield return StartCoroutine(MinMaxRecursiveCo(recursiveResult, depth - 1, playerToMove.torok, alpha, beta));//recursive call
                yield return MinMaxRecursiveCo(recursiveResult, depth - 1, playerToMove.torok, alpha, beta);

                Board.instance.UndoMove();//undo previous move

                if (recursiveResult.data.score > resultData.data.score)//if subtree result is better make best move equal to that
                {
                    resultData.data.move = move;
                    resultData.data.score = recursiveResult.data.score;
                }

                alpha = Mathf.Max(alpha, resultData.data.score);//update alpha value if needed

                if (alpha >= beta)
                {
                    //print("broke in max");
                    break;
                }
            }
        }
        else
        {
            List<Move> allAvailableMoves = Board.instance.GetAllMoves(true);// get list of all possible moves

            //check if allavailableMoves has no moves
            if (allAvailableMoves.Count < 1)
            {
                resultData.data = new ScoredMove(null, float.PositiveInfinity);
                initRunning = false;
                yield break;
            }

            allAvailableMoves.Sort(mc); // jenny

            resultData.data = new ScoredMove(allAvailableMoves[0], float.PositiveInfinity);
            //print("Amount of moves torok moves available: " + allAvailableMoves.Count);
            foreach (Move move in allAvailableMoves)
            {
                totalNumNodesLookedAt++;
                Board.instance.MovePiece(move.startX, move.startY, move.endX, move.endY);//move piece

                DataHolder<ScoredMove> recursiveResult = new DataHolder<ScoredMove>(); //= MinMaxRecursive(depth - 1, playerToMove.player, alpha, beta);//recursive call data storage

                //yield return StartCoroutine(MinMaxRecursiveCo(recursiveResult, depth - 1, playerToMove.player, alpha, beta));//recursive call
                yield return MinMaxRecursiveCo(recursiveResult, depth - 1, playerToMove.torok, alpha, beta);

                Board.instance.UndoMove();//undo previous move

                if (recursiveResult.data.score < resultData.data.score)// if subtree is better make best move equal to that
                {
                    resultData.data.move = move;
                    resultData.data.score = recursiveResult.data.score;
                }

                beta = Mathf.Min(beta, resultData.data.score);//update beta if needed

                if (alpha >= beta)
                {
                    //print("broke in main");
                    break;
                }

            }
        }

        initRunning = false;
    }

    public IEnumerator GetMinMaxMoveCo(DataHolder<Move> resultMove,playerToMove toMove)
    {
        lookingForMove = true;
        float startTime = Time.time;
        totalNumNodesLookedAt = 0;
        numFramesSearched = 0;

        DataHolder<ScoredMove> finalResult = new DataHolder<ScoredMove>();
        searchCounter = 0;

        yield return StartCoroutine(MinMaxRecursiveCoInit(finalResult,maxDepth, toMove, float.MinValue, float.MaxValue));

        /*while (initRunning)
        {
            yield return null;
        }*/

        resultMove.data = finalResult.data.move;
        lookingForMove = false;
        Debug.Log("Search Time: " + (Time.time - startTime));
        Debug.Log("Num of frames searched " + numFramesSearched);
        Debug.Log("Searched " + totalNumNodesLookedAt + " nodes");
    }

    //------------
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


public class DataHolder<T>
{
    
    public T data;

    public DataHolder()
    {
        data = default(T);
    }

    public DataHolder(T newData)
    {
        data = newData;
    }
}
/*
public struct MinMaxJob : IJob
{
    public class ScoredMove
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
}*/