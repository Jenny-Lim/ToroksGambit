using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button optionsButton; // to change once there's stuff
    [SerializeField] public PauseMenu pauseFxn;

    [SerializeField] private Animation curtainOpen;

    public static MainMenu instance;
    //public bool startPressed;

    private void Start()
    {
        if (instance == null) { instance = this; }

        //GameStateManager.instance.ChangeGameState(GameStateManager.GameState.title);

        // to change once there's stuff
        //if(SaveManager.instance.hasSaveGame)
        //{
        //    continueButton.interactable = true;

        //}
        //else 
        //{
            //continueButton.interactable = false;
        //}
        //continueButton.interactable = true;
        optionsButton.interactable = false;
        pauseFxn.enabled = false;
        //startPressed = false;
    }



    void OnEnable()
    {
        startButton.interactable = false;
        continueButton.interactable = false;
        //print("yeye");
        Invoke("GoBackToTitle", 1.0f); // it was some shit with execution order
        //GameStateManager.instance.ChangeGameState(GameStateManager.GameState.title);
        CameraHeadMovements.instance.menuDone = false;
    }

    void GoBackToTitle()
    {
        startButton.interactable = true; //sometimes intro doesnt play -- if you press play when not in the title coro. so heres a bandaid fix
        if (SaveManager.instance.hasSaveGame)
        {
            continueButton.interactable = true;
        }
        GameStateManager.instance.ChangeGameState(GameStateManager.GameState.title);
    }

    public void NewGame() // have yet to test fully
    {
        //GameManager.instance.StartNewGame();
        // get first level

        //startPressed = true;

        // reset torok
        TorokPersonalityAI.instance.SetAngerLevel(1);
        InterruptManager.instance.ResetInterruptListTriggers();

        GameStateManager.instance.currentLevelNumber = -1;
        GameStateManager.instance.SetNextLevel(); // bug possibly here

        // reset inventory
        /*
        Inventory.instance.SetPieceAmount((Inventory.InventoryPieces)0, 4); // pawns
        for (int i = 1; i < 3; i++)
        {
            Inventory.instance.SetPieceAmount((Inventory.InventoryPieces)i, 5); // others (no king)
        }
        Inventory.instance.SetPieceAmount((Inventory.InventoryPieces)4, 1); // queen
        */

        // reset currency
        Currency.instance.SetCurrency(0);
        SaveManager.instance.StartNew();

        // move to proper area
        //if (GameStateManager.instance.currentState == GameStateManager.GameState.title)
        //{
            //print("sup");

            curtainOpen.Play("Curtain_Open");
            //CameraHeadMovements.instance.LookAtPlayArea();
        //}

        gameObject.SetActive(false); // hide main menu
    }

    public void ContinueGame()
    {
        SaveManager.instance.LoadSave();
        GameStateManager.instance.SetNextLevel(); // bug possibly here
        curtainOpen.Play("Curtain_Open");
        //CameraHeadMovements.instance.LookAtPlayArea();
        gameObject.SetActive(false); // hide main menu

        // nothing atm -- should save level, angerlvl, interrupt thing??, inventory, currency
    }

    public void Options()
    {
        // nothing atm -- probably difficulty settings, audio, dialogue / cutscenes
    }

    public void ExitGame()
    {
        #if UNITY_STANDALONE
        Application.Quit();
        #endif

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

}
