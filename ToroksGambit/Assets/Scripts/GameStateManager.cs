//using OpenCover.Framework.Model;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using System.Threading.Tasks;

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
        shop,
        Interrupt,
        lose
    }

    public BaseCondition winCondition;
    private static BaseCondition.Condition mostRecentWinCheckResult = BaseCondition.Condition.None;

    private static bool isPlayersTurn = true;
    [SerializeField] public static int turnCount = 1;//the amount of moves/turns that have happened in the current game
    [SerializeField] private GameState currentState = GameState.title;
    private bool TorokIsMoving;

    public static GameStateManager instance;

    public bool lookingForMove = false;
    [SerializeField] private GameObject victoryText;
    [SerializeField] private Coroutine activeCoRo;
    public static bool lastValidateCheck = false;

    [SerializeField] private static TextMeshProUGUI objectiveText;

    private DataHolder<Move> resultingMove;

    private Task moveSearchTask;

    [SerializeField] private UIParticleSystem ticketParticleSystem;

    public GameState GetGameState()
    {
        return currentState;
    }
    

    private void Awake()
    {
        //bool isNull;
        if (instance == null)
        {
            instance = this;

            //isNull = true;
        }
        //else
        //{
        //    isNull = false;
        //}
        //print(isNull);
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
                }
                else
                {
                    //BEHOLD MY GRAVEYARD OF MOVE STUTTER FIXES

                    //**THIS IS THE ONE WHERE THE ITERATIVE VERSION IS USED FOR MULTIPLE FRAMES**
                    if (!TorokIsMoving)
                    {
                        resultingMove = new DataHolder<Move>();
                        MinMax.instance.GetMinMaxMoveIter(resultingMove);
                        TorokIsMoving = true;
                    }
                    else
                    {
                        if (MinMax.instance.finishedSearch == true && Board.instance.canMove)//finished search and can make move
                        {
                            Board.instance.MoveValidatorCoRo(resultingMove.data.startX, resultingMove.data.startY, resultingMove.data.endX, resultingMove.data.endY);
                            Board.instance.canMove = false;
                        }
                    }

                    //**THIS IS THE ONE THAT USES TASK SYSTEM AND DOESNT WORK RN MAYBE FOREVER CUZ THIS SHIT WACK**
                    /*if (!TorokIsMoving)
                    {
                        TorokIsMoving = true;
                        moveSearchTask = await MinMax.instance.GetMinMaxMoveAsync(resultingMove, MinMax.playerToMove.torok);
                        
                    }
                    else
                    {
                        Debug.Log(MinMax.instance.finishedSearch);
                        if (MinMax.instance.finishedSearch && Board.instance.canMove == true)
                        {
                            Board.instance.MoveValidatorCoRo(resultingMove.data.startX, resultingMove.data.startY, resultingMove.data.endX, resultingMove.data.endY);
                            Board.instance.canMove = false;
                        }
                    }
                    */

                    //**THIS IS THE ONE THAT USES A MULTIPLE FRAME SEARCH**
                    /*if (!TorokIsMoving)//if not currently looking for a move
                    {
                        resultingMove = new DataHolder<Move>();
                        MinMax.instance.StartCoroutine(MinMax.instance.GetMinMaxMoveCo(resultingMove, MinMax.playerToMove.torok));
                        TorokIsMoving = true;
                    }
                    else//if already looking for move
                    {
                        if (!MinMax.instance.lookingForMove && Board.instance.canMove == true)// <- means finished searching
                        {
                            Board.instance.MoveValidatorCoRo(resultingMove.data.startX, resultingMove.data.startY, resultingMove.data.endX, resultingMove.data.endY);
                            Board.instance.canMove = false;
                        }
                    }
                    */

                    //**THIS IS THE ONE THAT USES A SINGLE FRAME SEARCH IE OLD WORKING METHOD** 
                    /*if (!TorokIsMoving)
                    {
                        TorokIsMoving = true;
                        Move resultMove = MinMax.instance.GetMinMaxMove(playerToMove.torok);
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
                       
                    }*/
                    
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
                MainMenu.instance.pauseFxn.enabled = false;
                if (activeCoRo == null)
                {
                    activeCoRo = StartCoroutine(titleCoRo());
                }
                break;
            case GameState.lose:
                if (activeCoRo == null)
                {
                    activeCoRo = StartCoroutine(LoseCoRo());
                }
                break;
        }
    }

    public IEnumerator titleCoRo()
    {
        Debug.Log("insdie titleCoRo");
        //if (MainMenu.instance.menuDone) {
        while (!CameraHeadMovements.instance.menuDone)
        {
            //CameraHeadMovements.instance.LookAtPlayArea();
            yield return null;
        }

        activeCoRo = null;
        yield return new WaitForSeconds(0.5f);
        ChangeGameState(GameState.intro);
        //}
    }

    public IEnumerator IntroCoRo()
    {
        Debug.Log("insdie IntroCoRo");
        yield return new WaitForSeconds(0.5f);
        yield return CameraHeadMovements.instance.StartCoroutine(CameraHeadMovements.instance.LookAtTorokExclusively());
        
        
        if (winCondition.conditionType == 0)//nonpawn condition
        {
            TorokPersonalityAI.instance.PlayAnimationAndSound(SoundLibrary.Categories.LevelIntroNonPawn);
        }
        else if (winCondition.conditionType == 1)//capture the flag condition
        {
            TorokPersonalityAI.instance.PlayAnimationAndSound(SoundLibrary.Categories.LevelIntroCTF);
        }
        else if (winCondition.conditionType == 2)//checkmate condition
        {
            TorokPersonalityAI.instance.PlayAnimationAndSound(SoundLibrary.Categories.LevelIntroCheckmate); 
        }
        else if (winCondition.conditionType == 3)//king of the hill condition
        {
            TorokPersonalityAI.instance.PlayAnimationAndSound(SoundLibrary.Categories.LevelIntroKOTH);
        }

        //needs to wait for sound to end
        yield return new WaitForSeconds(2f);

        yield return CameraHeadMovements.instance.StartCoroutine(CameraHeadMovements.instance.LookAtBoardExclusively());
        ChangeGameState(GameState.deployment);
        Inventory.instance.ShowInventoryPanel();
        Inventory.instance.SlideShowInventoryPanel();
        MainMenu.instance.pauseFxn.enabled = true;
        
        Inventory.instance.EnableDeployUI();
        activeCoRo = null;
    }

    public IEnumerator WinAnimCoro()
    {
        Inventory.instance.HideInventoryPanel();
        Inventory.instance.objectiveArea.SetActive(false);

        Currency.instance.ticketTextObject.SetActive(true);// remove this once this function gets organized just to see the currency number as the tickets get there
        Currency.instance.ticketBackgroundObject.SetActive(true);
        ticketParticleSystem.SpawnTickets((currentLevelNumber + 1) * 6);//<- needs to be abled to get that number from the currency object for how many tickets you got

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
        PhysicalShop.instance.ResetShop();
        Board.instance.ReturnPiecesToInventory();

        //Currency.instance.GetReward(currentLevelNumber + 1);

        turnCount = 1;
        isPlayersTurn = true;
        ChangeGameState(GameState.shop);
        PhysicalShop.instance.EnterShop();
        activeCoRo = null;
        SaveManager.instance.SaveGame();
    }

    public IEnumerator LoseCoRo()
    {
        //show some type of defeat text or something

        yield return CameraHeadMovements.instance.StartCoroutine(CameraHeadMovements.instance.LookAtTorokExclusively());
        yield return TorokPersonalityAI.instance.StartCoroutine(TorokPersonalityAI.instance.PlayAnimationAndSoundCoRo(SoundLibrary.Categories.LoseGame));
        //yield return new WaitForSeconds(3);
        PauseMenu.instance.ReturnToMainMenu();
    }

    public void SetNextLevel()
    {
        Inventory.instance.resetDeployCount();
        Inventory.instance.SetDeployUI();
        ResetToDeploy();

        Board.instance.DeactivateWinTiles();
        Board.instance.DeactivateDeployTiles();
        Board.instance.Reset();
        Board.instance.ResetTiles();

        //Inventory.instance.hasPlacedPiece = false;
        Inventory.instance.numPiecesPlaced = 0;
        //Inventory.instance.ShowInventoryPanel();

        MinMax.instance.ResetDepth();

        if (currentLevelNumber != -1)
        {
            TorokPersonalityAI.instance.IncreaseAngerLevel();
        }
        

        if (currentLevelNumber + 1 < LevelNames.Count)
        {
            BoardLoader.instance.LoadBoard(LevelNames[++currentLevelNumber]);
        }

        Inventory.instance.SetObjective();

        turnCount = 1;
        Board.instance.ActivateWinTiles();
        Board.instance.ActivateDeployTiles();
        ChangeGameState(GameState.intro);
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
        Board.playerInCheck = Board.instance.IsKingInCheck(false);
        Board.torokInCheck = Board.instance.IsKingInCheck(true);

        //win condition checks
        if (winCondition != null)
        {
            winCondition.ProgressConditionState();
            mostRecentWinCheckResult = winCondition.IsWinCondition();
        }
        else
        {
            Debug.LogError("WinCondition| no win condition is set for this board state");
            return;
        }

        if (mostRecentWinCheckResult == BaseCondition.Condition.Player)
        {
            ChangeGameState(GameState.win);
            return;
            //reset this state
        }
        else if (mostRecentWinCheckResult == BaseCondition.Condition.Torok)
        {
            ChangeGameState(GameState.lose);
            return;
            //Debug.Log("Torok has won.");
            //lose logic
        }


        lastValidateCheck = false;
        InterruptManager.instance.EnactInterrupts(InterruptManager.InterruptTrigger.AfterTurn);
        turnCount++;
        isPlayersTurn = !isPlayersTurn;
        Inventory.instance.SetObjective();
        //print("Current Board Score " + BoardAnalyzer.instance.Analyze(Board.pieceBoard));
        //check win condition

        int torokPieceCount = 0;
        for (int i = 0; i < Board.boardSize; i++)
        {
            for (int j = 0; j < Board.boardSize; j++)
            {
                if (Board.pieceBoard[i,j] == null) { continue; }

                Piece targetPiece = Board.pieceBoard[i,j].GetComponent<Piece>();

                if (targetPiece.isTorok && (int)targetPiece.type <= (int)Piece.PieceType.queen) { 
                    torokPieceCount++;
                }
            }
        }

        if (torokPieceCount <= 1)
        {
            MinMax.instance.SetNewDepth(1);
        }
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
        Inventory.instance.numPiecesPlaced = 0;
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
