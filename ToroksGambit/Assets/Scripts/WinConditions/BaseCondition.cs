using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCondition : MonoBehaviour
{
    public enum Condition
    {
        None,
        Player,
        Torok
    }

    protected int playerScore = 0;
    protected int torokScore = 0;
    protected int scoreToWin = 3;


    public virtual Condition IsWinCondition()
    {
        Debug.Log("Check Win Condition");
        return Condition.None;
    } 

    public void IncreasePlayerScore()
    {
        playerScore++;
    }

    public void IncreaseTorokScore() {
        torokScore++;
    }
}
