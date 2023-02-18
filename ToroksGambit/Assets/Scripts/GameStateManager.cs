//using OpenCover.Framework.Model;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class GameStateManager : MonoBehaviour
{
    //level stuff
    public List<string> LevelNames;
    public int currentLevelNumber = 0;
    public enum GameState
    {
        none,
        deployment,
        game,
        shop
    }

    public BaseCondition winCondition;
    private static BaseCondition.Condition mostRecentWinCheckResult = BaseCondition.Condition.None;

    private static bool isPlayersTurn = true;
    [SerializeField] private static int turnCount = 1;//the amount of moves/turns that have happened in the current game
    [SerializeField] private GameState currentState = GameState.deployment;
    private bool TorokIsMoving;

    public static GameStateManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        
    }

    private void Start()
    {
        
        if (LevelNames.Count > 0)
        {
            BoardLoader.instance.LoadBoard(LevelNames[0]);
        }
        
        
    }

    public bool GetIsPlayersTurn()
    {
        return isPlayersTurn;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("g"))
        {
            print(isPlayersTurn);
        }

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

                    //chekc win condition
                    if (currentState == GameState.game)
                    {
                        //win condition checks
                        if (winCondition != null)
                        {
                            winCondition.ProgressConditionState();
                            mostRecentWinCheckResult = winCondition.IsWinCondition();
                        }

                        if (mostRecentWinCheckResult == BaseCondition.Condition.Player)
                        {
                            Debug.Log("Player has won.");
                            Currency.instance.GetReward(currentLevelNumber + 1);
                            ChangeGameState(GameState.shop);
                            turnCount = 1;
                            isPlayersTurn = true;
                            PhysicalShop.instance.EnterShop();

                            //reset this state
                        }
                        else if (mostRecentWinCheckResult == BaseCondition.Condition.Torok)
                        {
                            Debug.Log("Torok has won.");
                            //lose condition
                        }
                    }
                }
                else
                {
                    if (!TorokIsMoving)
                    {
                        TorokIsMoving = true;
                        Move resultMove = MinMax.instance.GetMinMaxMove(MinMax.playerToMove.torok);
                        if (resultMove != null)
                        {
                            //Board.instance.MovePieceVisual(resultMove.startX, resultMove.startY, resultMove.endX, resultMove.endY, Board.pieceBoard[resultMove.startX, resultMove.startY], resultMove.promoted);
                            Board.instance.canMove = false;
                            Board.instance.MoveValidator(resultMove.startX, resultMove.startY, resultMove.endX, resultMove.endY);
                            EndTurn();
                        }
                        else
                        {
                            Debug.Log("MinMax was not able to find a move. Either the game has ended, or it has no pieces on the board");
                            Debug.Log("Switching back to player's turn for convenience");
                            EndTurn();//this will eventually be deleted
                        }
                       
                    }
                }
                break;
            case GameState.shop:
                PhysicalShop.instance.PhysicalShopUpdate();
                break;
        }
    }

    public void SetNextLevel()
    {
        ResetToDeploy();
        if (currentLevelNumber + 1 < LevelNames.Count)
        {
            BoardLoader.instance.LoadBoard(LevelNames[++currentLevelNumber]);
        }
        
    }

    public void ChangeGameState(GameState newState)
    {
        currentState = newState;
        if (turnCount == 1 && newState == GameState.game)
        {
            InterruptManager.instance.EnactInterrupts(InterruptManager.InterruptTrigger.GameStart);
        }
    }

    public static void EndTurn()
    {
        Board.playerInCheck = Board.instance.IsKingInCheck(false);
        Board.torokInCheck = Board.instance.IsKingInCheck(true);
        InterruptManager.instance.EnactInterrupts(InterruptManager.InterruptTrigger.AfterTurn);
        turnCount++;
        isPlayersTurn = !isPlayersTurn;
        //check win condition
    }

    public static int GetTurnCount()
    {
        return turnCount;
    }

    public void ResetGameDeploy()
    {
        ChangeGameState(GameState.deployment);
        turnCount = 1;
        isPlayersTurn = true;
        InterruptManager.instance.ResetInterruptListTriggers();
        Inventory.instance.ResetStartButton();
    }

    public void ResetToDeploy()
    {
        ChangeGameState(GameState.deployment);
        turnCount = 1;
        isPlayersTurn = true;
        Inventory.instance.ResetStartButton();
    }

    public void ResetGame()
    {
        ChangeGameState(GameState.game);
        turnCount = 1;
        isPlayersTurn = true;
        InterruptManager.instance.ResetInterruptListTriggers();
    }
}
