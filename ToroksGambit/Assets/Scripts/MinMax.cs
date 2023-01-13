using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinMax : MonoBehaviour
{

    public enum playerToMove
    {
        player, torok
    }

    //the recursive wrapper for the minmax call
    public void GetMinMaxMove(int maxDepth, playerToMove toMove)
    {
        int move = MinMaxRecursive(maxDepth, toMove, float.MaxValue, float.MinValue);   
    }

    //the recursive functionality of the minmax call
    private int MinMaxRecursive(int depth, playerToMove whosMoving, float alpha, float beta)
    {
        //recursive termination
        if (depth == 0)
        {
            return 0;
        }


        if (whosMoving == playerToMove.player)//max
        {
            //best move value = negative infinity
            //get moves for all player pieces

            //for each move in available move list
            //make move 
            //recursive call
            //undo move

            //result equal to max of alpha and result
            //if recursive result is greater or equal to beta
            //break
        }
        else//min
        {
            //best move value = positive infinity
            //get moves for all torok pieces

            //for each move in available move list
            //make move 
            //recursive call
            //undo move

            //result equal to min of beta and result
            //if recursive result is less or equal to alpha
            //break
        }


        //return resulting move
        return 0;

    }
}
