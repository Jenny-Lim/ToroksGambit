//using OpenCover.Framework.Model;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Unity.Jobs;
using System.Collections;
using TMPro;
using UnityEngine.UIElements;

public class GameStateManager : MonoBehaviour
{
    //level stuff
    public List<string> LevelNames;
    public int currentLevelNumber = 0;
    public enum GameState
    {
        none,
        title,
        intro,
        deployment,
        game,
        win,
        shop
    }

    public BaseCondition winCondition;
    private static BaseCondition.Condition mostRecentWinCheckResult = BaseCondition.Condition.None;

    private static bool isPlayersTurn = true;
    [SerializeField] private static int turnCount = 1;//the amount of moves/turns that have happened in the current game
    [SerializeField] private GameState currentState = GameState.title;
    private bool TorokIsMoving;

    public static GameStateManager instance;

    JobHandle handle;
    MinMaxJob moveSearchJob;
    public bool lookingForMove = false;
    [SerializeField] private GameObject victoryText;
    [SerializeField] private Coroutine activeCoRo;
    public static bool lastValidateCheck = false;

    [SerializeField] private static TextMeshProUGUI objectiveText;


    

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        
    }

    private void Start()
    {
        
        //if (LevelNames.Count > 0)
        //{
            //BoardLoader.instance.LoadBoard(LevelNames[0]);
        //}
        
    }

    public bool GetIsPlayersTurn()
    {
        return isPlayersTurn;
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

                    ////chekc win condition
                    //if (currentState == GameState.game)
                    //{
                    //    //win condition checks
                    //    if (winCondition != null)
                    //    {
                    //        winCondition.ProgressConditionState();
                    //        mostRecentWinCheckResult = winCondition.IsWinCondition();
                    //    }

                    //    if (mostRecentWinCheckResult == BaseCondition.Condition.Player)
                    //    {
                    //        ChangeGameState(GameState.win);
                    //        //reset this state
                    //    }
                    //    else if (mostRecentWinCheckResult == BaseCondition.Condition.Torok)
                    //    {
                    //        Debug.Log("Torok has won.");
                    //        //lose logic
                    //    }
                    //}
                }
                else
                {
                    
                    /*if (lookingForMove)
                    {
                        if (handle.IsCompleted)
                        {
                            Move resultMove = moveSearchJob.selectedMove;
                            if (resultMove != null)
                            {
                                //Board.instance.MovePieceVisual(resultMove.startX, resultMove.startY, resultMove.endX, resultMove.endY, Board.pieceBoard[resultMove.startX, resultMove.startY], resultMove.promoted);
                                Board.instance.canMove = false;
                                Board.instance.MoveValidator(resultMove.startX, resultMove.startY, resultMove.endX, resultMove.endY);
                            }
                            else
                            {
                                Debug.Log("MinMax was not able to find a move. Either the game has ended, or it has no pieces on the board");
                                Debug.Log("Switching back to player's turn for convenience");
                            }
                            lookingForMove = false;
                            EndTurn();
                        }
                    }
                    else
                    {
                        moveSearchJob = new MinMaxJob(MinMax.instance.maxDepth, MinMaxJob.playerToMove.torok);
                        handle = moveSearchJob.Schedule();
                        lookingForMove = true;
                    }*/


                    if (!TorokIsMoving)
                    {
                        TorokIsMoving = true;
                        Move resultMove = MinMax.instance.GetMinMaxMove(MinMax.playerToMove.torok);
                        if (resultMove != null)
                        {
                            if (lastValidateCheck == false)
                            {
                                //Board.instance.MovePieceVisual(resultMove.startX, resultMove.startY, resultMove.endX, resultMove.endY, Board.pieceBoard[resultMove.startX, resultMove.startY], resultMove.promoted);
                                Board.instance.canMove = false;
                                //Board.instance.MoveValidator(resultMove.startX, resultMove.startY, resultMove.endX, resultMove.endY);
                                Board.instance.MoveValidatorCoRo(resultMove.startX, resultMove.startY, resultMove.endX, resultMove.endY);
                            }
                            
                            //EndTurn();//take out, move to coro
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
            case GameState.intro:
                //make camera look at torok
                //play animation
                if (activeCoRo == null)
                {
                    activeCoRo = StartCoroutine(IntroCoRo());
                    Inventory.instance.SetObjective();
                }

                break;
            case GameState.win:
                //put up some text that says you wont
                //add tickets
                if (activeCoRo == null)
                {
                    activeCoRo = StartCoroutine(WinAnimCoro());
                }
                

                break;
            case GameState.title:
                //just a title bro
                //print(MainMenu.instance.startPressed);
                if (activeCoRo == null)
                {
                    activeCoRo = StartCoroutine(titleCoRo());
                }
                break;
        }
    }

    public IEnumerator titleCoRo()
    {
        //if (MainMenu.instance.menuDone) {
        while (!CameraHeadMovements.instance.menuDone)
        {
            //CameraHeadMovements.instance.LookAtPlayArea();
            yield return null;
        }
            ChangeGameState(GameState.intro);
            activeCoRo = null;
        //}
    }

    public IEnumerator IntroCoRo()
    {
        yield return new WaitForSeconds(1);
        CameraHeadMovements.instance.LookAtTorok(2f);
        yield return new WaitForSeconds(1);

        //depending if we want this to play always this check can be taken out
        float rand = Random.Range(0,1);
        if (TorokPersonalityAI.instance.ShouldPlay(SoundLibrary.Categories.LevelIntro, rand))
        {
            TorokPersonalityAI.instance.PlayAnimationAndSound(SoundLibrary.Categories.LevelIntro);
        }

        yield return new WaitForSeconds(3.2f);
        ChangeGameState(GameState.deployment);
        activeCoRo = null;
    }

    public IEnumerator WinAnimCoro()
    {
        Inventory.instance.HideInventoryPanel();
        Inventory.instance.objectiveArea.SetActive(false);
        float counter = 0;
        while (counter < 2000)
        {
            if (Mathf.Sin(counter * 0.02f) > 0)
            {
                victoryText.SetActive(true);
            }
            else
            {
                victoryText.SetActive(false);
            }
            counter++;
            yield return null;
        }
        victoryText.SetActive(false);
        Debug.Log("Player has won.");

        Board.instance.ReturnPiecesToInventory();
        Currency.instance.GetReward(currentLevelNumber + 1);
        turnCount = 1;
        isPlayersTurn = true;
        ChangeGameState(GameState.shop);
        PhysicalShop.instance.EnterShop();
        activeCoRo = null;
        
    }

    public void SetNextLevel()
    {
        ResetToDeploy();
        Board.instance.Reset();
        Inventory.instance.hasPlacedPiece = false;
        if (currentLevelNumber + 1 < LevelNames.Count)
        {
            BoardLoader.instance.LoadBoard(LevelNames[++currentLevelNumber]);
        }
        Inventory.instance.ShowInventoryPanel();
        Inventory.instance.SetObjective();



        
    }

    public void ChangeGameState(GameState newState)
    {
        currentState = newState;
        if (turnCount == 1 && newState == GameState.game)
        {
            InterruptManager.instance.EnactInterrupts(InterruptManager.InterruptTrigger.GameStart);
        }
    }

    public void EndTurn()
    {
        //win condition checks
        if (winCondition != null)
        {
            winCondition.ProgressConditionState();
            mostRecentWinCheckResult = winCondition.IsWinCondition();
        }

        if (mostRecentWinCheckResult == BaseCondition.Condition.Player)
        {
            ChangeGameState(GameState.win);
            //reset this state
        }
        else if (mostRecentWinCheckResult == BaseCondition.Condition.Torok)
        {
            Debug.Log("Torok has won.");
            //lose logic
        }


        lastValidateCheck = false;
        Board.playerInCheck = Board.instance.IsKingInCheck(false);
        Board.torokInCheck = Board.instance.IsKingInCheck(true);
        InterruptManager.instance.EnactInterrupts(InterruptManager.InterruptTrigger.AfterTurn);
        turnCount++;
        isPlayersTurn = !isPlayersTurn;
        Inventory.instance.SetObjective();
        //print("Current Board Score " + BoardAnalyzer.instance.Analyze(Board.pieceBoard));
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
        Inventory.instance.hasPlacedPiece = false;//replaced below
        //Inventory.instance.ResetStartButton();
    }

    public void ResetToDeploy()
    {
        ChangeGameState(GameState.deployment);
        turnCount = 1;
        isPlayersTurn = true;
        Inventory.instance.hasPlacedPiece = false;//replaced below
        //Inventory.instance.ResetStartButton();
    }

    public void ResetGame()
    {
        ChangeGameState(GameState.game);
        turnCount = 1;
        isPlayersTurn = true;
        InterruptManager.instance.ResetInterruptListTriggers();
    }
}
