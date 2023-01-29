using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{

    public enum GameState
    {
        deployment,
        game,
        shop
    }


    private static bool isPlayersTurn = true;
    [SerializeField] private static int turnCount = 1;//the amount of moves/turns that have happened in the current game
    [SerializeField] private GameState currentState;
    private bool TorokIsMoving;

    public static GameStateManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case GameState.deployment:
                Inventory.instance.InventoryUpdate();
                break;

            case GameState.game:
                if (isPlayersTurn)
                {
                    TorokIsMoving = false;
                    Board.instance.BoardUpdate();
                }
                else
                {
                    if (!TorokIsMoving)
                    {
                        TorokIsMoving = true;
                        Move resultMove = MinMax.instance.GetMinMaxMove(2, MinMax.playerToMove.torok);
                        Board.instance.MovePieceVisual(resultMove.startX, resultMove.startY, resultMove.endX, resultMove.endY, Board.pieceBoard[resultMove.startX, resultMove.startY]);
                        Board.instance.canMove = false;
                        Board.instance.MoveValidator(resultMove.startX, resultMove.startY, resultMove.endX, resultMove.endY);
                    }
                }
                break;
            case GameState.shop:
                //ShopUpdate();
                break;
        }
    }

    public void ChangeGameState(GameState newState)
    {
        currentState = newState;
    }

    public static void EndTurn()
    {
        InterruptManager.instance.EnactInterrupts(InterruptManager.InterruptTrigger.AfterTurn);
        turnCount++;
        isPlayersTurn = !isPlayersTurn;
        
    }

    private void EnactTurn()
    {
        if (isPlayersTurn)// do players action
        {

        }
        else// do torok action
        {

        }
    }

    public static int GetTurnCount()
    {
        return turnCount;
    }

    public void ResetGame()
    {
        ChangeGameState(GameState.deployment);
        turnCount = 1;
        isPlayersTurn = true;
    }
}
