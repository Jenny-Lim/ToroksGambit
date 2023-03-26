using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;
using System.Threading.Tasks;
using System;
using System.Linq;

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
    private float startTime;

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


    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        initDepth = maxDepth;
    }

    //-----------Iterative Version
    #region "Helpers"
    private int FindMax(List<ScoredMove> list)
    {
        if (list.Count <= 0) { return -1; }

        int returnIndex = 0;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].score > list[returnIndex].score) { returnIndex = i; }
        }
        return returnIndex;
    }
    private int FindMin(List<ScoredMove> list)
    {
        if (list.Count <= 0) { return -1; }

        int returnIndex = 0;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].score < list[returnIndex].score) { returnIndex = i; }
        }
        return returnIndex;

    }
    #endregion

    public void GetMinMaxMoveIter(DataHolder<Move> resultMove)
    {
        finishedSearch = false;
        startTime = Time.time;
        StopAllCoroutines();

        StartCoroutine(GetMinMaxMoveIterCoRo(maxDepth, resultMove));
    }



    private IEnumerator GetMinMaxMoveIterCoRo(int maxDepth, DataHolder<Move> resultMove)
    {
        Stack<StateStorage> stack = new Stack<StateStorage>();// the stack
        stack.Push(new StateStorage(playerToMove.torok, Board.instance.GetAllMoves(true), maxDepth));//push current state to stack
        StateStorage curState = null;// the currently looked at state

        while (stack.Count > 0)//while stack not empty
        {
            #region "Max Searches Per Frame"
            if (totalNumNodesLookedAt >= maxSearchPerFrame)//yield for next frame if max searches allowed for this frame reached
            {
                totalNumNodesLookedAt = 0;
                yield return null;
            }
            #endregion

            #region "Get Current Stack State"
            curState = stack.Pop();//get next stack state
            totalNumNodesLookedAt++;
            #endregion

            #region "Leaf Node Reached"
            if (curState.depth <= 0)//leaf node reached
            {
                /*StateStorage curStateParent = stack.Pop();

                float leafAnalysis = BoardAnalyzer.instance.Analyze(Board.pieceBoard, maxDepth + GameStateManager.GetTurnCount());
                if (curState.toMove == playerToMove.player)//max
                {
                    if (leafAnalysis > curStateParent.bestMove.score)//max
                    {
                        curStateParent.bestMove.score = leafAnalysis;
                        curStateParent.bestMove.move = null;
                    }
                }
                else
                {
                    if (leafAnalysis < curStateParent.bestMove.score)//min
                    {
                        curStateParent.bestMove.score = leafAnalysis;
                        curStateParent.bestMove.move = null;
                    }
                }

                stack.Push(curStateParent);*/

                StateStorage curStateParent = stack.Pop();//this would be the parent of curState

                ScoredMove leafValue = new ScoredMove();
                leafValue.score = BoardAnalyzer.instance.Analyze(Board.pieceBoard, maxDepth + GameStateManager.GetTurnCount());

                curStateParent.scoredList.Add(leafValue);

                /*//update parent values to store leaf node
                float boardScore = BoardAnalyzer.instance.Analyze(Board.pieceBoard, maxDepth + GameStateManager.GetTurnCount());
                curStateParent.moveValues.Add(boardScore);*/

                stack.Push(curStateParent);//push parent back to stack

                Board.instance.UndoMove();
                continue;
            }
            #endregion

            #region "Find Next Not Searched Move"
            int indexToLookAt = curState.searchedMoves.IndexOf(false);//the first unsearched move in this state
            if (curState.lastIndexSearched >= curState.availMoves.Count-1)//there are no new moves left
            {

                //if this is the root state/node
                if (curState.depth == maxDepth)
                {
                    //return result from best score from scoredList of root node
                    if (curState.toMove == playerToMove.player)//max
                    {
                        resultMove.data = curState.scoredList[FindMax(curState.scoredList)].move;
                    }
                    else//min
                    {
                        resultMove.data = curState.scoredList[FindMin(curState.scoredList)].move;
                    }
                    finishedSearch = true;
                    Debug.Log("Time took for search: " + (Time.time - startTime) + " sec");
                    break;
                }


                StateStorage curStateParent = stack.Pop();//get the parent state

                int indexOfBest;//find the best value
                if (curState.toMove == playerToMove.player)//if max state
                {
                    indexOfBest = FindMax(curState.scoredList);
                }
                else//if min state
                {
                    indexOfBest = FindMin(curState.scoredList);
                }

                //create scoredMove using the last move the parent made and the best score the curState has
                ScoredMove curStateResult = new ScoredMove(curStateParent.availMoves[curStateParent.lastIndexSearched], 
                    curState.scoredList[indexOfBest].score);

                //add to parent scoredlist
                curStateParent.scoredList.Add(curStateResult);

                //place parent back on state
                stack.Push(curStateParent);

                //undo move that the parent made to get to this position
                Board.instance.UndoMove();
                continue;
            }
            #endregion

            #region "Push Cur State to Stack, Move Piece For Next State"
            stack.Push(curState);
            curState.lastIndexSearched++;
            Board.instance.MovePiece(curState.availMoves[curState.lastIndexSearched].startX, curState.availMoves[curState.lastIndexSearched].startY, 
                curState.availMoves[curState.lastIndexSearched].endX, curState.availMoves[curState.lastIndexSearched].endY);//make the change to the board for the next pushed state
            #endregion

            #region "Get child State Data"
            playerToMove nextPlayerToMove = playerToMove.player;
            if (curState.toMove == playerToMove.player) { nextPlayerToMove = playerToMove.torok; }
            List<Move> nextStateMoves = Board.instance.GetAllMoves(nextPlayerToMove == playerToMove.torok);
            nextStateMoves.Sort(mc);
            #endregion

            #region "Add Child State To Stack"
            StateStorage childState = new StateStorage(nextPlayerToMove, nextStateMoves, curState.depth-1);
            stack.Push(childState);
            curState.searchedMoves[indexToLookAt] = true;
            #endregion

        }
    }

    //~~~~~~~~~Iterative Version end

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

public enum playerToMove
{
    player, torok
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

//this class stores the game state
public class StateStorage
{
    public StateStorage()
    {

    }

    public StateStorage(playerToMove movingPlayer, List<Move> moves, int depth)
    {
        toMove = movingPlayer;
        availMoves = moves;

        bestMove = new ScoredMove();
        if (movingPlayer == playerToMove.player) bestMove.score = float.MinValue;
        else bestMove.score = float.MaxValue;

        searchedMoves = new List<bool>(moves.Count);
        for (int i = 0; i < availMoves.Count; i++)
        {
            searchedMoves.Add(false);
        }

        moveValues = new List<float>(moves.Count);
        for (int i = 0; i < availMoves.Count; i++)
        {
            moveValues.Add(0.0f);
        }
        this.depth = depth;

        scoredList= new List<ScoredMove>();
    }

    public List<Move> availMoves;//list of all moves
    public List<bool> searchedMoves;// list of which index of availMoves has been searched, may or may not actually need this
    public int lastIndexSearched = -1;//the index of availMoves which was searched last
    public ScoredMove bestMove;// the best value that this state has so far
    public playerToMove toMove;//if this state is a min or max state, player = max, torok = min
    public int depth;//what depth of the tree this state is
    public List<float> moveValues;//the values of the moves in availMoves i:i
    public List<ScoredMove> scoredList;
    

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