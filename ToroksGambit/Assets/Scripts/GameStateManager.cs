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


    [SerializeField] private bool isPlayersTurn = true;
    [SerializeField] private static int turnCount = 1;//the amount of moves/turns that have happened in the current game
    [SerializeField] private GameState currentState;

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case GameState.deployment:
                Inventory.instance.InventoryUpdate();
                break;

            case GameState.game:
                Board.instance.BoardUpdate();
                break;
            case GameState.shop:
                //ShopUpdate();
                break;
        }

        if (Input.GetKeyDown("q"))
        {
            Move resultMove = MinMax.instance.GetMinMaxMove(2, MinMax.playerToMove.torok);
            
            //if (Board.instance.canMove)
            //{
                print("call visual move");
                Board.instance.MovePieceVisual(resultMove.startX, resultMove.startY, resultMove.endX, resultMove.endY, Board.pieceBoard[resultMove.startX, resultMove.startY]);
            //}
            Board.instance.canMove = false;
            print("call move Validate");
            Board.instance.MoveValidator(resultMove.startX, resultMove.startY, resultMove.endX, resultMove.endY);
        }

    }

    public void ChangeGameState(GameState newState)
    {
        currentState = newState;
    }

    public void EndTurn()
    {
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
}
